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
    public float hp = 100f;
    public float fireCooldown = 1.0f; // 怪物发射子弹的间隔
    public E_PoolType enemyBulletType; // 怪物发出的子弹类型

    protected float currFireCoolDown;
    protected bool receiveDamage = false;
    protected float currReceivedDamage;

    // Animator
    protected AnimatorController animator;
    protected float moveFrame;

    // UI
    public Slider enemyHp;

    // damage receiver
    protected ThunderChainDamageReceiver thunderChainDamageReceiver;
    protected ExplodeDamageReceiver explodeDamageReceiver;
    protected ContinuousDamageReceiver continuousDamageReceiver;


    protected void OnEnable()
    {
        //Debug.Log("In Enemy Start");
        //Enemy_HP = gameObject.AddComponent<Slider>();

        // UI
        if(enemyHp != null ) enemyHp.value = enemyHp.maxValue = 100;
        
        // Animator
        animator = gameObject.AddComponent<AnimatorController>();

        // damage receiver
        thunderChainDamageReceiver = gameObject.AddComponent<ThunderChainDamageReceiver>();
        explodeDamageReceiver = gameObject.AddComponent<ExplodeDamageReceiver>();
        continuousDamageReceiver = gameObject.AddComponent<ContinuousDamageReceiver>();
    }

    protected void SimpleFire()
    {
        Debug.Log("Enemy: SimpleFire");
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
        enemyHp.value = hp;

        if (hp <= 0)
        {
            enemyHp.gameObject.SetActive(false);
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
        SetEnemyHP(GetEnemyHP() - bullet.playerDamage);
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

}
