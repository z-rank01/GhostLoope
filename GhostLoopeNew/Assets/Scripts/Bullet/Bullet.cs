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

    private bool isFromPlayer = false; // 该子弹是否是由玩家发出

    public ParticleSystem BulletParticleSystem; // 子弹特效

    public ParticleSystem BulletTrailParticle; // 子弹尾迹

    private bool activated = true;
    private float bulletSpeed;
    private Vector3 fireDirection;


    public void SetIsFromPlayer(bool value) { isFromPlayer =  value; }
    public bool GetIsFromPlayer()
    {
        return isFromPlayer;
    }


    void Start()
    {
        BulletTrailParticle.Play();
        
    }

    // Update is called once per frame
    void Update()
    {

        BulletParticleSystem.Play();

        //for (int i = 0; i < testParticles.Length; i++)
        //{
        //    testParticles[i].Play();
        //}


        if (Time.timeScale == 0.0f) return; //暂停子弹的运动


        if (activated) Flying();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isSwallowed == true) return; // 吞噬的子弹不需要做碰撞处理

        if (other.gameObject.CompareTag("EnvironmentObject"))
        {
            //Debug.Log("Collider Boooommm!!");
            PoolManager.GetInstance().ReturnObj(bulletType, gameObject);
        }

        // if hit enemy
        if (other.gameObject.CompareTag("Enemy") && isSwallowed == false)
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 怪物收到玩家造成的该子弹的伤害
                Debug.Log("怪物收到玩家造成的该子弹的伤害: " + playerDamage + " Bullet.IsSwallowed: " + isSwallowed);
                //enemyChasing.ReceiveDamage(playerDamage);

                enemy.EnemyReceiveDamage(this);
                PoolManager.GetInstance().ReturnObj(bulletType, this.gameObject);

                //EventCenter.GetInstance().EventTrigger<float>(E_Event.ReceiveDamage, playerDamage);
            }

        }

        // if hit tenacity
        if (other.gameObject.CompareTag("Tenacity"))
        {
            Tenacity tenacityShield = other.gameObject.GetComponent<Tenacity>();
            tenacityShield.ReceiveDamage(this);
            PoolManager.GetInstance().ReturnObj(bulletType, this.gameObject);
        }

        ////// 怪物的子弹击中玩家
        //if (other.gameObject.CompareTag("Player"))
        //{
        //    //PoolManager.GetInstance().ReturnObj(E_PoolType.SimpleBullet, gameObject);
        //    // EventCenter.GetInstance().EventTrigger<SpecialBullet>(E_Event.PlayerReceiveDamage, this);
        //}


        // if hit wall
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


        // theta为发射方向与正前方方向的夹角
        float theta = Mathf.Atan2(fireDirection.x, fireDirection.z);

        // 修改y的rotation，使其指向子弹发射方向的相反方向，形成子弹尾迹
        float rot_y = theta / Mathf.PI * 180 + 180;

        // 修改特效的y轴的旋转
        BulletTrailParticle.transform.rotation = Quaternion.Euler(0, rot_y, 0);
    }

    public void Flying()
    {
        transform.position += fireDirection * bulletSpeed;
    }


    //public bool CheckWithinScreen()
    //{
    //    Vector3 viewPortPosition = Camera.main.WorldToViewportPoint(transform.position);

    //    if (viewPortPosition.x >= 0 && viewPortPosition.x <= 1
    //        &&
    //        viewPortPosition.y >= 0 && viewPortPosition.y <= 1
    //        &&
    //        viewPortPosition.z > 0)
    //        return true;
    //    else return false;
    //}

    public void Activate()
    {
        activated = true;
    }

    public void Deactivate()
    {
        activated = false;
    }
}
