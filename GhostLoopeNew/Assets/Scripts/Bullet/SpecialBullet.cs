using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpecialBullet : Bullet
{
    public ParticleSystem BulletParticleSystem; // �ӵ���Ч


    
    

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
