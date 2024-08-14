using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInstance : BaseSingletonMono<GlobalInstance>
{
    public GlobalSetting globalSetting;
    GameObject globalInstance;
    
    // Singleton tool
    //PoolManager poolManager;
    //EventCenter eventCenter;
    //ResourcesManager resourcesManager;
    //ScenesManager scenesManager;


    // Main class
    InputController inputController;
    Player player;

    void Awake()
    {
        //Time.timeScale = 0.0f; //Ĭ��ֹͣ��Ϸ���������Ϸ��ʼ��Ϸ


        // global initialization
        Init();
        globalSetting.Init();
        globalInstance = new GameObject("Global Instance");

        // mono
        player = globalSetting.playerObject.GetComponent<Player>();

        // ��Ҫ�õ������л�ʱ���Զ�������������Ϳ��ԣ��Ὣ����뵽donotDestroyOnLoad��
        
        //resourcesManager = globalInstance.AddComponent<ResourcesManager>();
        //scenesManager = globalInstance.AddComponent<ScenesManager>();
        //resourcesManager.Init();
        //scenesManager.Init();

        // not mono
        inputController = new InputController();
        //eventCenter = new EventCenter();
        //poolManager = new PoolManager();
        inputController.Init();
        EventCenter.GetInstance().Init();
        PoolManager.GetInstance().Init();


        PoolManager.GetInstance().AddPool(E_PoolType.SimpleBullet, globalSetting.simpleBullet);


        // Ϊ�������������ӵ�
        PoolManager.GetInstance().AddPool(E_PoolType.FireBullet, globalSetting.FireBullet);
        PoolManager.GetInstance().AddPool(E_PoolType.ThunderBullet, globalSetting.ThunderBullet);
        PoolManager.GetInstance().AddPool(E_PoolType.ExplodeBullet, globalSetting.ExplodeBullet);
        PoolManager.GetInstance().AddPool(E_PoolType.BurnBullet, globalSetting.BurnBullet);
        PoolManager.GetInstance().AddPool(E_PoolType.IceBullet, globalSetting.IceBullet);
        PoolManager.GetInstance().AddPool(E_PoolType.PoisonBullet, globalSetting.PoisonBullet);
        PoolManager.GetInstance().AddPool(E_PoolType.SpiritPoisonBullet, globalSetting.SpiritPoisonBullet);


        // ����
        PoolManager.GetInstance().AddPool(E_PoolType.EyeBallBatBlue, globalSetting.EyeBallBatBlue);
        PoolManager.GetInstance().AddPool(E_PoolType.EyeBallBatRed, globalSetting.EyeBallBatRed);
        PoolManager.GetInstance().AddPool(E_PoolType.EyeBallBatGreen, globalSetting.EyeBallBatGreen);

        PoolManager.GetInstance().AddPool(E_PoolType.Bomb, globalSetting.Bomb);
        PoolManager.GetInstance().AddPool(E_PoolType.SnowBomb, globalSetting.SnowBomb);


        PoolManager.GetInstance().AddPool(E_PoolType.BossPoisonBomb1, globalSetting.BossPoisonBomb1);

        PoolManager.GetInstance().AddPool(E_PoolType.Egg, globalSetting.Egg);

        PoolManager.GetInstance().AddPool(E_PoolType.Bomb1, globalSetting.Bomb1);
        PoolManager.GetInstance().AddPool(E_PoolType.SnowBomb1, globalSetting.SnowBomb1);

    }

    private void Start()
    {
        //Time.timeScale = 0; // Ĭ����ͣ��Ϸ��������Ϸ������Ϊ1����������Ϸ


        inputController.Start();

        // Set up property
        //Debug.Log("Set up Player Property");
        SetProperty(E_Property.san, GlobalSetting.GetInstance().san);
        SetProperty(E_Property.resilience, 0.0f); // ���Ĭ�ϵ�����ֵΪ0


        SetProperty(E_Property.speed, GlobalSetting.GetInstance().playerSpeed);
        SetProperty(E_Property.dashSpeed, GlobalSetting.GetInstance().playerDashSpeed);
        SetProperty(E_Property.slowSpeed, GlobalSetting.GetInstance().playerSlowSpeed);

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
