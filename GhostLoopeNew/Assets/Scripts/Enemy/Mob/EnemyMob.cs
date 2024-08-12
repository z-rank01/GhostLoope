using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;



public class EnemyMob : Enemy
{
    [Header("Mob Setting")]
    public E_EnemyType enemyType;
    public float enemySpeed = 1.0f;
    public NavMeshAgent enemyAgent;


    protected new void OnEnable()
    {
        //Debug.Log("In ChasingStart");
        base.OnEnable();

        enemyAgent = gameObject.AddComponent<NavMeshAgent>();
        enemyAgent.stoppingDistance = 2.0f;

        AddDieAnimationEvent();
        //EventCenter.GetInstance().AddEventListener<float>(E_Event.ReceiveDamage, this.ReceiveDamage);
        //ChasingHP = Instantiate();
    }


    // Update is called once per frame
    protected void Update()
    {
        //Debug.Log("In Enemy_Chasing Update");
        float currDistance = GetPlayerDistance();

        if (currFireCoolDown > 0) currFireCoolDown -= Time.deltaTime;

        // if enemy type is chasing, chase player
        if (enemyType == E_EnemyType.chasingMob)
        {
            // ½øÈë¾¯½ä·¶Î§ÄÚ£¬×·×ÙÍæ¼Ò
            if (currDistance <= alertDistance)
            {
                enemyAgent.speed = enemySpeed;

                Debug.Log("enemyAgent: " + enemyAgent);

                // event
                enemyAgent.SetDestination(Player.GetInstance().GetPlayerTransform().position);

                // animate
                moveFrame += Time.deltaTime;
                moveFrame = moveFrame > 1 ? 1 : moveFrame;
                animator.SetFloat("Move", moveFrame);
            }
            else
            {
                // animate
                moveFrame -= Time.deltaTime;
                moveFrame = moveFrame < 0 ? 0 : moveFrame;
                animator.SetFloat("Move", moveFrame);
            }
        }
        

        // ½øÈë¹¥»÷·¶Î§ÄÚ£¬Ö´ÐÐ¹¥»÷Âß¼­
        if (currDistance <= attackDistance)
        {
            if (currFireCoolDown <= 0)
            {
                animator.SetBool("Attack", true);
                SimpleFire();
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
            receiveDamage = false;
        }
        else
            animator.SetBool("TakeDamage", false);

        CheckHP();

    }


    // private function
    private float GetPlayerDistance()
    {
        if (enemyAgent == null) Debug.LogError("Could not find AI agent!");
        Vector3 playerPosition = Player.GetInstance().GetPlayerTransform().position;
        float distance = (playerPosition - transform.position).magnitude;
        return distance;
    }

    private void RandomMoving()
    {
        //Ëæ»úÒÆ¶¯
        if (enemyAgent.remainingDistance <= 0)
        {
            enemyAgent.SetDestination(FindRandomPosition());
        }
    }

    private Vector3 FindRandomPosition()
    {
        Vector3 randomDir = new Vector3(Random.Range(-1, 1f), 0, Random.Range(-1, 1f));
        return transform.position + randomDir.normalized * Random.Range(2, 5);
    }

    protected virtual void CheckHP()
    {
        // ÅÐ¶Ï¹ÖÎïÊÇ·ñËÀÍö
        if (hp <= 0)
        {
            MusicManager.GetInstance().PlayFireSound("òùòð¹Ö±¬Õ¨ÒôÐ§");

            // animation
            
            animator.SetTrigger("Die");
        }
    }

    protected virtual void AddDieAnimationEvent()
    {
        // event: disable object
        AnimationEvent dieEvent = new AnimationEvent();
        dieEvent.functionName = "DisableAfterDie";
        dieEvent.time = animator.GetClipLength("Die");
        dieEvent.objectReferenceParameter = this.gameObject;

        // add event
        animator.AddEvent("Die", dieEvent);
    }


    // interface
    public void DisableAfterDie(GameObject targetObj)
    {
        if (targetObj.GetComponent<Enemy>().GetEnemyHP() <= 0) 
            targetObj.SetActive(false);
    }
}
