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
    protected bool isAlreadyChained = false; // 当前怪物是否已经收到了连锁闪电伤害

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


    // 每deltaTime受到一次伤害，每次伤害为extraDamage，共受到count次伤害
    IEnumerator ReceiveExtraDamage(float deltaTime, float extraDamage, int count)
    {
        for (int i = 0; i < count; i++)
        {
            SetEnemyHP(GetEnemyHP() - extraDamage);

            // 先立即受到一次额外伤害，然后等待deltaTime
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

    // 闪电最多还可连锁到count个敌人
    void ReceiveThunderChainDamage(int count,float damage, float thunderRadius)
    {
        if (count <= 0) return;

        isAlreadyChained = true; // 打上标记，防止重复受到伤害
        SetEnemyHP(GetEnemyHP() - damage);


        GameObject[] EnemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        float minDistance = 100000000.0f;
        int id = -1;
        for (int i = 0; i < EnemyArray.Length; i++)
        {
            if (EnemyArray[i] == null || EnemyArray[i].GetComponent<Enemy>().isAlreadyChained) continue;


            Debug.Log("EnemyArray[i]: " + EnemyArray[i].name);
            float dis = (EnemyArray[i].transform.position - transform.position).magnitude;
            Debug.Log("distance: " + dis);

            // 找到距离当前怪物最近的一个怪物
            if (dis <= thunderRadius && dis < minDistance)
            {
                minDistance = dis;
                id = i;
            }
        }

        if (id != -1)
        {
            Debug.Log("Next Thunder Enemy:  " + EnemyArray[id].name);
            EnemyArray[id].GetComponent<Enemy>().ReceiveThunderChainDamage(count-1, damage, thunderRadius);
        }
        isAlreadyChained = false; // 取消标记
        
    }

    // explodeDamage: 爆炸伤害 explodeRadius: 爆炸半径
    public void ReceiveExplodeDamage(float explodeDamage, float explodeRadius)
    {
        GameObject[] EnemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < EnemyArray.Length; i++)
        {
            if (EnemyArray[i] == null || EnemyArray[i].GetComponent<Enemy>() == this) continue;

            float dis = (EnemyArray[i].transform.position - transform.position).magnitude;

            // 在爆炸范围内的怪物都会受到伤害
            if (dis <= explodeRadius)
            {
                Enemy enemy = EnemyArray[i].GetComponent<Enemy>();
                enemy.SetEnemyHP(enemy.GetEnemyHP() - explodeDamage);

            }
        }

    }



    public void EnemyReceiveDamage(Bullet bullet)
    {
        Debug.Log("In EnemyReceiveDamage + bullet.type: " + bullet.bulletType + bullet.playerDamage);



        SetEnemyHP(GetEnemyHP() - bullet.playerDamage);

        

        // 特殊效果 + 额外伤害
        switch (bullet.bulletType)
        {
            case E_PoolType.SimpleBullet:
                
                break;
            case E_PoolType.FireBullet:
                break;
            case E_PoolType.ThunderBullet: // 闪电子弹的额外伤害

                ReceiveThunderChainDamage(3, bullet.extraDamage, bullet.thunderRadius);
                //StartCoroutine(ReceiveExtraDamage(0, bullet.extraDamage, 1));

                

                break;
            case E_PoolType.ExplodeBullet:

                // 爆炸子弹的AOE伤害，在爆炸范围内的怪物都会受到伤害
                ReceiveExplodeDamage(bullet.playerDamage, bullet.explodeRadius);

                break;
            case E_PoolType.BurnBullet:

                StartCoroutine(ReceiveExtraDamage(0.5f, 5.0f, 6));

                break;
            case E_PoolType.IceBullet: // 冰弹的减速效果，3秒后消失
                //StartCoroutine(ReceiveIceEffect(3.0f));
                break;

            case E_PoolType.PoisonBullet:


                //StartCoroutine(ReceiveExtraDamage(1.0f, 3.0f, 10));


                break;
            case E_PoolType.SpiritPoisonBullet:

                //StartCoroutine(ReceiveSpiritPosionEffect(3.0f));
                break;
        }

        // 判断怪物是否死亡
        if (HP <= 0)
        {
            MusicManager.GetInstance().PlayFireSound("蝙蝠怪爆炸音效");

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
        Vector3 fireOrigin = transform.position + fireDirection * 10.0f;

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
