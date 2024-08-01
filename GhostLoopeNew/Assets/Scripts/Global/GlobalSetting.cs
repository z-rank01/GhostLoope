using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;


public class GlobalSetting : BaseSingletonMono<GlobalSetting>
{
    // Prefab for pool
    public GameObject simpleBullet;


    public GameObject FireBullet;
    public GameObject ThunderBullet;
    public GameObject ExplodeBullet;
    public GameObject BurnBullet;
    public GameObject IceBullet;
    public GameObject PoisonBullet;
    public GameObject SpiritPoisonBullet;




    public float bulletSpeed;

    public float specialBulletSpeed;

    public float enemyBulletSpeed;

    // public GameObject SpecialBullet;
    // public GameObject Enemy;

    // Player GameObject
    public GameObject playerObject;
    public float san;
    public float resilience;
    public float playerSpeed;
    public float playerDashSpeed;
}