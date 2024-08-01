using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBullet : MonoBehaviour
{
    public ParticleSystem BulletParticleSystem; // �ӵ���Ч
    public float damage = 5.0f; // ���﷢���ӵ��Ļ����˺�
    public float extraDamage = 0.0f; // ���﷢���ӵ��Ķ����˺�

    public float playerDamage = 5.0f; // ��ҷ����ӵ����˺�
    
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
