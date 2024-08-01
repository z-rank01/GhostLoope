using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BulletType
{
    Fire,
    Thunder,
    Explode,
    Burn,
    Ice,
    Poison,
    SpiritPoison
};

public class SpecialBullet : Bullet
{
    public ParticleSystem BulletParticleSystem; // ×Óµ¯ÌØÐ§


    


    //public BulletType type = BulletType.Fire;
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
