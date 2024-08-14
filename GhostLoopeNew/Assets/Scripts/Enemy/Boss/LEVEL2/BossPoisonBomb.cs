using BehaviorDesigner.Runtime.ObjectDrawers;
using BehaviorDesigner.Runtime.Tasks.Unity.Math;
using Lightbug.LaserMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public enum E_PoisonBombStatus
{
    normal, 
    broken, 
    skill1, 
    skill2, 
    skill3,
    LOCK
}

public class BossPoisonBomb : Enemy
{
    [Header("Boss PoisonBomb Setting")]
    public float enemyWalkSpeed = 4.0f;
    public float enemyRunSpeed = 8.0f;
    public int fireTimes = 2;

    private HashSet<E_PoisonBombStatus> poisonBombStatusContainer;
    private float currSkill1Time;
    private int currfireTimes;

    [Header("Skill 1 Setting")]
    public GameObject BombPrefab;
    public float skill1CoolDown = 2.0f;


    [Header("Skill 2 Setting")]
    public GameObject crossboneObject;
    public float preThrowTime = 0.2f;

    private bool hasStartCrossboneAttack = false;
    private Crossbone crossbone;



    [Header("Skill 3 Setting")]
    public E_PoolType bulletType;
    public float skill3Time = 2.0f;
    public float skill3Frequency = 4f;
    public float skill3DirectionNumber = 8;
    public float rotateSpeed = 1.0f;

    private bool skill3Ended = false;
    private float currSkill3Time;

    [Header("Tenacity")]
    public GameObject tenacityObj;
    private Tenacity tenacity;


    private NavMeshAgent agent;

    new protected void OnEnable()
    {
        base.OnEnable();

        // animator event
        AddCrossboneAttackAnimationEvent();
        AddDieAnimationEvent();

        // status
        poisonBombStatusContainer = new HashSet<E_PoisonBombStatus>();
        poisonBombStatusContainer.Add(E_PoisonBombStatus.normal);

        // ai
        // 不知道为什么，设置不了父物体呀
        //Transform BossSpawnPoint = GameObject.Find("BossSpawnPoint").transform;
        //Debug.Log("BossSpawn: " + BossSpawnPoint);
        //transform.SetParent(BossSpawnPoint, false); // 加AI后再设置位置会导致AI的碰撞体和场景阻挡
        
        // 只能手动设个固定的位置了
        transform.SetLocalPositionAndRotation(new Vector3(0,0,15), Quaternion.identity);

        // 就是这个问题，加了AI后在设置位置会被场景中的物体碰撞


        agent = gameObject.AddComponent<NavMeshAgent>();
        agent.stoppingDistance = 3.0f;
        agent.speed = enemyWalkSpeed;

        // cooldown
        currSkill1Time = skill1CoolDown;
        currfireTimes = 0;


        // skill1
        PoolManager.GetInstance().AddPool(E_PoolType.BossPoisonBomb, BombPrefab);

        // skill2
        crossbone = crossboneObject.GetComponent<Crossbone>();

        // skill3
        currSkill3Time = 0;

        // tenacity
        tenacity = tenacityObj.GetComponent<Tenacity>();
        tenacity.Init();
        tenacity.SetTenacityParent(this.gameObject);
        tenacityObj.SetActive(false);
        EventCenter.GetInstance().AddEventListener<float>(E_Event.TenacityReceiveDamage, this.EnemyReceiveDamage);

        // UI
        enemySan = GameObject.Find("Enemy_San").GetComponent<Slider>();
        enemySan.value = enemySan.maxValue = maxHp;
        enemyRes = GameObject.Find("Enemy_Res").GetComponent<Slider>();
        enemyRes.value = enemyRes.maxValue = tenacity.tenacity;
    }

    protected void Update()
    {
        string currStatus = "Current Status: ";
        foreach (var status in poisonBombStatusContainer)
        {
            currStatus += status.ToString() + "  ";
        }
        Debug.Log(currStatus);


        // cooldown
        if (currFireCoolDown > 0) currFireCoolDown -= Time.deltaTime;
        if (currSkill1Time > 0) currSkill1Time -= Time.deltaTime;

        float currDistance = GetPlayerDistance();

        // normal status
        if (poisonBombStatusContainer.Contains(E_PoisonBombStatus.normal) &&
            !poisonBombStatusContainer.Contains(E_PoisonBombStatus.LOCK))
        {
            ResetStatus(E_PoisonBombStatus.skill2);

            // chasing
            if (currDistance <= alertDistance)
            {
                agent.SetDestination(Player.GetInstance().GetPlayerTransform().position);
                agent.speed = enemyWalkSpeed;

                // animate
                moveFrame += Time.deltaTime;
                moveFrame = moveFrame > 0.5f ? 0.5f : moveFrame;
                animator.SetFloat("Move", moveFrame);
            }
            else
            {
                // animate
                moveFrame -= Time.deltaTime;
                moveFrame = moveFrame < 0 ? 0 : moveFrame;
                animator.SetFloat("Move", moveFrame);
            }

            // simple attack
            if (currDistance <= attackDistance)
            {
                if (currFireCoolDown <= 0)
                {
                    //animator.SetBool("Attack", true);
                    SimpleFire();
                    currfireTimes++;
                    currFireCoolDown = fireCooldown;
                }
                else animator.SetBool("Attack", false);
            }
            else
            {
                animator.SetBool("Attack", false);
            }

            // animator when receiving damage
            if (receiveDamage)
            {
                animator.SetBool("TakeDamage", true);
                tenacity.DecreaseTenacity(currReceivedDamage);
                receiveDamage = false;
            }
            else
                animator.SetBool("TakeDamage", false);
        }

        // skill 1
        if (poisonBombStatusContainer.Contains(E_PoisonBombStatus.skill1))
        {
            // animate

            // agent
            // agent.enabled = false;

            // event
            ThrowBomb();
            RemoveSkill1Status();
            ResetStatus(E_PoisonBombStatus.skill1);
        }


        // skill 2
        if (poisonBombStatusContainer.Contains(E_PoisonBombStatus.skill2))
        {
            // agent
            agent.enabled = false;

            if (!hasStartCrossboneAttack)
                StartCrossboneAttack();
            
        }

        // skill3
        if (poisonBombStatusContainer.Contains(E_PoisonBombStatus.skill3))
        {
            RotateAndFire();
            if (skill3Ended)
            {
                RemoveSkill3Status();
                ResetStatus(E_PoisonBombStatus.skill3);
            }
        }

        // tenacity
        if (poisonBombStatusContainer.Contains(E_PoisonBombStatus.broken))
        {
            agent.enabled = false;

            tenacityObj.SetActive(true);
            tenacity.SpawnBullet();

            // Debug.Log("bullet on scene: " + tenacity.CheckBulletOnScene());
            if (!tenacity.CheckBulletOnScene() || !tenacity.CheckCounterFinish())
            {
                tenacity.DisableTenacity();

                ResetStatus(E_PoisonBombStatus.normal);
                ResetStatus(E_PoisonBombStatus.broken);

                RemoveBrokenStatus();

                // after recovery, exert skill3
                AddStatus(E_PoisonBombStatus.skill3, true);
            }
        }


        if (CheckIfSkill1())
        {
            AddStatus(E_PoisonBombStatus.skill1, true);
        }
        if (CheckIfSkill2())
        {
            AddStatus(E_PoisonBombStatus.skill2, true);
        }
        if (CheckIfBrokenStatus())
        {
            AddStatus(E_PoisonBombStatus.broken, true);
        }
        CheckHP();

        // update UI value
        enemyRes.value = tenacity.GetCurrentTenacity();
    }


    // status control
    private void AddStatus(E_PoisonBombStatus targetStatus, bool needBlock)
    {
        switch (targetStatus)
        {
            case E_PoisonBombStatus.normal:
                poisonBombStatusContainer.Add(E_PoisonBombStatus.normal);
                break;
            case E_PoisonBombStatus.skill1:
                ResetStatus(E_PoisonBombStatus.normal);
                poisonBombStatusContainer.Add(E_PoisonBombStatus.skill1);
                break;
            case E_PoisonBombStatus.skill2:
                poisonBombStatusContainer.Add(E_PoisonBombStatus.skill2);
                break;
            case E_PoisonBombStatus.skill3:
                ResetStatus(E_PoisonBombStatus.normal);
                poisonBombStatusContainer.Add(E_PoisonBombStatus.skill3);
                break;
            case E_PoisonBombStatus.broken:

                ResetStatus(E_PoisonBombStatus.normal);
                poisonBombStatusContainer.Add(E_PoisonBombStatus.broken);
                break;
        }

        if (needBlock) poisonBombStatusContainer.Add(E_PoisonBombStatus.LOCK);
    }

    private void ResetStatus(E_PoisonBombStatus resetStatus)
    {
        switch (resetStatus)
        {
            case E_PoisonBombStatus.normal:
                moveFrame = 0;

                animator.SetBool("Attack", false);
                animator.SetBool("TakeDamage", false);
                animator.SetFloat("Move", moveFrame);

                agent.enabled = true;
                receiveDamage = false;
                currfireTimes = 0;
                break;

            case E_PoisonBombStatus.skill1:
                currSkill1Time = skill1CoolDown;
                break;

            case E_PoisonBombStatus.skill2:
                hasStartCrossboneAttack = false;
                break;

            case E_PoisonBombStatus.skill3:
                currSkill3Time = 0;
                skill3Ended = false;
                break;

            case E_PoisonBombStatus.broken:
                tenacity.ResetTanacity();
                break;
        }
    }

    private bool CheckIfBrokenStatus()
    {
        return tenacity.CheckTenacityEqualZero() &&
               !poisonBombStatusContainer.Contains(E_PoisonBombStatus.LOCK);
    }

    private bool CheckIfSkill1()
    {
        return !poisonBombStatusContainer.Contains(E_PoisonBombStatus.LOCK) &&
                currSkill1Time < 0 &&
                GetPlayerDistance() > attackDistance;
    }

    private bool CheckIfSkill2()
    {
        return !poisonBombStatusContainer.Contains(E_PoisonBombStatus.LOCK) &&
                currfireTimes >= fireTimes &&
                GetPlayerDistance() <= attackDistance;
    }
    
    private void RemoveNormalStatus()
    {
        poisonBombStatusContainer.Remove(E_PoisonBombStatus.normal);
    }

    private void RemoveBrokenStatus()
    {
        poisonBombStatusContainer.Remove(E_PoisonBombStatus.broken);
        poisonBombStatusContainer.Remove(E_PoisonBombStatus.LOCK);
    }

    private void RemoveSkill1Status()
    {
        poisonBombStatusContainer.Remove(E_PoisonBombStatus.skill1);
        poisonBombStatusContainer.Remove(E_PoisonBombStatus.LOCK);
    }

    public void RemoveSkill2Status()
    {
        poisonBombStatusContainer.Remove(E_PoisonBombStatus.skill2);
        poisonBombStatusContainer.Remove(E_PoisonBombStatus.LOCK);
    }

    public void RemoveSkill3Status()
    {
        poisonBombStatusContainer.Remove(E_PoisonBombStatus.skill3);
        poisonBombStatusContainer.Remove(E_PoisonBombStatus.LOCK);
    }

    // skill1
    private void ThrowBomb()
    {
        MusicManager.GetInstance().PlayFireSound("扔炸弹的声音");

        BombOfBossPoison bomb = PoolManager.GetInstance().GetObj(E_PoolType.BossPoisonBomb).GetComponent<BombOfBossPoison>();
        bomb.SetOrigin(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
        bomb.throwOut();
    }

    // skill2
    private void StartCrossboneAttack()
    {
        hasStartCrossboneAttack = true;
        StartCoroutine(CrossboneAttack());
    }

    private IEnumerator CrossboneAttack()
    {
        yield return new WaitForSeconds(preThrowTime);

        // event
        transform.LookAt(Player.GetInstance().transform);
        float crossBoneDistance = GetPlayerDistance() / attackDistance;

        // music
        MusicManager.GetInstance().PlayFireSound("回旋镖音效");

        // animate 
        animator.SetBool("StartCrossboneAttack", true);   
        animator.SetFloat("CrossboneAttack", crossBoneDistance);
    }
    

    // skill3
    private void RotateAndFire()
    {
        // rotate
        transform.Rotate(transform.up, 0.1f * rotateSpeed);

        // music
        // MusicManager.GetInstance().PlayFireSound("底盘旋转音效");
        // MusicManager.GetInstance().PlayFireSound("第二关boss弹幕子弹（技能）音效");

        // get direction and fire
        currSkill3Time += Time.deltaTime;
        if (currSkill3Time % (skill3Time / skill3Frequency) <= Time.deltaTime)
        {
            for (int i = 1; i <= skill3DirectionNumber; i++)
            {
                float angle = Mathf.Lerp(0, 360.0f, i / skill3DirectionNumber);
                Vector3 direction = Quaternion.AngleAxis(angle, transform.up) * transform.forward;
                var bullet = PoolManager.GetInstance().GetObj(bulletType).GetComponent<Bullet>();
                bullet.FireOut(transform.position + direction * bulletSpawnDistance,
                               direction,
                               GlobalSetting.GetInstance().bulletSpeed);
            }
        }
        // count
        
        if (currSkill3Time >= skill3Time) skill3Ended = true;
    }


    // general function
    private float GetPlayerDistance()
    {
        Vector3 playerPosition = Player.GetInstance().GetPlayerTransform().position;
        float distance = (playerPosition - transform.position).magnitude;
        return distance;
    }

    private void CheckHP()
    {
        enemySan.value = hp;

        // 判断怪物是否死亡
        if (hp <= 0)
        {
            Player.GetInstance().SetIsFightingBoss(false); // 设置未处于Boss战状态，取消显示怪物血条
            // MusicManager.GetInstance().PlayFireSound("爆炸音效");
            RemoveNormalStatus();
            agent.enabled = false;

            // animation
            animator.SetTrigger("Die");
        }
    }

    private void AddDieAnimationEvent()
    {
        // event: disable object
        AnimationEvent dieEvent = new AnimationEvent();
        dieEvent.functionName = "DisableAfterDie";
        dieEvent.time = animator.GetClipLength("Die");
        dieEvent.objectReferenceParameter = this.gameObject;

        // add event
        animator.AddEvent("Die", dieEvent);
    }

    private void AddCrossboneAttackAnimationEvent()
    {
        // event: finish attack
        AnimationEvent crossboneAttackEvent = new AnimationEvent();
        crossboneAttackEvent.functionName = "FinishCrossboneAttack";
        crossboneAttackEvent.time = animator.GetClipLength("Crossbone Attack");
        crossboneAttackEvent.objectReferenceParameter = this.gameObject;

        // event: finish long range attack
        AnimationEvent crossboneLongAttackEvent = new AnimationEvent();
        crossboneLongAttackEvent.functionName = "FinishCrossboneAttack";
        crossboneLongAttackEvent.time = animator.GetClipLength("Crossbone Long Range Attack");
        crossboneLongAttackEvent.objectReferenceParameter = this.gameObject;

        // add event
        animator.AddEvent("Crossbone Attack", crossboneAttackEvent);
        animator.AddEvent("Crossbone Long Range Attack", crossboneLongAttackEvent);
    }


    // interface
    public void DisableAfterDie(GameObject targetObj)
    {
        // MusicManager.GetInstance().PlayFireSound("boss死亡(自爆）音效");
        targetObj.SetActive(false);

        GameObject bulletObj = PoolManager.GetInstance().GetObj(E_PoolType.ExplodeBullet);
        targetObj.GetComponent<BossPoisonBomb>().EnemyReceiveDamage(bulletObj.GetComponent<Bullet>());
        PoolManager.GetInstance().ReturnObj(E_PoolType.ExplodeBullet, bulletObj);
    }

    public void FinishCrossboneAttack(GameObject targetObj)
    {
        BossPoisonBomb bossPoisonBomb = targetObj.GetComponent<BossPoisonBomb>();
        bossPoisonBomb.ResetStatus(E_PoisonBombStatus.normal);
        bossPoisonBomb.animator.SetBool("StartCrossboneAttack", false);
        bossPoisonBomb.RemoveSkill2Status();
    }
}