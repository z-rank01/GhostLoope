using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInstance : BaseSingletonMono<GlobalInstance>
{
    public GlobalSetting globalSetting;
    GameObject globalInstance;
    
    // Singleton tool
    PoolManager poolManager;
    EventCenter eventCenter;
    ResourcesManager resourcesManager;
    ScenesManager scenesManager;


    // Main class
    InputController inputController;
    Player player;

    void Awake()
    {
        // global initialization
        Init();
        globalSetting.Init();
        globalInstance = new GameObject("Global Instance");

        // mono
        player = globalSetting.playerObject.GetComponent<Player>();
        resourcesManager = globalInstance.AddComponent<ResourcesManager>();
        scenesManager = globalInstance.AddComponent<ScenesManager>();
        resourcesManager.Init();
        scenesManager.Init();

        // not mono
        inputController = new InputController();
        eventCenter = new EventCenter();
        poolManager = new PoolManager();
        inputController.Init();
        eventCenter.Init();
        poolManager.Init();
        poolManager.AddPool(E_PoolType.SimpleBullet, globalSetting.simpleBullet);


        // 为对象池添加特殊子弹
        poolManager.AddPool(E_PoolType.FireBullet, globalSetting.FireBullet);
        poolManager.AddPool(E_PoolType.ThunderBullet, globalSetting.ThunderBullet);
        poolManager.AddPool(E_PoolType.ExplodeBullet, globalSetting.ExplodeBullet);
        poolManager.AddPool(E_PoolType.BurnBullet, globalSetting.BurnBullet);
        poolManager.AddPool(E_PoolType.IceBullet, globalSetting.IceBullet);
        poolManager.AddPool(E_PoolType.PoisonBullet, globalSetting.PoisonBullet);
        poolManager.AddPool(E_PoolType.SpiritPoisonBullet, globalSetting.SpiritPoisonBullet);

        
    }

    private void Start()
    {
        inputController.Start();

        // Set up property
        //Debug.Log("Set up Player Property");
        SetProperty(E_Property.san, GlobalSetting.GetInstance().san);
        SetProperty(E_Property.resilience, GlobalSetting.GetInstance().resilience);
        SetProperty(E_Property.speed, GlobalSetting.GetInstance().playerSpeed);
        SetProperty(E_Property.dashSpeed, GlobalSetting.GetInstance().playerDashSpeed);
    }

    void Update()
    {
        inputController.Update();
    }

    // interface

    public void AddStatus(E_InputStatus inputStatus)
    {
        player.AddStatus(inputStatus);
    }

    public void RemoveStatus(E_InputStatus inputStatus)
    {
        player.ClearStatus(inputStatus);
    }

    public void SetProperty(E_Property eProperty, object property)
    {
        player.SetProperty(eProperty, property);
    }
}
