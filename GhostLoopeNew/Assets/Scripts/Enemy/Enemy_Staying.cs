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
        Enemy_HP.value = Enemy_HP.maxValue = 100;
        enemyAgent = GetComponent<NavMeshAgent>();
        state = EnemyState.MovingState;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        EventCenter.GetInstance().AddEventListener<float>(E_Event.ReceiveDamage, ReceiveDamage);
    }


    public void ReceiveDamage(float damage)
    {
        Debug.Log("InReceiveDamage");
        HP -= damage;
        Enemy_HP.value -= damage;
        if (HP <= 0)
        {
            GameObject.Destroy(gameObject);
            GameObject.Destroy(Enemy_HP.gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (player == null) return;
        Vector3 playerPosition = player.transform.position;
        float distance = (playerPosition - transform.position).magnitude;


        if (distance <= AttackDistance)
        {
            Debug.Log("Enemy_Staying Attack");
            // Attack();
        }
    }
}
