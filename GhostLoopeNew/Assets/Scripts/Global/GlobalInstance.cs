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

        // 
    }

    private void Start()
    {
       inputController.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
