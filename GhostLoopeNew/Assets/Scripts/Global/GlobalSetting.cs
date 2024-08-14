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
    public float san;               // 玩家最大生命值
    public float resilience;        // 玩家最大韧性值


    public float playerSpeed;
    public float playerDashSpeed;

    public float playerSlowSpeed;   // 玩家受到减速效果的移动速度



    [Header("Enemy Setting")]


    public GameObject EyeBallBatRed; // 动态生成怪物
    public GameObject EyeBallBatGreen; // 动态生成怪物
    public GameObject EyeBallBatBlue; // 动态生成怪物
    
    public GameObject Bomb; // 动态生成怪物
    public GameObject SnowBomb; // 动态生成怪物
    public GameObject BossPoisonBomb; //第二关BOSS
    public GameObject BossPoisonBomb1; //第二关BOSS
    public GameObject Egg;


    public GameObject Bomb1; // 动态生成怪物
    public GameObject SnowBomb1; // 动态生成怪物

}