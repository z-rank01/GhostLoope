using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{

    public E_PoolType bulletType;

    public float damage = 5.0f; // 怪物发出子弹的基础伤害
    public float extraDamage = 0.0f; // 怪物发出子弹的额外伤害

    public float extraDamageTime = 0.0f; // 额外伤害持续时间

    public float playerDamage = 25.0f; // 玩家发出子弹的伤害

    public float thunderRadius = 100.0f; // 连锁闪电可追踪的最大范围

    public float explodeRadius = 100.0f; // 爆炸范围

    public bool isSwallowed = false; // 该子弹是否被吞噬

    private bool activated = true;
    private float bulletSpeed;
    private Vector3 fireDirection;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Time.timeScale == 0.0f) return; //暂停子弹的运动


        if (activated) Flying();
        if (!CheckWithinScreen())
        {
            Debug.Log("OUT OF SCREEN");
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isSwallowed == true) return; // 吞噬的子弹不需要做碰撞处理

        if (other.gameObject.CompareTag("EnvironmentObject"))
        {
            Debug.Log("Collider Boooommm!!");
            PoolManager.GetInstance().ReturnObj(bulletType, gameObject);
        }

        // 玩家的子弹击中了敌人
        if (other.gameObject.CompareTag("Enemy") && isSwallowed == false)
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 怪物收到玩家造成的该子弹的伤害
                Debug.Log("怪物收到玩家造成的该子弹的伤害: " + playerDamage + " Bullet.IsSwallowed: " + isSwallowed);
                //enemyChasing.ReceiveDamage(playerDamage);

                enemy.EnemyReceiveDamage(this);
                PoolManager.GetInstance().ReturnObj(bulletType, gameObject);

                //EventCenter.GetInstance().EventTrigger<float>(E_Event.ReceiveDamage, playerDamage);
            }

        }

        ////// 怪物的子弹击中玩家
        //if (other.gameObject.CompareTag("Player"))
        //{
        //    //PoolManager.GetInstance().ReturnObj(E_PoolType.SimpleBullet, gameObject);
        //    // EventCenter.GetInstance().EventTrigger<SpecialBullet>(E_Event.PlayerReceiveDamage, this);
        //}



        if (other.gameObject.CompareTag("Boundary"))
        {
            //Debug.Log("OUT OF BOUNDARY");
            PoolManager.GetInstance().ReturnObj(bulletType, gameObject);
        }
    }


    // interface
    public void FireOut(Vector3 position, Vector3 fireDirection, float bulletSpeed)
    {
        transform.position = position;
        this.fireDirection = fireDirection;
        this.bulletSpeed = bulletSpeed;
    }

    public void Flying()
    {
        transform.position += fireDirection * bulletSpeed;
    }


    public bool CheckWithinScreen()
    {
        Vector3 viewPortPosition = Camera.main.WorldToViewportPoint(transform.position);

        if (viewPortPosition.x >= 0 && viewPortPosition.x <= 1
            &&
            viewPortPosition.y >= 0 && viewPortPosition.y <= 1
            &&
            viewPortPosition.z > 0)
            return true;
        else return false;
    }

    public void Activate()
    {
        activated = true;
    }

    public void Deactivate()
    {
        activated = false;
    }
}
