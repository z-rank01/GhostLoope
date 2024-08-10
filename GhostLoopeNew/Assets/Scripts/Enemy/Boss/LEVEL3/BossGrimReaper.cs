using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum E_GrimStatus
{
    normal, 
    broken, 
    skill1, 
    skill2,
    LOCK
}

public class BossGrimReaper : Enemy
{
    [Header("Boss GrimReaper Setting")]
    public float enemyWalkSpeed = 1.0f;
    public float enemyDashSpeed = 4.0f;
    public float skillCoolDown = 5.0f;

    private HashSet<E_GrimStatus> grimStatusContainer;

    [Header("Skill 1 Setting")]
    public GameObject leftHandObject;
    public GameObject rightHandObject;
    public GameObject skill1DestinationObject;
    public float skill1DashingTime = 4.0f;
    public int castTimes = 4;
    public float castRadius = 3.0f;
    public float slashAttackDamage = 10.0f;
    public int skill1FireTimes = 6;

    private int currSkill1FireTimes;
    private float currDashingTime;
    private bool reachTarget = false;
    private bool hasSetTargetPosition = false;
    private Vector3 targetPosition;

    [Header("Skill 2 Setting")]
    public int skill2FireTimes = 4;
    public float skill2SpinTime = 5.0f;

    private bool hasStartedSpinAttack = false;
    private int currSkill2FireTimes;
    private float currSpinTime;
    private float spinAttackFloat;

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
        AddSlashAttackEvent();

        // cooldown
        currSkill1FireTimes = 0;
        currSkill2FireTimes = 0;

        // status
        grimStatusContainer = new HashSet<E_GrimStatus>();
        grimStatusContainer.Add(E_GrimStatus.normal);

        // tenacity
        tenacity = tenacityObj.GetComponent<Tenacity>();
        tenacity.Init();
        tenacity.SetTenacityParent(this.gameObject);
        tenacityObj.SetActive(false);
        EventCenter.GetInstance().AddEventListener<float>(E_Event.TenacityReceiveDamage, this.EnemyReceiveDamage);
    }

    protected void Update()
    {
        // cooldown
        if (!grimStatusContainer.Contains(E_GrimStatus.LOCK))
        {
            if (currFireCoolDown > 0) currFireCoolDown -= Time.deltaTime;
        }
        

        float currDistance = GetPlayerDistance();

        // normal status
        if (grimStatusContainer.Contains(E_GrimStatus.normal) &&
            !grimStatusContainer.Contains(E_GrimStatus.LOCK))
        {
            //ResetStatus(E_GrimStatus.skill1);
            //ResetStatus(E_GrimStatus.skill2);
            // chasing
            if (currDistance > agent.stoppingDistance)
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
                    currSkill1FireTimes++;
                    currSkill2FireTimes++;
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

        // skill1 
        if (grimStatusContainer.Contains(E_GrimStatus.skill1))
        {
            if (!hasSetTargetPosition) SetTarget();
            else
            {
                if (!reachTarget) DashToward();
                else
                {
                    // animation
                    animator.SetBool("SlashAttack", true);
                }
            }
        }


        // skill2
        if (grimStatusContainer.Contains(E_GrimStatus.skill1))
        {
            if (!hasStartedSpinAttack) StartSpinAttack();
            else
            {
                if (!SpinAttack()) EndSpinAttack();
            }
        }


            // tenacity
            if (grimStatusContainer.Contains(E_GrimStatus.broken))
        {
            agent.enabled = false;

            tenacityObj.SetActive(true);
            tenacity.SpawnBullet();

            if (!tenacity.CheckBulletOnScene() || !tenacity.CheckCounterFinish())
            {
                tenacity.DisableTenacity();

                ResetStatus(E_GrimStatus.broken);
                ResetStatus(E_GrimStatus.normal);
                RemoveBrokenStatus();
            }
        }

        if (CheckIfSkill1Status())
        {
            AddStatus(E_GrimStatus.skill1, true);
        }
        if (CheckIfSkill2Status())
        {
            AddStatus(E_GrimStatus.skill2, true);
        }
        if (CheckIfBrokenStatus())
        {
            AddStatus(E_GrimStatus.broken, true);
        }
        CheckHP();
    }


    // status control
    private void AddStatus(E_GrimStatus targetStatus, bool needLock)
    {
        switch (targetStatus)
        {
            case E_GrimStatus.normal:
                grimStatusContainer.Add(E_GrimStatus.normal);
                break;
            case E_GrimStatus.broken:
                ResetStatus(E_GrimStatus.normal);
                ResetStatus(E_GrimStatus.skill1);
                ResetStatus(E_GrimStatus.skill2);
                grimStatusContainer.Add(E_GrimStatus.broken);
                break;
            case E_GrimStatus.skill1:
                grimStatusContainer.Add(E_GrimStatus.skill1);
                break;
            case E_GrimStatus.skill2:
                grimStatusContainer.Add(E_GrimStatus.skill2);
                break;
        }

        if (needLock) grimStatusContainer.Add(E_GrimStatus.LOCK);
    }

    private void ResetStatus(E_GrimStatus resetStatus)
    {
        switch (resetStatus)
        {
            case E_GrimStatus.normal:
                moveFrame = 0;

                animator.SetBool("Attack", false);
                animator.SetBool("TakeDamage", false);
                animator.SetFloat("Move", moveFrame);

                agent.enabled = true;
                receiveDamage = false;
                break;
            case E_GrimStatus.skill1:
                currSkill1FireTimes = 0;
                animator.SetBool("SlashAttack", false);
                hasSetTargetPosition = false;
                reachTarget = false;
                break;
            case E_GrimStatus.skill2:
                currSkill2FireTimes = 0;
                hasStartedSpinAttack = false;
                spinAttackFloat = 0.0f;
                currSpinTime = 0.0f;
                break;
            case E_GrimStatus.broken:
                tenacity.ResetTanacity();
                break;
        }
    }

    private bool CheckIfSkill1Status()
    {
        return !grimStatusContainer.Contains(E_GrimStatus.LOCK) &&
               currSkill1FireTimes >= skill1FireTimes;
    }

    private bool CheckIfSkill2Status()
    {
        return !grimStatusContainer.Contains(E_GrimStatus.LOCK) &&
                currSkill2FireTimes >= skill2FireTimes;
    }


    private bool CheckIfBrokenStatus()
    {
        return tenacity.CheckTenacityEqualZero() &&
               !grimStatusContainer.Contains(E_GrimStatus.LOCK);
    }

    public void RemoveSkill1Status()
    {
        grimStatusContainer.Remove(E_GrimStatus.skill1);
        grimStatusContainer.Remove(E_GrimStatus.LOCK);
    }

    public void RemoveSkill2Status()
    {
        grimStatusContainer.Remove(E_GrimStatus.skill2);
        grimStatusContainer.Remove(E_GrimStatus.LOCK);
    }

    private void RemoveBrokenStatus()
    {
        grimStatusContainer.Remove(E_GrimStatus.broken);
        grimStatusContainer.Remove(E_GrimStatus.LOCK);
    }


    // skill1
    private void SetTarget()
    {
        // instantiate target object
        targetPosition = Player.GetInstance().transform.position;
        skill1DestinationObject = Instantiate(skill1DestinationObject, targetPosition, Quaternion.identity);

        // set agent
        agent.SetDestination(skill1DestinationObject.transform.position);
        agent.speed = enemyDashSpeed;

        hasSetTargetPosition = true;
    }

    private void DashToward()
    {
        // counter if skill time's up
        if (currDashingTime > 0) currDashingTime -= Time.deltaTime;
        else
        {
            reachTarget = true;
            currDashingTime = skill1DashingTime;
        }

        // dash (for seconds)
        float distance = (transform.position - targetPosition).magnitude;
        if (distance > agent.stoppingDistance)
        {
            // animate
            moveFrame += Time.deltaTime;
            moveFrame = moveFrame > 1.0f ? 1.0f : moveFrame;
            animator.SetFloat("Move", moveFrame);
        }
        else
        {
            // animate
            moveFrame -= Time.deltaTime;
            moveFrame = moveFrame < 0 ? 0 : moveFrame;
            animator.SetFloat("Move", moveFrame);

            reachTarget = true;
        }
    }

    // skill2
    private void StartSpinAttack()
    {
        currSpinTime += Time.deltaTime;
        spinAttackFloat += Time.deltaTime;
        animator.SetBool("StartSpinAttack", true);
        if (spinAttackFloat <= 0.5f)
        {
            animator.SetFloat("SpinAtack", spinAttackFloat);
        }
        else
        {
            spinAttackFloat = 0.5f;
            hasStartedSpinAttack = true;
        }
        
    }

    private bool SpinAttack()
    {
        currSpinTime += Time.deltaTime;
        if (currSpinTime <= skill2SpinTime - 0.5f) return true;
        else return false;
    }

    private void EndSpinAttack()
    {
        currSpinTime += Time.deltaTime;
        spinAttackFloat += Time.deltaTime;
        if (spinAttackFloat <= 1.0f)
        {
            animator.SetFloat("SpinAttack", spinAttackFloat);
        }
        else
        {
            animator.SetBool("StartSpinAttack", false);
            RemoveSkill2Status();
            ResetStatus(E_GrimStatus.skill2);
        }
    }


    // general function
    private void CheckHP()
    {
        // ÅÐ¶Ï¹ÖÎïÊÇ·ñËÀÍö
        if (hp <= 0)
        {
            MusicManager.GetInstance().PlayFireSound("òùòð¹Ö±¬Õ¨ÒôÐ§");

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

    private void AddSlashAttackEvent()
    {
        // slash attack event
        for (int i = 1; i <= castTimes; i++)
        {
            AnimationEvent leftHandAttackEvent = new AnimationEvent();
            leftHandAttackEvent.functionName = "LeftSlashAttack";
            leftHandAttackEvent.time = animator.GetClipLength("Slash Attack 01") * ((float)i / (float)castTimes);

            AnimationEvent rightHandAttackEvent = new AnimationEvent();
            rightHandAttackEvent.functionName = "RightSlashAttack";
            rightHandAttackEvent.time = animator.GetClipLength("Slash Attack 02") * ((float)i / (float)castTimes);

            // add event
            animator.AddEvent("Slash Attack 01", leftHandAttackEvent);
            animator.AddEvent("Slash Attack 02", rightHandAttackEvent);
        }

        // attack finish event
        AnimationEvent attackFinishEvent = new AnimationEvent();
        attackFinishEvent.functionName = "ResetSkill1Status";
        attackFinishEvent.time = animator.GetClipLength("Slash Attack 02");

        animator.AddEvent("Slash Attack 02", attackFinishEvent);
    }


    // interface
    public void DisableAfterDie(GameObject targetObj)
    {
        targetObj.SetActive(false);
        agent.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(leftHandObject.transform.position,
                          castRadius);
    }


    public void LeftSlashAttack()
    {
        RaycastHit leftHandHitInfo;
        if (Physics.SphereCast(leftHandObject.transform.position,
                               castRadius,
                               transform.forward,
                               out leftHandHitInfo))
        {
            //Debug.LogWarning("Hit Something!" + leftHandHitInfo.collider.name);
            GameObject hitObj = leftHandHitInfo.collider.gameObject;
            if (hitObj.tag == "Player")
            {
                //Debug.LogWarning("Hit player!");
                hitObj.GetComponent<Player>().PlayerReceiveDamage(slashAttackDamage);
            }
        }
    }

    public void RightSlashAttack()
    {
        RaycastHit rightHandHitInfo;
        if (Physics.SphereCast(rightHandObject.transform.position,
                               castRadius,
                               transform.forward,
                               out rightHandHitInfo))
        {
            GameObject hitObj = rightHandHitInfo.collider.gameObject;
            if (hitObj.tag == "Player")
            {
                //Debug.LogWarning("Hit player!");
                hitObj.GetComponent<Player>().PlayerReceiveDamage(slashAttackDamage);
            }
        }
    }

    public void ResetSkill1Status()
    {
        RemoveSkill1Status();
        ResetStatus(E_GrimStatus.skill1);
    }
}