using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Enemy_Staying : Enemy
{

    // Start is called before the first frame update
    //void Start()
    //{
    //    base.Start();
    //    Debug.Log("In Enemy_Staying Start");
        
    //    //EventCenter.GetInstance().AddEventListener<float>(E_Event.ReceiveDamage, this.ReceiveDamage);


    //}


    //void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("In Enemy_Staying OnTriggerEnter:" + other.gameObject.name);
    //    Bullet bullet = other.GetComponent<Bullet>();
    //    if (bullet != null)
    //    {
    //        Debug.Log("bullet.playerDamage: " +  bullet.playerDamage);
    //    }
    //}

    //void OnTriggerEnter(Collider other)
    //{
    //    Bullet bullet = other.GetComponent<Bullet>();

    //    if (bullet != null)
    //    {
    //        PoolManager.GetInstance().ReturnObj(bullet.bulletType, bullet.gameObject);

    //        ReceiveDamage(bullet.playerDamage);

    //    }
    //}
    public void ReceiveDamage(float damage)
    {
        
        if (Enemy_HP == null) return;


        Debug.Log("InReceiveDamage");
        HP -= damage;
        Enemy_HP.value -= damage;
        if (HP <= 0)
        {
            GameObject.Destroy(gameObject);
            GameObject.Destroy(Enemy_HP.gameObject);
        }
    }

    public void EnemyFire()
    {
        Debug.Log("EnemyFire!");


        Vector3 playerPosition = Player.GetInstance().transform.position;


        // Set fire direction
        Vector3 fireDirection = playerPosition - transform.position;
        fireDirection = Vector3.Normalize(fireDirection);
        fireDirection.y = 0;

        // Set fire origin
        Vector3 fireOrigin = transform.position + fireDirection * 2.0f;

        SpecialBullet bullet = PoolManager.GetInstance().GetObj(EnemyBulletType).GetComponent<SpecialBullet>();
        bullet.bulletType = EnemyBulletType;

        bullet.FireOut(fireOrigin,
                       fireDirection,
                       GlobalSetting.GetInstance().enemyBulletSpeed);
    }


    // Update is called once per frame
    void Update()
    {
        if (player == null) return;
        Vector3 playerPosition = player.transform.position;
        float distance = (playerPosition - transform.position).magnitude;


        if (CurFireDelay > 0) CurFireDelay -= Time.deltaTime;


        if (distance <= AttackDistance)
        {
            Debug.Log("Enemy_Staying Attack");

            
            if (CurFireDelay <= 0)
            {
                EnemyFire();
                CurFireDelay = FireDelay;
            }
        }
    }
}
