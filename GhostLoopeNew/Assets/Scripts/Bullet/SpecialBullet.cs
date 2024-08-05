using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpecialBullet : Bullet
{
    public ParticleSystem BulletParticleSystem; // ×Óµ¯ÌØÐ§


    
    

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
