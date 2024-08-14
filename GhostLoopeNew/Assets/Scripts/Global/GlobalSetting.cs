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
    public float san;               // ����������ֵ
    public float resilience;        // ����������ֵ


    public float playerSpeed;
    public float playerDashSpeed;

    public float playerSlowSpeed;   // ����ܵ�����Ч�����ƶ��ٶ�



    [Header("Enemy Setting")]


    public GameObject EyeBallBatRed; // ��̬���ɹ���
    public GameObject EyeBallBatGreen; // ��̬���ɹ���
    public GameObject EyeBallBatBlue; // ��̬���ɹ���
    
    public GameObject Bomb; // ��̬���ɹ���
    public GameObject SnowBomb; // ��̬���ɹ���
    public GameObject BossPoisonBomb; //�ڶ���BOSS
    public GameObject BossPoisonBomb1; //�ڶ���BOSS
    public GameObject Egg;


    public GameObject Bomb1; // ��̬���ɹ���
    public GameObject SnowBomb1; // ��̬���ɹ���

}