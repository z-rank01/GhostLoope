using BehaviorDesigner.Runtime.Tasks.Unity.UnityPhysics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;


public enum E_ShadowStatus
{
    LOCK, 
    skill2,
    skill3, 
    normal, 
    broken
}

public class BossShadow : Enemy
{
    [Header("Boss Shadow Setting")]
    public float enemyWalkSpeed = 4.0f;
    public float enemyRunSpeed = 8.0f;
    public float skillCoolDown = 5.0f;
    public int fireTimes = 3;

    //private E_ShadowStatus shadowStatus;
    private HashSet<E_ShadowStatus> shadowsStatusContainer;
    private float currSkillTime;
    private int currfireTimes;

    [Header("Skill 2 Setting")]
    public GameObject leftHandObject;
    public GameObject rightHandObject;
    public GameObject skill2DestinationObject;
    public float castRadius = 3.0f;
    public int castTimes = 4;
    public float slashAttackDamage = 10.0f;
    public float skill2DashingTime = 4.0f;

    private float currDashingTime;
    private bool reachTarget = false;
    private bool hasSetTargetPosition = false;
    private Vector3 targetPosition;

    [Header("Skill 3 Setting")]
    public GameObject hintRangePrefab;
    public float rangeAttackRadius = 5.0f;
    public float rangeAttackDamage = 40.0f;
    public int preAttackSeconds = 2;

    private bool hasStartedRangeAttack = false;
    private GameObject hintRangeObject;

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
        currSkillTime = skillCoolDown;
        currDashingTime = skill2DashingTime;
        currfireTimes = 0;

        // status
        shadowsStatusContainer = new HashSet<E_ShadowStatus>();
        shadowsStatusContainer.Add(E_ShadowStatus.normal);

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
        // cooldown
        if (!shadowsStatusContainer.Contains(E_ShadowStatus.LOCK))
        {
            if (currFireCoolDown > 0) currFireCoolDown -= Time.deltaTime;
        }
        if (currSkillTime > 0) currSkillTime -= Time.deltaTime;

        float currDistance = GetPlayerDistance();

        // normal status
        if (shadowsStatusContainer.Contains(E_ShadowStatus.normal) &&
            !shadowsStatusContainer.Contains(E_ShadowStatus.LOCK))
        {
            ResetStatus(E_ShadowStatus.skill2);
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

        

        // skill 2
        if (shadowsStatusContainer.Contains(E_ShadowStatus.skill2))
        {
            // skill2
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

        // skill3
        if (shadowsStatusContainer.Contains(E_ShadowStatus.skill3))
        {
            agent.enabled = false;
            RangeAttack();
        }


        // tenacity
        if (shadowsStatusContainer.Contains(E_ShadowStatus.broken))
        {
            agent.enabled = false;

            tenacityObj.SetActive(true);
            tenacity.SpawnBullet();
            
            if (!tenacity.CheckBulletOnScene() || !tenacity.CheckCounterFinish())
            {
                tenacity.DisableTenacity();

                ResetStatus(E_ShadowStatus.broken);
                ResetStatus(E_ShadowStatus.normal);
                RemoveBrokenStatus();
            }
        }


        // check status
        if (CheckIfSkill2Status(currDistance))
        {
            currSkillTime = skillCoolDown;
            currfireTimes = 0;
            AddStatus(E_ShadowStatus.skill2, true);
        }
        if (CheckIfSkill3Status(currDistance))
        {
            currSkillTime = skillCoolDown;
            currfireTimes = 0;
            AddStatus(E_ShadowStatus.skill3, true);
        }
        if (CheckIfBrokenStatus())
        {
            currfireTimes = 0;
            AddStatus(E_ShadowStatus.broken, true);
        }
        CheckHP();

        // update UI value
        enemyRes.value = tenacity.GetCurrentTenacity();
    }


    // status control
    private void AddStatus(E_ShadowStatus targetStatus, bool needLock)
    {
        switch (targetStatus)
        {
            case E_ShadowStatus.normal:
                shadowsStatusContainer.Add(E_ShadowStatus.normal);
                break;
            case E_ShadowStatus.broken:
                ResetStatus(E_ShadowStatus.normal);
                ResetStatus(E_ShadowStatus.skill2);
                shadowsStatusContainer.Add(E_ShadowStatus.broken);
                break;
            case E_ShadowStatus.skill2:
                ResetStatus(E_ShadowStatus.normal);
                shadowsStatusContainer.Add(E_ShadowStatus.skill2);
                break;
            case E_ShadowStatus.skill3:
                ResetStatus(E_ShadowStatus.normal);
                shadowsStatusContainer.Add(E_ShadowStatus.skill3);
                break;
        }

        if (needLock) shadowsStatusContainer.Add(E_ShadowStatus.LOCK);
    }

    private void ResetStatus(E_ShadowStatus resetStatus)
    {
        switch (resetStatus)
        {
            case E_ShadowStatus.normal:
                moveFrame = 0;

                animator.SetBool("Attack", false);
                animator.SetBool("TakeDamage", false);
                animator.SetFloat("Move", moveFrame);

                agent.enabled = true;
                receiveDamage = false;
                currfireTimes = 0;
                break;
            case E_ShadowStatus.skill2:
                animator.SetBool("SlashAttack", false);

                hasSetTargetPosition = false;
                reachTarget = false;
                break;
            case E_ShadowStatus.skill3:
                hintRangeObject = null;
                agent.enabled = true;
                hasStartedRangeAttack = false;
                break;
            case E_ShadowStatus.broken:
                tenacity.ResetTanacity();
                break;
        }
    }

    private bool CheckIfSkill2Status(float currDistance)
    {
        return !shadowsStatusContainer.Contains(E_ShadowStatus.LOCK) &&
               currSkillTime <= 0 &&
               currfireTimes >= fireTimes &&
               currDistance > rangeAttackRadius;
    }

    private bool CheckIfSkill3Status(float currDistance)
    {
        return !shadowsStatusContainer.Contains(E_ShadowStatus.LOCK) &&
               currSkillTime <= 0 &&
               currfireTimes >= fireTimes &&
               currDistance <= rangeAttackRadius;
    }

    private bool CheckIfBrokenStatus()
    {
        return tenacity.CheckTenacityEqualZero() &&
               !shadowsStatusContainer.Contains(E_ShadowStatus.LOCK);
    }

    public void RemoveSkill2Status()
    {
        shadowsStatusContainer.Remove(E_ShadowStatus.skill2);
        shadowsStatusContainer.Remove(E_ShadowStatus.LOCK);
    }

    private void RemoveSkill3Status()
    {
        shadowsStatusContainer.Remove(E_ShadowStatus.skill3);
        shadowsStatusContainer.Remove(E_ShadowStatus.LOCK);
    }

    private void RemoveBrokenStatus()
    {
        shadowsStatusContainer.Remove(E_ShadowStatus.broken);
        shadowsStatusContainer.Remove(E_ShadowStatus.LOCK);
    }


    // skill 2
    private void SetTarget()
    {
        // instantiate target object
        targetPosition = Player.GetInstance().transform.position;
        skill2DestinationObject = Instantiate(skill2DestinationObject, targetPosition, Quaternion.identity);
        
        // set agent
        agent.SetDestination(skill2DestinationObject.transform.position);
        agent.speed = enemyRunSpeed;
        
        hasSetTargetPosition = true;
    }

    private void DashToward()
    {
        // counter if skill time's up
        if (currDashingTime > 0) currDashingTime -= Time.deltaTime;
        else
        {
            reachTarget = true;
            currDashingTime = skill2DashingTime;
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


    // skill 3
    private void RangeAttack()
    {
        if (!hasStartedRangeAttack)
        {
            hasStartedRangeAttack = true;
            StartCoroutine(PreAttack());
        }
    }

    IEnumerator PreAttack()
    {
        // start animation
        animator.SetBool("Attack", true);

        // geenrate hint
        if (hintRangeObject == null)
        {
            hintRangeObject = Instantiate(hintRangePrefab, this.transform);
            hintRangeObject.transform.localPosition = Vector3.zero;
        }

        yield return new WaitForSeconds(preAttackSeconds);

        ExertExplosion();

        // finish animation
        animator.SetBool("Attack", false);

        // destroy object
        DestroyImmediate(hintRangeObject);
        

        // switch to normal
        RemoveSkill3Status();
        ResetStatus(E_ShadowStatus.skill3);
    }

    private void ExertExplosion()
    {
        // exert effect
        if (GetPlayerDistance() < rangeAttackRadius)
            Player.GetInstance().PlayerReceiveDamage(rangeAttackDamage);
    }



    // general function
    private void CheckHP()
    {
        if(enemySan) enemySan.value = hp;
        // ÅÐ¶Ï¹ÖÎïÊÇ·ñËÀÍö
        if (hp <= 0)
        {
            MusicManager.GetInstance().PlayFireSound("òùòð¹Ö±¬Õ¨ÒôÐ§");

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

    private void AddSlashAttackEvent()
    {
        // slash attack event
        for (int i = 1; i <= castTimes; i++)
        {
            AnimationEvent leftHandAttackEvent = new AnimationEvent();
            leftHandAttackEvent.functionName = "LeftSlashAttack";
            leftHandAttackEvent.time = animator.GetClipLength("Left Slash Attack") * ((float)i / (float)castTimes);
            leftHandAttackEvent.objectReferenceParameter = this.gameObject;

            AnimationEvent rightHandAttackEvent = new AnimationEvent();
            rightHandAttackEvent.functionName = "RightSlashAttack";
            rightHandAttackEvent.time = animator.GetClipLength("Right Slash Attack") * ((float)i / (float)castTimes);
            rightHandAttackEvent.objectReferenceParameter = this.gameObject;

            // add event
            animator.AddEvent("Left Slash Attack", leftHandAttackEvent);
            animator.AddEvent("Right Slash Attack", rightHandAttackEvent);
        }

        // attack finish event
        AnimationEvent attackFinishEvent = new AnimationEvent();
        attackFinishEvent.functionName = "RemoveSkill2Status";
        attackFinishEvent.time = animator.GetClipLength("Right Slash Attack");
        attackFinishEvent.objectReferenceParameter = this.gameObject;

        animator.AddEvent("Right Slash Attack", attackFinishEvent);
    }


    private float GetPlayerDistance()
    {
        Vector3 playerPosition = Player.GetInstance().GetPlayerTransform().position;
        float distance = (playerPosition - transform.position).magnitude;
        return distance;
    }


    // interface

    public void DisableAfterDie(GameObject targetObj)
    {
        targetObj.SetActive(false);
        agent.enabled = false;
    }

    public void LeftSlashAttack(GameObject targetObj)
    {
        BossShadow bossShadow = targetObj.GetComponent<BossShadow>();
        
        RaycastHit leftHandHitInfo;
        if (Physics.SphereCast(bossShadow.leftHandObject.transform.position, 
                               bossShadow.castRadius,
                               targetObj.transform.forward, 
                               out leftHandHitInfo))
        {
            //Debug.LogWarning("Hit Something!" + leftHandHitInfo.collider.name);
            GameObject hitObj = leftHandHitInfo.collider.gameObject;
            if (hitObj != null && hitObj.tag == "Player")
            {
                //Debug.LogWarning("Hit player!");
                Player.GetInstance().PlayerReceiveDamage(bossShadow.slashAttackDamage);
            }
        }
    }

    public void RightSlashAttack(GameObject targetObj)
    {
        BossShadow bossShadow = targetObj.GetComponent<BossShadow>();
        RaycastHit rightHandHitInfo;
        if (Physics.SphereCast(bossShadow.rightHandObject.transform.position, 
                               bossShadow.castRadius,
                               targetObj.transform.forward, 
                               out rightHandHitInfo))
        {
            GameObject hitObj = rightHandHitInfo.collider.gameObject;
            if (hitObj != null && hitObj.tag == "Player")
            {
                //Debug.LogWarning("Hit player!");
                Player.GetInstance().PlayerReceiveDamage(bossShadow.slashAttackDamage);
            }
        }
    }

    public void RemoveSkill2Status(GameObject targetObj)
    {
        BossShadow bossShadow = targetObj.GetComponent<BossShadow>();
        bossShadow.RemoveSkill2Status();
        bossShadow.ResetStatus(E_ShadowStatus.skill2);
    }
}