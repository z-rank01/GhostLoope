using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Enemy : MonoBehaviour
{
    protected NavMeshAgent enemyAgent;
    public float AlertDistance = 5.0f;
    public float AttackDistance = 3.0f;
    public float HP = 100f;
    public bool IsFollowPlayer = false;
    public Player player;


    public Slider Enemy_HP;

    
    public enum EnemyState
    {
        MovingState,
        FightingState
    }

    protected EnemyState state;

    // Start is called before the first frame update
    //void Start()
    //{
    //    enemyAgent = GetComponent<NavMeshAgent>();
    //    state = EnemyState.MovingState;
    //    player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

    //    //Collider collider= GetComponent<Collider>();
    //    //collider.enabled = true;
    //    //collider.isTrigger = false;
    //}

    
    


    void OnTriggerEnter(Collider other)
    {
        Debug.Log("In OnTriggerEnter Enemy");
    }
    Vector3 FindRandomPosition()
    {
        Vector3 randomDir = new Vector3(Random.Range(-1, 1f), 0, Random.Range(-1, 1f));
        return transform.position + randomDir.normalized * Random.Range(2, 5);
    }
}
