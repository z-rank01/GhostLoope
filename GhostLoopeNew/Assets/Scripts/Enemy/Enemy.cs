using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Enemy : MonoBehaviour
{
    
    public float AlertDistance = 5.0f;
    public float AttackDistance = 3.0f;
    public float HP = 100f;
    public bool IsFollowPlayer = false;
    public Player player;


    public Slider Enemy_HP;


    public float FireDelay = 1.0f; // 怪物发射子弹的间隔
    protected float CurFireDelay = 0.0f;

    public E_PoolType EnemyBulletType; // 怪物发出的子弹类型



    public enum EnemyState
    {
        MovingState,
        FightingState
    }

    protected EnemyState state;


    public void Start()
    {
        Debug.Log("In Enemy Start");
        //Enemy_HP = gameObject.AddComponent<Slider>();

        if(Enemy_HP != null ) Enemy_HP.value = Enemy_HP.maxValue = 100;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        
    }


    
    


    
    Vector3 FindRandomPosition()
    {
        Vector3 randomDir = new Vector3(Random.Range(-1, 1f), 0, Random.Range(-1, 1f));
        return transform.position + randomDir.normalized * Random.Range(2, 5);
    }
}
