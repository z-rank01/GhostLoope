using UnityEngine;

public class BossEggletMob : EnemyMob
{
    new protected void OnEnable()
    {
        base.OnEnable();
        // event
        // 
        EventCenter.GetInstance().EventTrigger(E_Event.BossShadeIncreaseMobOnScene);

    }

    protected new void Update()
    {
        //Debug.Log("In Enemy_Chasing Update");
        float currDistance = GetPlayerDistance();

        if (currFireCoolDown > 0) currFireCoolDown -= Time.deltaTime;

        // if enemy type is chasing, chase player
        if (enemyType == E_EnemyType.chasingMob)
        {
            // ½øÈë¾¯½ä·¶Î§ÄÚ£¬×·×ÙÍæ¼Ò
            if (currDistance <= alertDistance && enemyAgent.isOnNavMesh == true)
            {
                // Debug.Log("enemyAgent: " + enemyAgent);
                enemyAgent.SetDestination(Player.GetInstance().GetPlayerTransform().position);
                enemyAgent.speed = enemySpeed;

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

    protected override void CheckHP()
    {
        // ÅÐ¶Ï¹ÖÎïÊÇ·ñËÀÍö
        if (hp <= 0)
        {
            // MusicManager.GetInstance().PlayFireSound("±¬Õ¨ÒôÐ§");

            animator.SetTrigger("Die");
        }
    }

    private float GetPlayerDistance()
    {
        if (enemyAgent == null) Debug.LogError("Could not find AI agent!");
        Vector3 playerPosition = Player.GetInstance().GetPlayerTransform().position;
        float distance = (playerPosition - transform.position).magnitude;
        return distance;
    }

    private void OnDisable()
    {
        EventCenter.GetInstance().EventTrigger(E_Event.BossShadeDecreaseMobOnScene);
    }
}