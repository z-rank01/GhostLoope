using UnityEngine;
using UnityEngine.AI;

public class Enemy_Chasing : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        state = EnemyState.MovingState;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if(enemyAgent != null ) enemyAgent.stoppingDistance = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null || enemyAgent == null) return;
        Vector3 playerPosition = player.transform.position;
        float distance = (playerPosition - transform.position).magnitude;

        //Debug.Log("distance: " + distance);
        //if(player.transform.position)



        // ½øÈë¾¯½ä·¶Î§ÄÚ£¬×·×ÙÍæ¼Ò

        if (IsFollowPlayer || distance <= AlertDistance)
        {
            IsFollowPlayer = true;
            enemyAgent.SetDestination(player.transform.position);

        }

        // ½øÈë¹¥»÷·¶Î§ÄÚ£¬Ö´ÐÐ¹¥»÷Âß¼­
        if (distance <= AttackDistance)
        {
            Debug.Log("Enemy_Chasing Attack");

            // Attack();
        }


        // Ëæ»úÒÆ¶¯
        //if (state == EnemyState.MovingState)
        //{
        //    if (enemyAgent.remainingDistance <= 0)
        //    {
        //        Debug.Log("Start Move");
        //        enemyAgent.SetDestination(FindRandomPosition());
        //    }
        //}
    }
}
