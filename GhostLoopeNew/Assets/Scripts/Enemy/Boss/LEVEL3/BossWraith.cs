using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UI;
public enum E_WraithStatus
{
    normal,
    broken,
    skill3,
    skill4,
    LOCK
}

public class BossWraith : Enemy
{
    [Header("Boss GrimReaper Setting")]
    public float enemyWalkSpeed = 1.0f;
    public float enemyDashSpeed = 4.0f;
    public float skillCoolDown = 5.0f;

    private HashSet<E_WraithStatus> wraithStatusContainer;

    [Header("Skill 3 Setting")]
    public GameObject leftWingObject;
    public GameObject rightWingObject;
    public int skill3FireTimes = 3;
    public int spinCastTimes = 4;
    public float spinCastRadius = 1.0f;
    public float spinAttackDamage = 100.0f;

    private int currSkill3FireTimes;


    [Header("Skill 4 Setting")]
    public GameObject preChainObject;
    public float riseSpeed = 0.2f;
    public float riseHeight = 3.0f;
    public float grabDistance = 3.0f;
    public float preChainTime = 1.5f;
    public float chainTime = 0.5f;
    public float chainAttackDamage = 100.0f;
    public float retrieveSan = 500.0f;

    private Vector3 currPlayerPosition;
    private int hpBelow80 = 1;
    private int hpBelow60 = 1;
    private int hpBelow40 = 1;
    private int hpBelow20 = 1;
    private float currRiseHeight = 0.0f;
    private Vector3 targetHeightPosition;

    private bool hasSetHeight = false;
    private bool hasRisedUp = false;
    private bool hasReleasedChain = false;

    [Header("Tenacity")]
    public GameObject tenacityObj;
    private Tenacity tenacity;

    private NavMeshAgent agent;

    new protected void OnEnable()
    {
        //Debug.Log("In ChasingStart");
        base.OnEnable();

        // ai
        agent = gameObject.AddComponent<NavMeshAgent>();
        agent.stoppingDistance = 3.0f;
        agent.speed = enemyWalkSpeed;

        // animation event
        AddDieAnimationEvent();
        AddSpinAttackEvent();
        AddChainAttackEvent();

        // cooldown
        currSkill3FireTimes = 0;

        // status
        wraithStatusContainer = new HashSet<E_WraithStatus>();
        wraithStatusContainer.Add(E_WraithStatus.normal);

        // skill4
        EventCenter.GetInstance().AddEventListener(E_Event.BossWraithChainSuccess, ChainSuccessEvent);
        EventCenter.GetInstance().AddEventListener(E_Event.BossWraithChainFail, ChainFailEvent);

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

        // 
        // DontDestroyOnLoad(animator);
    }

    protected void Update()
    {
        //string currStatus = "Current Status: ";
        //foreach (var status in wraithStatusContainer)
        //{
        //    currStatus += status.ToString() + "  ";
        //}
        //Debug.LogWarning(currStatus);
        if (animator == null)
            animator = GetComponent<AnimatorController>();


        // cooldown
        if (!wraithStatusContainer.Contains(E_WraithStatus.LOCK))
        {
            if (currFireCoolDown > 0) currFireCoolDown -= Time.deltaTime;
        }


        float currDistance = GetPlayerDistance();

        // normal status
        if (wraithStatusContainer.Contains(E_WraithStatus.normal) &&
            !wraithStatusContainer.Contains(E_WraithStatus.LOCK))
        {
            //ResetStatus(E_GrimStatus.skill1);
            //ResetStatus(E_GrimStatus.skill2);

            ResetStatus(E_WraithStatus.skill4);
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
                    currSkill3FireTimes++;
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

        // skill3 
        if (wraithStatusContainer.Contains(E_WraithStatus.skill3))
        {
            agent.SetDestination(Player.GetInstance().GetPlayerTransform().position);
            agent.speed = enemyDashSpeed;
            animator.SetBool("SpinAttack", true);
        }


        // skill4
        if (wraithStatusContainer.Contains(E_WraithStatus.skill4))
        {
            if (!hasSetHeight) SetRiseHeight();
            else
            {
                if (!hasRisedUp) RiseUp();
                else
                {
                    if (!hasReleasedChain) ReleaseChain();
                }
            }
        }

        // tenacity
        if (wraithStatusContainer.Contains(E_WraithStatus.broken))
        {
            agent.enabled = false;

            tenacityObj.SetActive(true);
            tenacity.SpawnBullet();

            if (!tenacity.CheckBulletOnScene() || !tenacity.CheckCounterFinish())
            {
                tenacity.DisableTenacity();

                ResetStatus(E_WraithStatus.broken);
                ResetStatus(E_WraithStatus.normal);
                RemoveBrokenStatus();
            }
        }

        if (CheckIfSkill3Status())
        {
            AddStatus(E_WraithStatus.skill3, true);
        }
        if (CheckIfSkill4Status())
        {
            AddStatus(E_WraithStatus.skill4, true);
        }
        if (CheckIfBrokenStatus())
        {
            AddStatus(E_WraithStatus.broken, true);
        }
        CheckHP();

        // update UI value
        enemyRes.value = tenacity.GetCurrentTenacity();
    }

    // status control
    private void AddStatus(E_WraithStatus targetStatus, bool needLock)
    {
        switch (targetStatus)
        {
            case E_WraithStatus.normal:
                wraithStatusContainer.Add(E_WraithStatus.normal);
                break;
            case E_WraithStatus.broken:
                ResetStatus(E_WraithStatus.normal);
                ResetStatus(E_WraithStatus.skill3);
                ResetStatus(E_WraithStatus.skill4);
                wraithStatusContainer.Add(E_WraithStatus.broken);
                break;
            case E_WraithStatus.skill3:
                ResetStatus(E_WraithStatus.normal);
                ResetStatus(E_WraithStatus.skill3);
                ResetStatus(E_WraithStatus.skill4);
                wraithStatusContainer.Add(E_WraithStatus.skill3);
                break;
            case E_WraithStatus.skill4:
                ResetStatus(E_WraithStatus.normal);
                ResetStatus(E_WraithStatus.skill3);
                wraithStatusContainer.Add(E_WraithStatus.skill4);
                break;
        }

        if (needLock) wraithStatusContainer.Add(E_WraithStatus.LOCK);
    }

    private void ResetStatus(E_WraithStatus resetStatus)
    {
        switch (resetStatus)
        {
            case E_WraithStatus.normal:
                moveFrame = 0;

                animator.SetBool("Attack", false);
                animator.SetBool("TakeDamage", false);
                animator.SetFloat("Move", moveFrame);

                agent.enabled = true;
                receiveDamage = false;
                currFireCoolDown = fireCooldown;
                break;
            case E_WraithStatus.skill3:
                currSkill3FireTimes = 0;
                animator.SetBool("SpinAttack", false);
                break;
            case E_WraithStatus.skill4:
                currRiseHeight = 0.0f;
                hasSetHeight = false;
                hasRisedUp = false;
                hasReleasedChain = false;
                agent.enabled = true;
                break;
            case E_WraithStatus.broken:
                tenacity.ResetTanacity();
                break;
        }
    }

    private bool CheckIfSkill3Status()
    {
        return !wraithStatusContainer.Contains(E_WraithStatus.LOCK) &&
                currSkill3FireTimes >= skill3FireTimes;
    }

    private bool CheckIfSkill4Status()
    {
        return !wraithStatusContainer.Contains(E_WraithStatus.LOCK) &&
                CheckCurrentHP();
    }

    private bool CheckIfBrokenStatus()
    {
        return tenacity.CheckTenacityEqualZero() &&
               !wraithStatusContainer.Contains(E_WraithStatus.LOCK);
    }

    public void RemoveSkill3Status()
    {
        wraithStatusContainer.Remove(E_WraithStatus.skill3);
        wraithStatusContainer.Remove(E_WraithStatus.LOCK);

        ResetStatus(E_WraithStatus.skill3);
    }

    private void RemoveSkill4Status()
    {
        animator.SetBool("PullOpponent", false);
        animator.SetBool("WingStab", false);

        wraithStatusContainer.Remove(E_WraithStatus.skill4);
        wraithStatusContainer.Remove(E_WraithStatus.LOCK);
    }

    private void RemoveBrokenStatus()
    {
        wraithStatusContainer.Remove(E_WraithStatus.broken);
        wraithStatusContainer.Remove(E_WraithStatus.LOCK);
    }

    


    // skill3 
    private void SpinAttack()
    {
        animator.SetBool("SpinAttack", true);
    }


    // skill4 
    private bool CheckCurrentHP()
    {
        if (hp <= maxHp * 0.8 && hp > maxHp * 0.6 && hpBelow80 > 0)
        {
            hpBelow80--;
            return true;
        }
        else if (hp <= maxHp * 0.6 && hp > maxHp * 0.4 && hpBelow60 > 0)
        {
            hpBelow60--;
            return true;
        }
        else if (hp <= maxHp * 0.4 && hp > maxHp * 0.2 && hpBelow40 > 0)
        {
            hpBelow40--;
            return true;
        }
        else if (hp <= maxHp * 0.2 && hpBelow20 > 0)
        {
            hpBelow20--;
            return true;
        }
        else return false;
    }

    private void SetRiseHeight()
    {
        targetHeightPosition = new Vector3(transform.position.x, 
                                           transform.position.y + riseHeight, 
                                           transform.position.z);
        hasSetHeight = true;
    }

    private void RiseUp()
    {
        currRiseHeight += Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, targetHeightPosition, currRiseHeight);
        if (transform.position == targetHeightPosition) hasRisedUp = true;
    }

    private void ReleaseChain()
    {
        animator.SetBool("WingStab", true);
        BossWraithPreChain preChain = Instantiate(preChainObject, Player.GetInstance().transform.position, Quaternion.identity).GetComponent<BossWraithPreChain>();
        preChain.Init(preChainTime, chainTime);
        preChain.StartReleaseChain();

        hasReleasedChain = true;
    }


    public void ChainSuccessEvent()
    {
        MusicManager.GetInstance().PlayFireSound("翅膀包裹玩家造成伤害的音效");
        animator.SetBool("PullOpponent", true);
        Player.GetInstance().transform.position = transform.position + transform.forward * grabDistance;
    }

    public void ChainFailEvent()
    {
        RemoveSkill4Status();
    }



    // general function
    private void CheckHP()
    {
        // 判断怪物是否死亡
        if (hp <= 0)
        {
            MusicManager.GetInstance().PlayFireSound("爆炸音效");

            // animation
            animator.SetTrigger("Die");
        }
    }

    private float GetPlayerDistance()
    {
        Vector3 playerPosition = Player.GetInstance().GetPlayerTransform().position;
        float distance = (playerPosition - transform.position).magnitude;
        return distance;
    }


    // animation event
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

    private void AddSpinAttackEvent()
    {
        // slash attack event
        for (int i = 0; i < spinCastTimes; i++)
        {
            AnimationEvent leftWingAttackEvent = new AnimationEvent();
            leftWingAttackEvent.functionName = "LeftSpinAttack";
            leftWingAttackEvent.time = animator.GetClipLength("Spin Wing Slash Attack") * ((float)i / (float)spinCastTimes);

            AnimationEvent rightWingAttackEvent = new AnimationEvent();
            rightWingAttackEvent.functionName = "RightSpinAttack";
            rightWingAttackEvent.time = animator.GetClipLength("Spin Wing Slash Attack") * ((float)i / (float)spinCastTimes);

            // add event
            animator.AddEvent("Spin Wing Slash Attack", leftWingAttackEvent);
            animator.AddEvent("Spin Wing Slash Attack", rightWingAttackEvent);
        }

        // attack finish event
        AnimationEvent attackFinishEvent = new AnimationEvent();
        attackFinishEvent.functionName = "RemoveSkill3Status";
        attackFinishEvent.time = animator.GetClipLength("Spin Wing Slash Attack");

        animator.AddEvent("Spin Wing Slash Attack", attackFinishEvent);
    }

    private void AddChainAttackEvent()
    {
        // slash attack event
        for (int i = 0; i < spinCastTimes; i++)
        {
            AnimationEvent leftWingAttackEvent = new AnimationEvent();
            leftWingAttackEvent.functionName = "LeftChainAttack";
            leftWingAttackEvent.time = animator.GetClipLength("Pull Opponent") * ((float)i / (float)spinCastTimes);

            AnimationEvent rightWingAttackEvent = new AnimationEvent();
            rightWingAttackEvent.functionName = "RightChainAttack";
            rightWingAttackEvent.time = animator.GetClipLength("Pull Opponent") * ((float)i / (float)spinCastTimes);

            // add event
            animator.AddEvent("Pull Opponent", leftWingAttackEvent);
            animator.AddEvent("Pull Opponent", rightWingAttackEvent);
        }

        // attack finish event
        AnimationEvent chainFinishEvent = new AnimationEvent();
        chainFinishEvent.functionName = "RemoveSkill4Status";
        chainFinishEvent.time = 0;

        animator.AddEvent("Pull Opponent", chainFinishEvent);
    }


    // interface
    public void DisableAfterDie(GameObject targetObj)
    {
        if (targetObj.GetComponent<Enemy>().GetEnemyHP() <= 0)
        {
            MusicManager.GetInstance().PlayFireSound("BOSS3-2倒地音效");
            targetObj.SetActive(false);
            agent.enabled = false;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(rightWingObject.transform.position, spinCastRadius);
    //}

    public void LeftSpinAttack()
    {
        MusicManager.GetInstance().PlayFireSound("二阶段旋转移动的音效");

        RaycastHit leftWingHitInfo;
        if (Physics.SphereCast(leftWingObject.transform.position,
                               spinCastRadius,
                               leftWingObject.transform.up,
                               out leftWingHitInfo))
        {
            GameObject hitObj = leftWingHitInfo.collider.gameObject;
            if (hitObj != null && hitObj.tag == "Player")
            {
                Player.GetInstance().PlayerReceiveDamage(spinAttackDamage);
            }
        }
    }

    public void RightSpinAttack()
    {
        MusicManager.GetInstance().PlayFireSound("二阶段旋转移动的音效");

        RaycastHit rightWingHitInfo;
        if (Physics.SphereCast(rightWingObject.transform.position,
                               spinCastRadius,
                               rightWingObject.transform.up,
                               out rightWingHitInfo))
        {
            GameObject hitObj = rightWingHitInfo.collider.gameObject;
            if (hitObj != null && hitObj.tag == "Player")
            {
                Debug.LogWarning("Right Wing Hit player!");
                Player.GetInstance().PlayerReceiveDamage(spinAttackDamage);
            }
        }
    }

    public void LeftChainAttack()
    {
        MusicManager.GetInstance().PlayFireSound("蓄力和发动锁链音效");

        RaycastHit leftWingHitInfo;
        if (Physics.SphereCast(leftWingObject.transform.position,
                               spinCastRadius,
                               leftWingObject.transform.up,
                               out leftWingHitInfo))
        {
            //Debug.LogWarning("Hit Something!" + leftWingHitInfo.collider.name);
            GameObject hitObj = leftWingHitInfo.collider.gameObject;
            if (hitObj != null && hitObj.tag == "Player")
            {
                Debug.LogWarning("Left Wing Hit player!");
                Player.GetInstance().PlayerReceiveDamage(spinAttackDamage);
                this.SetEnemyHP(GetEnemyHP() + retrieveSan);
                if (GetEnemyHP() > maxHp * 0.2) hpBelow20++;
            }
        }
    }

    public void RightChainAttack()
    {
        MusicManager.GetInstance().PlayFireSound("蓄力和发动锁链音效");

        RaycastHit rightWingHitInfo;
        if (Physics.SphereCast(rightWingObject.transform.position,
                               spinCastRadius,
                               rightWingObject.transform.up,
                               out rightWingHitInfo))
        {
            GameObject hitObj = rightWingHitInfo.collider.gameObject;
            if (hitObj != null && hitObj.tag == "Player")
            {
                Debug.LogWarning("Right Wing Hit player!");
                Player.GetInstance().PlayerReceiveDamage(spinAttackDamage);
                this.SetEnemyHP(GetEnemyHP() + retrieveSan);
                if (GetEnemyHP() > maxHp * 0.2) hpBelow20++;
            }
        }
    }

}
