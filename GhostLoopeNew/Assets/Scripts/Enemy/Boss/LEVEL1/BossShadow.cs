using BehaviorDesigner.Runtime.Tasks.Unity.UnityPhysics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.Rendering;


public enum E_ShadowStatus
{
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
    public NavMeshAgent normalAgent;
    public float skillCoolDown = 5.0f;
    
    private E_ShadowStatus shadowStatus;
    private float currSkillTime;

    [Header("Skill 2 Setting")]
    public GameObject leftHandObject;
    public GameObject rightHandObject;
    public GameObject skill2DestinationObject;
    public float castRadius = 3.0f;
    public int castTimes = 4;
    public float slashAttackDamage = 10.0f;
    public float skill2DashingTime = 5.0f;

    private float currDashingTime;
    private bool reachTarget = false;
    private bool hasSetTargetPosition = false;
    private Vector3 targetPosition;
    private Vector3 targetDirection;

    [Header("Skill 3 Setting")]
    public GameObject hintRangePrefab;
    public float rangeAttackRadius = 5.0f;
    public float rangeAttackDamage = 40.0f;
    public int preAttackSeconds = 3;

    private GameObject hintRangeObject;

    [Header("Tenacity")]
    public GameObject tenacityObj;
    private Tenacity tenacity;

    protected new void Start()
    {
        //Debug.Log("In ChasingStart");
        base.Start();

        // ai
        normalAgent = gameObject.AddComponent<NavMeshAgent>();
        normalAgent.stoppingDistance = 3.0f;
        normalAgent.speed = enemyWalkSpeed;

        // animation event
        AddSlashAttackEvent();

        // cooldown
        currSkillTime = skillCoolDown;
        currDashingTime = skill2DashingTime;

        // status
        shadowStatus = E_ShadowStatus.normal;

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
        if (currFireCoolDown > 0) currFireCoolDown -= Time.deltaTime;
        if (currSkillTime > 0) currSkillTime -= Time.deltaTime;

        float currDistance = GetPlayerDistance();

        // normal status
        if (shadowStatus == E_ShadowStatus.normal)
        {
            RestSkill2Status();

            // chasing
            if (normalAgent.enabled != false && currDistance > normalAgent.stoppingDistance)
            {
                normalAgent.SetDestination(Player.GetInstance().GetPlayerTransform().position);
                normalAgent.speed = enemyWalkSpeed;

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
                    //SimpleFire();
                    currFireCoolDown = fireDelay;
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
        if (shadowStatus == E_ShadowStatus.skill2)
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
        if (shadowStatus == E_ShadowStatus.skill3)
        {
            normalAgent.enabled = false;
            RangeAttack();
        }


        // tenacity
        if (shadowStatus == E_ShadowStatus.broken)
        {
            tenacityObj.SetActive(true);
            normalAgent.enabled = false;
            if (!tenacity.CheckBulletOnScene())
                shadowStatus = E_ShadowStatus.normal;
        }

        SwitchToSkill(currDistance);
        SwitchToBroken();
        CheckHP();
    }

    private void SwitchToSkill(float currDistance)
    {
        // skill
        if (currSkillTime <= 0)
        {
            if (currDistance <= rangeAttackRadius)
            {
                ResetNormalStatus();
                // skill3
                shadowStatus = E_ShadowStatus.skill3;
            }
            else
            {
                ResetNormalStatus();
                // skill2
                shadowStatus = E_ShadowStatus.skill2;
            }

            currSkillTime = skillCoolDown;
        }
    }

    private void SwitchToBroken()
    {
        if (tenacity.CheckTenacityEqualZero())
            shadowStatus = E_ShadowStatus.broken;
    }


    // skill 2
    private void SetTarget()
    {
        // instantiate target object
        targetPosition = Player.GetInstance().transform.position;
        skill2DestinationObject = Instantiate(skill2DestinationObject, targetPosition, Quaternion.identity);
        
        // set agent
        normalAgent.SetDestination(skill2DestinationObject.transform.position);
        normalAgent.speed = enemyRunSpeed;
        
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
        if (distance > normalAgent.stoppingDistance)
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
    IEnumerator PreAttack()
    {
        yield return new WaitForSeconds(preAttackSeconds);

        // finish animation
        animator.SetBool("Attack", false);

        // exert effect
        if (GetPlayerDistance() < rangeAttackRadius) 
            Player.GetInstance().PlayerReceiveDamage(rangeAttackDamage);

        // destroy object
        DestroyImmediate(hintRangeObject);
        hintRangeObject = null;

        // switch to normal
        shadowStatus = E_ShadowStatus.normal;

        // restart agent
        normalAgent.enabled = true;
    }

    private void RangeAttack()
    {
        // start animation
        animator.SetBool("Attack", true);

        // geenrate hint
        if (hintRangeObject == null)
        {
            hintRangeObject = Instantiate(hintRangePrefab, this.transform);
            hintRangeObject.transform.localPosition = Vector3.zero;
        }

        StartCoroutine(PreAttack());
    }


    // general function
    private void CheckHP()
    {
        // ÅÐ¶Ï¹ÖÎïÊÇ·ñËÀÍö
        if (hp <= 0)
        {
            MusicManager.GetInstance().PlayFireSound("òùòð¹Ö±¬Õ¨ÒôÐ§");
            normalAgent.enabled = false;

            // animation
            AddDieAnimationEvent();
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
        attackFinishEvent.functionName = "SwitchToNormal";
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

    private void ResetNormalStatus()
    {
        moveFrame = 0;

        animator.SetBool("Attack", false);
        animator.SetBool("TakeDamage", false);
        animator.SetFloat("Move", moveFrame);

        receiveDamage = false;
    }

    private void RestSkill2Status()
    {
        animator.SetBool("SlashAttack", false);

        hasSetTargetPosition = false;
        reachTarget = false;
    }

    private void OnDrawGizmos()
    {
        // for debugging 
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(leftHandObject.transform.position,
                          //castRadius);
        //Gizmos.DrawLine(leftHandObject.transform.position, transform.forward);

        //Gizmos.DrawWireSphere(rightHandObject.transform.position,
                          //castRadius);
        //Gizmos.DrawLine(rightHandObject.transform.position, transform.forward);
    }


    // interface

    public void DisableAfterDie(GameObject targetObj)
    {
        targetObj.SetActive(false);
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
            if (hitObj.tag == "Player")
            {
                //Debug.LogWarning("Hit player!");
                hitObj.GetComponent<Player>().PlayerReceiveDamage(bossShadow.slashAttackDamage);
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
            if (hitObj.tag == "Player")
            {
                //Debug.LogWarning("Hit player!");
                hitObj.GetComponent<Player>().PlayerReceiveDamage(bossShadow.slashAttackDamage);
            }
        }
    }

    public void SwitchToNormal(GameObject targetObj)
    {
        BossShadow bossShadow = targetObj?.GetComponent<BossShadow>();
        bossShadow.SetNormalStatus();
    }

    public void SetNormalStatus()
    {
        shadowStatus = E_ShadowStatus.normal;
    }

    public void SetSkill2Status()
    {
        shadowStatus = E_ShadowStatus.skill2;
    }

    public void SetSkill3Status()
    {
        shadowStatus = E_ShadowStatus.skill3;
    }
}