using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBullet : MonoBehaviour
{
    public ParticleSystem BulletParticleSystem; // 子弹特效
    public float damage = 5.0f; // 怪物发出子弹的基础伤害
    public float extraDamage = 0.0f; // 怪物发出子弹的额外伤害

    public float playerDamage = 5.0f; // 玩家发出子弹的伤害
    
    public enum BulletType
    {
        Normal,
        Fire,
        Thunder,
        Explode,
        Burn,
        Ice,
        Poison,
        SpiritPoison
    };

    public BulletType type = BulletType.Normal;
    public void PlayParticleSystem()
    {
        BulletParticleSystem.Play();
    }
    public void ExtraEffect()
    {
        //switch (type)
        //{
        //    case BulletType.Fire:
        //        break;
        //}
    }
}
