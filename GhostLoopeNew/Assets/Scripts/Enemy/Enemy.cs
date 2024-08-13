using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum E_EnemyType
{
    chasingMob,
    stayingMob,
}

public class Enemy : MonoBehaviour
{
    [Header("Default Setting")]
    // enemy property
    public float alertDistance = 5.0f;
    public float attackDistance = 3.0f;
    public float bulletSpawnDistance = 5.0f;
    public float maxHp = 100f;
    
    public float fireCooldown = 1.0f; // 怪物发射子弹的间隔
    public E_PoolType enemyBulletType; // 怪物发出的子弹类型

    protected float hp;
    protected float currFireCoolDown;
    protected bool receiveDamage = false;
    protected float currReceivedDamage;



    // Animator
    protected AnimatorController animator;
    protected float moveFrame;

    // UI
    public bool isNeedRedHp = true; // 怪物是否需要红色的hp，应该小怪需要，boss不需要
    public int id; // 怪物的id，需要都不同，才能正确找到UI和存档

    // public string hpSliderName;
    public Slider enemyHp; // 小怪的生命条


    public Slider enemySan; // Boss的生命条
    public Slider enemyRes; // Boss的韧性条



    // damage receiver
    protected ThunderChainDamageReceiver thunderChainDamageReceiver;
    protected ExplodeDamageReceiver explodeDamageReceiver;
    protected ContinuousDamageReceiver continuousDamageReceiver;


    protected void OnEnable()
    {
        //Debug.Log("In Enemy Start");
        //Enemy_HP = gameObject.AddComponent<Slider>();

        // hp
        hp = maxHp;


        // UI
        if (enemyHp != null)
        {
            enemyHp.maxValue = hp; // 先设置max，再设置当前值
            enemyHp.value = enemyHp.maxValue;
        }
        else
        {
            if (GameObject.Find("Enemy_HP" + id) != null && isNeedRedHp == true)
            {
                // 小怪需要的血条
                enemyHp = GameObject.Find("Enemy_HP" + id).GetComponent<Slider>();
                enemyHp.value = enemyHp.maxValue = hp; // 先设置max，再设置当前值
                enemyHp.GetComponent<HintUI>().SetCameraAndFollowingTarget(Camera.main, transform);
            }
        }


        //GameObject hpSlider = GameObject.Find(hpSliderName);
        //if (hpSlider != null)
        //{
        //    hpSlider.SetActive(true);
        //    enemyHp = hpSlider.GetComponent<Slider>();
        //    if (enemyHp != null)
        //    {
        //        enemyHp.maxValue = hp; // 先设置max，再设置当前值
        //        enemyHp.value = enemyHp.maxValue;
        //    }
        //    if (enemyRes != null)
        //    {
        //        enemyRes.maxValue = 40;
        //        enemyRes.value = 40;
        //    }
        //    HintUI hintUI = hpSlider.GetComponent<HintUI>();
        //    if (hintUI != null)
        //        hintUI.SetCameraAndFollowingTarget(Camera.main, transform);
        //}
        //else
        //{
        //    if (enemyHp != null)
        //    {
        //        enemyHp.maxValue = hp; // 先设置max，再设置当前值
        //        enemyHp.value = enemyHp.maxValue;
        //    }
        //    if (enemyRes != null)
        //    {
        //        enemyRes.maxValue = 40;
        //        enemyRes.value = 40;
        //    }
        //}



        // Animator
        animator = gameObject.AddComponent<AnimatorController>();

        // damage receiver
        thunderChainDamageReceiver = gameObject.AddComponent<ThunderChainDamageReceiver>();
        explodeDamageReceiver = gameObject.AddComponent<ExplodeDamageReceiver>();
        continuousDamageReceiver = gameObject.AddComponent<ContinuousDamageReceiver>();




        //enemySan = GameObject.Find("Enemy_San").GetComponent<Slider>();
        //enemySan.value = enemySan.maxValue = hp;
        //enemyRes = GameObject.Find("Enemy_Res").GetComponent<Slider>();
    }

    protected void SimpleFire()
    {
        //Debug.Log("Enemy: SimpleFire");
        Vector3 playerPosition = Player.GetInstance().GetPlayerTransform().position;


        // Set fire direction
        Vector3 fireDirection = playerPosition - transform.position;
        fireDirection = Vector3.Normalize(fireDirection);
        fireDirection.y = 0;

        // Set fire origin
        Vector3 fireOrigin = transform.position + fireDirection * bulletSpawnDistance;

        SpecialBullet bullet = PoolManager.GetInstance().GetObj(enemyBulletType).GetComponent<SpecialBullet>();
        bullet.bulletType = enemyBulletType;


        bullet.Activate();
        bullet.SetIsFromPlayer(false); // 标记为怪物发出的子弹

        bullet.FireOut(fireOrigin,
                       fireDirection,
                       GlobalSetting.GetInstance().enemyBulletSpeed);
    }

    public void SetEnemyHPSlider(bool active)
    {
        enemyHp.gameObject.SetActive(active);
    }


    // interface
    public void SetEnemyHP(float hp)
    {
        this.hp = hp;
        if(enemyHp != null) enemyHp.value = hp;
        Debug.Log("In Enemy SetEnemyHp" + hp);
        if (enemySan)
        {
            Debug.Log("enemySan is not null!");
            enemySan.value = hp;
        }
            //if(enemyRes)enemyRes.value = hp;

        if (hp <= 0)
        {
            Debug.Log("In SetEnemy HP Slider:");
            Debug.Log("enemyHp: " + enemyHp);
            if (enemyHp != null && enemyHp.GetComponent<HintUI>() != null)
            {
                // 将跟随物体设为空即可，方便后面继续使用该血条
                enemyHp.GetComponent<HintUI>().offset = new Vector3(0, 1000000, 0);
                //enemyHp.gameObject.SetActive(false);
            }

            
        }
    }

    public float GetEnemyHP()
    {
        return hp;
    }

    public void EnemyReceiveDamage(Bullet bullet)
    {
        //Debug.Log("In EnemyReceiveDamage + bullet.type: " + bullet.bulletType + bullet.playerDamage);

        // update property
        receiveDamage = true;

        // 特殊子弹
        if (bullet.bulletType != E_PoolType.SimpleBullet)
        {
            // 造成不同倍率的伤害
            if (Player.GetInstance().GetSoul_2())
            {
                currReceivedDamage = bullet.playerDamage * 3;
                
            }
            else if (Player.GetInstance().GetSoul_1())
            {
                currReceivedDamage = bullet.playerDamage * 1.25f;
            }
            else
            {
                currReceivedDamage = bullet.playerDamage * 1;
            }
        }
        // 普通子弹
        else
        {
            currReceivedDamage = bullet.playerDamage;
        }

        SetEnemyHP(GetEnemyHP() - currReceivedDamage);
        currReceivedDamage = bullet.playerDamage;
        
        // 特殊效果 + 额外伤害
        switch (bullet.bulletType)
        {
            case E_PoolType.SimpleBullet:
                break;

            case E_PoolType.FireBullet:
                break;

            case E_PoolType.ThunderBullet: // 闪电子弹的额外伤害
                thunderChainDamageReceiver.ReceiveDamage(3, bullet.extraDamage, bullet.thunderRadius, this);
                //StartCoroutine(ReceiveExtraDamage(0, bullet.extraDamage, 1));
                break;

            case E_PoolType.ExplodeBullet:
                explodeDamageReceiver.ReceiveDamage(bullet);
                break;

            case E_PoolType.BurnBullet:
                continuousDamageReceiver.ReceiveDamage(0.5f, 0.5f, 3, this);
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
    }

    public void EnemyReceiveDamage(float damage)
    {
        SetEnemyHP(GetEnemyHP() - damage);
    }

    public void SetSlider(Slider sanSlider, Slider resilianceSlider)
    {
        sanSlider = GameObject.Find("Enemy_San").GetComponent<Slider>();

        sanSlider.value = sanSlider.maxValue = hp;

        resilianceSlider = GameObject.Find("Enemy_Res").GetComponent<Slider>();

        resilianceSlider.value = resilianceSlider.maxValue = 40;



        //sanSlider.gameObject.SetActive(true);
        //resilianceSlider.gameObject.SetActive(true);

        //enemyHp = sanSlider;
        //enemyRes = resilianceSlider;

        //if (enemyHp != null)
        //{
        //    enemyHp.maxValue = hp; // 先设置max，再设置当前值
        //    enemyHp.value = enemyHp.maxValue;
        //}
        //if (enemyRes != null)
        //{
        //    enemyRes.maxValue = 40;
        //    enemyRes.value = 40;
        //}
        //HintUI hintUI = enemyHp.GetComponent<HintUI>();
        //if (hintUI != null)
        //    hintUI.SetCameraAndFollowingTarget(Camera.main, transform);
    }
}
