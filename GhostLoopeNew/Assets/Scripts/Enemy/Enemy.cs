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


    public float FireDelay = 1.0f; // ���﷢���ӵ��ļ��
    protected float CurFireDelay = 0.0f;

    public E_PoolType EnemyBulletType; // ���﷢�����ӵ�����


    public enum EnemyState
    {
        MovingState,
        FightingState
    }

    protected EnemyState state;


    public void Start()
    {
        //Debug.Log("In Enemy Start");
        //Enemy_HP = gameObject.AddComponent<Slider>();

        if(Enemy_HP != null ) Enemy_HP.value = Enemy_HP.maxValue = 100;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        
    }


    // ÿdeltaTime�ܵ�һ���˺���ÿ���˺�ΪextraDamage�����ܵ�count���˺�
    IEnumerator ReceiveExtraDamage(float deltaTime, float extraDamage, int count)
    {
        for (int i = 0; i < count; i++)
        {
            SetEnemyHP(GetEnemyHP() - extraDamage);

            // �������ܵ�һ�ζ����˺���Ȼ��ȴ�deltaTime
            yield return new WaitForSeconds(deltaTime);
        }
    }


    public void SetEnemyHP(float hp)
    {
        HP = hp;
        Enemy_HP.value = hp;
    }
    public float GetEnemyHP()
    {
        return HP;
    }

    public void EnemyReceiveDamage(Bullet bullet)
    {
        Debug.Log("In EnemyReceiveDamage + bullet.type: " + bullet.bulletType + bullet.playerDamage);



        SetEnemyHP(GetEnemyHP() - bullet.playerDamage);



        // ����Ч�� + �����˺�
        switch (bullet.bulletType)
        {
            case E_PoolType.SimpleBullet:
                
                break;
            case E_PoolType.FireBullet:
                break;
            case E_PoolType.ThunderBullet: // �����ӵ��Ķ����˺�


                StartCoroutine(ReceiveExtraDamage(0, bullet.extraDamage, 1));


                break;
            case E_PoolType.ExplodeBullet:
                break;
            case E_PoolType.BurnBullet:

                StartCoroutine(ReceiveExtraDamage(0.5f, 5.0f, 6));

                break;
            case E_PoolType.IceBullet: // �����ļ���Ч����3�����ʧ
                //StartCoroutine(ReceiveIceEffect(3.0f));
                break;

            case E_PoolType.PoisonBullet:


                //StartCoroutine(ReceiveExtraDamage(1.0f, 3.0f, 10));


                break;
            case E_PoolType.SpiritPoisonBullet:

                //StartCoroutine(ReceiveSpiritPosionEffect(3.0f));
                break;
        }

        // �жϹ����Ƿ�����
        if (HP <= 0)
        {
            MusicManager.GetInstance().PlayFireSound("����ֱ�ը��Ч");

            GameObject.Destroy(gameObject);
            GameObject.Destroy(Enemy_HP.gameObject);
        }



    }

    






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
        Vector3 fireOrigin = transform.position + fireDirection * 20.0f;

        SpecialBullet bullet = PoolManager.GetInstance().GetObj(EnemyBulletType).GetComponent<SpecialBullet>();
        bullet.bulletType = EnemyBulletType;


        bullet.Activate();


        bullet.FireOut(fireOrigin,
                       fireDirection,
                       GlobalSetting.GetInstance().enemyBulletSpeed);
    }










    Vector3 FindRandomPosition()
    {
        Vector3 randomDir = new Vector3(Random.Range(-1, 1f), 0, Random.Range(-1, 1f));
        return transform.position + randomDir.normalized * Random.Range(2, 5);
    }
}
