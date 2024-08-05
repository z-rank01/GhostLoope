using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;


public class GlobalSetting : BaseSingletonMono<GlobalSetting>
{
    [Header("Bullet Setting")]
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

    [Header("Player Setting")]
    // Player GameObject
    public GameObject playerObject;
    public float san;
    public float resilience;
    public float playerSpeed;
    public float playerDashSpeed;

    public float playerSlowSpeed; // 玩家受到减速效果的移动速度
}