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



        // ���뾯�䷶Χ�ڣ�׷�����

        if (IsFollowPlayer || distance <= AlertDistance)
        {
            IsFollowPlayer = true;
            enemyAgent.SetDestination(player.transform.position);

        }

        // ���빥����Χ�ڣ�ִ�й����߼�
        if (distance <= AttackDistance)
        {
            Debug.Log("Enemy_Chasing Attack");

            // Attack();
        }


        // ����ƶ�
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
