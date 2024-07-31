using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;


public class GlobalSetting : BaseSingletonMono<GlobalSetting>
{
    [Header("Bullet Setting")]
    // Prefab for pool
    public GameObject simpleBullet;
    public float bulletSpeed;
    // public GameObject SpecialBullet;
    // public GameObject Enemy;

    [Header("Player Setting")]
    // Player GameObject
    public GameObject playerObject;
    public float san;
    public float resilience;
    public float playerSpeed;
    public float playerDashSpeed;
}