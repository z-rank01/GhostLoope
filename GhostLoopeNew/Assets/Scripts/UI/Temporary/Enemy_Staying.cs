using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Staying : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        state = EnemyState.MovingState;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = player.transform.position;
        float distance = (playerPosition - transform.position).magnitude;


        if (distance <= AttackDistance)
        {
            Debug.Log("Enemy_Staying Attack");
            // Attack();
        }
    }
}
