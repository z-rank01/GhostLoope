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

    public float playerDamage = 5.0f; // 玩家发出子弹的伤害

    
    

    // Start is called before the first frame update
    private float bulletSpeed;
    private Vector3 fireDirection;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Flying();
        if (!CheckWithinScreen())
        {
            Debug.Log("OUT OF SCREEN");
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnvironmentObject"))
        {
            Debug.Log("Collider Boooommm!!");
            PoolManager.GetInstance().ReturnObj(bulletType, gameObject);
        }

        // 玩家的子弹击中了敌人
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy_Staying enemyStaying = other.gameObject.GetComponent<Enemy_Staying>();
            Enemy_Chasing enemyChasing = other.gameObject.GetComponent<Enemy_Chasing>();
            if (enemyStaying != null)
            {
                // 怪物收到玩家造成的该子弹的伤害
                Debug.Log("playerDamage: " + playerDamage);
                enemyStaying.ReceiveDamage(playerDamage);


                PoolManager.GetInstance().ReturnObj(bulletType, gameObject);

                //EventCenter.GetInstance().EventTrigger<float>(E_Event.ReceiveDamage, playerDamage);
            }
            if (enemyChasing != null)
            {
                // 怪物收到玩家造成的该子弹的伤害
                Debug.Log("playerDamage: " + playerDamage);
                enemyChasing.ReceiveDamage(playerDamage);

                PoolManager.GetInstance().ReturnObj(bulletType, gameObject);

                //EventCenter.GetInstance().EventTrigger<float>(E_Event.ReceiveDamage, playerDamage);
            }
            
        }

        //// 怪物的子弹击中玩家
        if (other.gameObject.CompareTag("Player"))
        {
            //PoolManager.GetInstance().ReturnObj(E_PoolType.SimpleBullet, gameObject);
            // EventCenter.GetInstance().EventTrigger<SpecialBullet>(E_Event.PlayerReceiveDamage, this);
        }



        if (other.gameObject.CompareTag("Boundary"))
        {
            Debug.Log("OUT OF BOUNDARY");
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
}
