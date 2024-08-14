using BehaviorDesigner.Runtime.Tasks.Unity.UnityPlayerPrefs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : BaseSingletonMono<Player>
{
    PlayerProperty playerProperty;
    PlayerStatus playerStatus;
    PlayerController playerController;
    PlayerAnimator playerAnimator;

    [SerializeField]
    private float gunHeat;  // 普通射击冷却时间
    private float currGunHeat = 0;  //  触发下一次攻击的冷却时间，如果 <= 0则可以射击

    [SerializeField]
    private float interactTime;  // 交互冷却时间
    private float currinteractTime = 0;  //  触发下一次交互的冷却时间，如果 <= 0则可以交互

    [SerializeField]
    private float dashTime = 1f; // 冲刺的冷却时间
    private float currDashTime = 0f; // 触发下一次冲刺的冷却时间，如果 <= 0则可以冲刺

    [SerializeField]
    private float dashDecreaseRes = 10.0f; // 冲刺需要消耗的Res值

    [SerializeField]
    private float swallowTime = 1.0f; //吞噬技能的冷却时间
    private float curSwallowTime = 0.0f; // 触发下一次吞噬的冷却时间，如果 <= 0则可以吞噬

    [SerializeField]
    private float swallowRadius = 2.0f; // 玩家吞噬技能的半径

    [SerializeField]
    private float swallowDecreaseSan = 10.0f; // 吞噬成功消耗的San值

    [SerializeField]
    private float swallowIncreaseRes = 10.0f; // 吞噬成功增加的Res值


    [SerializeField]
    private float HealingTime; // 经过HealingTime时长没有收到伤害，主角便会回血

    [SerializeField]
    private float RecoverRatio; // 玩家回血的速率，每秒恢复recoverRatio点生命值


    public float GetSwallowDecreaseSan() { return swallowDecreaseSan; }
    public float GetSwallowIncreaseRes() { return swallowIncreaseRes; }

    public float GetSwallowRadius() { return swallowRadius; }

    private bool isGettingHurt = false; // 玩家是否正受到伤害
    private bool isFightingBoss = false; // 玩家是否正在Boss战

    private bool isGettingSoul_1 = false; // 是否得到了第一关Boss的灵魂
    private bool isGettingSoul_2 = false; // 是否得到了第二关Boss的灵魂


    private bool isGameEnd = false; // 游戏是否结束

    public bool isNeedToShowText = false; // 是否需要显示增益文本
    public string showText; // 所显示的增益文本
    public void SetIsGameEnd(bool value) { isGameEnd = value; }
    public bool GetIsGameEnd() { return isGameEnd; }
    public void SetSoul_1(bool value) { isGettingSoul_1 = value; }
    public void SetSoul_2(bool value) { isGettingSoul_2 = value; }
    public bool GetSoul_1() { return isGettingSoul_1; }
    public bool GetSoul_2() { return isGettingSoul_2; }

    public NavMeshSurface surface;
    public void SetIsFightingBoss(bool value) 
    { 
        isFightingBoss = value; 
        

        if (isFightingBoss)
        {
            MusicManager.GetInstance().PlayFireSound("Boss战音乐");
        }
    }
    public bool GetIsFightingBoss() { return isFightingBoss; }
    
    // 玩家只要受到伤害，就将isGettingHurt置为true，开启时长为HealingTime的定时器，定时器结束后将isGettingHurt置为false
    public IEnumerator GettingHurt()
    {
        isGettingHurt = true;
        //Debug.Log("GettingHurt Begin " + HealingTime + " " + isGettingHurt);
        // 先立即受到一次额外伤害，然后等待deltaTime
        yield return new WaitForSeconds(HealingTime);

        //Debug.Log("GettingHurt End");
        isGettingHurt = false;
    }



    public void SetTransformPosition(Vector3 position)
    {
        transform.position = position;
    }
    public void Awake()
    {

        


        Init();
        // Mono
        playerProperty = gameObject.AddComponent<PlayerProperty>();
        playerController = gameObject.AddComponent<PlayerController>();
        playerAnimator = gameObject.AddComponent<PlayerAnimator>();

        // not Mono
        playerStatus = new PlayerStatus();
        playerStatus.Init();


        // 击中玩家后的响应事件
        EventCenter.GetInstance().AddEventListener<SpecialBullet>(E_Event.PlayerReceiveDamage, PlayerReceiveDamage);



        // 还原场景
        if (SaveManager.GetInstance().loading)
        {
            LoadSceneInfo();
            SaveManager.GetInstance().loading = false;
        }


        Debug.Log("LoadSceneInfo: " + SaveManager.GetInstance().san);
        GlobalSetting.GetInstance().san = SaveManager.GetInstance().san;
        GlobalSetting.GetInstance().resilience = SaveManager.GetInstance().res;

        SetProperty(E_Property.san, GlobalSetting.GetInstance().san);
        SetProperty(E_Property.resilience, 0.0f); // 玩家默认的韧性值为0

        isGettingSoul_1 = SaveManager.GetInstance().is_Soul1;
        isGettingSoul_2 = SaveManager.GetInstance().is_Soul2;


        string levelName = SceneManager.GetActiveScene().name;

        if (levelName == "Level1")
        {
            MusicManager.GetInstance().PlayBackgroundMusic("第一关-配乐");
            MusicManager.GetInstance().PlayEnvironmentSound("第一关-环境音-微风");
        }
        else if (levelName == "Level2")
        {
            MusicManager.GetInstance().PlayBackgroundMusic("第二关-配乐");
            MusicManager.GetInstance().PlayEnvironmentSound("第二关-环境音-小雨");
        }
        else if (levelName == "Level3")
        {
            MusicManager.GetInstance().PlayBackgroundMusic("第三关-配乐");
            MusicManager.GetInstance().PlayEnvironmentSound("第三关-环境音-森林");
        }

    }

    public void LoadSceneInfo()
    {
        // position
        transform.position = new Vector3(SaveManager.GetInstance().x, SaveManager.GetInstance().y, SaveManager.GetInstance().z);


        

        Debug.Log("Player Loading");
        Debug.Log(SaveManager.GetInstance().GraveStoneId.Count);
        // gravestone
        GameObject[] currentGraveStone = GameObject.FindGameObjectsWithTag("GraveStone");
        for (int i = 0; i < currentGraveStone.Length; i++)
        {
            Debug.Log("StoneID: " + currentGraveStone[i].GetInstanceID());
            if (!SaveManager.GetInstance().GraveStoneId.Contains(currentGraveStone[i].GetComponent<GraveStone>().id))
            {
                Destroy(currentGraveStone[i]);
            }
        }

        // spawnEnemyPoint
        GameObject[] currentSpawnEnemy = GameObject.FindGameObjectsWithTag("SpawnEnemy");
        for (int i = 0; i < currentSpawnEnemy.Length; i++)
        {
            if (!SaveManager.GetInstance().SpawnEnemyId.Contains(currentSpawnEnemy[i].GetComponent<SpawnEnemy>().id))
            {
                Destroy(currentSpawnEnemy[i]);
            }
        }

        // computer(the other spawnEnemyPoint)
        GameObject[] Computer = GameObject.FindGameObjectsWithTag("computer");
        bool flag = false; // 该怪物生成点在保存文件中是否还存在
        for (int i = 0; i < Computer.Length; i++)
        {
            if (Computer[i].GetComponent<SpawnEnemy>() != null)
            {
                if (!SaveManager.GetInstance().ComputerId.Contains(Computer[i].GetComponent<SpawnEnemy>().id))
                {
                    flag = true;
                    break;
                }
            }
        }
        // 要摧毁整个物体
        if (flag)
        {
            for (int i = 0; i < Computer.Length; i++)
            {
                Destroy(Computer[i]);
            }
        }
    }

    public void Update()
    {
        // 主角死亡，回到上一个存档点
        if (playerProperty.GetProperty(E_Property.san) <= 0)
        {
            EventCenter.GetInstance().Clear();
            SaveManager.GetInstance().LoadGame();
            return;
        }
        //Debug.Log("Player.Transform: " + transform.position);
        // 回血
        float san = playerProperty.GetProperty(E_Property.san);
        float maxSan = GlobalSetting.GetInstance().san;
        if (san < maxSan && isGettingHurt == false)
        {
            //Debug.Log("Healinginging");
            playerProperty.SetProperty(E_Property.san, san + Time.deltaTime * RecoverRatio);
        }



        // per frame updating
        TowardMouseDirection();
        CheckPlayerSan();

        // Update coldDown counter
        if (currDashTime > 0) currDashTime -= Time.deltaTime;
        if (currGunHeat > 0) currGunHeat -= Time.deltaTime;
        if(curSwallowTime > 0)curSwallowTime -= Time.deltaTime;
        if (currinteractTime > 0) currinteractTime -= Time.deltaTime;

        // Check move action
        if (ContainStatus(E_InputStatus.moving))
        {
            playerController.Act(E_InputStatus.moving);
            playerAnimator.Move();
        }
        else playerAnimator.Idle();

        // Check fire action
        if (ContainStatus(E_InputStatus.firing))
        {

            // 判断鼠标是否放到了UI上
            bool isMouseOnUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

            if (isMouseOnUI) // 鼠标是否点击UI
            {
                return;
            }

            Debug.Log("Mouse is clicked on : " + isMouseOnUI);


            if (CheckFire())
            {
                currGunHeat = gunHeat;
                playerController.Act(E_InputStatus.firing);
                playerAnimator.Attack();
            }
                
        }
        else playerAnimator.ClearAttack();

        // Check interact action
        if (ContainStatus(E_InputStatus.interacting))
        {
            if (CheckInteract())
            {
                currinteractTime = interactTime;
                playerController.Act(E_InputStatus.interacting);
                playerAnimator.TakeDamage();
            }
        }
        else playerAnimator.ClearTakeDamage();

        // Check special action
        if (ContainStatus(E_InputStatus.swallowingAndFiring))
        {
            if (CheckSwallow())
            {
                curSwallowTime = swallowTime;
                playerController.Act(E_InputStatus.swallowingAndFiring);
                playerAnimator.TakeDamage();
            }
        }
        else playerAnimator.ClearTakeDamage();

        // Check dash action
        if (ContainStatus(E_InputStatus.dashing))
        {
            if (CheckDash())
            {
                currDashTime = dashTime;

                float res = playerProperty.GetProperty(E_Property.resilience);
                playerProperty.SetProperty(E_Property.resilience, res - dashDecreaseRes);

                playerController.Act(E_InputStatus.dashing);
                playerAnimator.Dash();
            }
        }
        else playerAnimator.ClearDash();

        // Check dead action
        if (ContainStatus(E_InputStatus.die))
        {
            playerAnimator.Die();
            playerController.Act(E_InputStatus.die);
        }

        //Debug.Log("Build NavMeshSurface");
        //surface.BuildNavMesh();
    }

    // per frame update function

    // update player's forward direction
    private void TowardMouseDirection()
    {
        // Get mouse world direction
        Vector3 screenWorldPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 mouseScreenPostion = Mouse.current.position.ReadValue();
        mouseScreenPostion.z = screenWorldPos.z;
        Vector3 mouseWorldPostion = Camera.main.ScreenToWorldPoint(mouseScreenPostion);
        mouseWorldPostion.y = transform.position.y;
        // Set mouse direction
        Vector3 mouseDirection = mouseWorldPostion - transform.position;
        mouseDirection = Vector3.Normalize(mouseDirection);

        transform.LookAt(mouseWorldPostion);
    }

    // check player's property and colddown counter
    private void CheckPlayerSan()
    {
        if (playerProperty.GetProperty(E_Property.san) <= 0)
            AddStatus(E_InputStatus.die);
    }

    
    private bool CheckFire()
    {
        if (currGunHeat <= 0) return true;
        return false;
    }

    private bool CheckDash()
    {
        if (currDashTime <= 0 && playerProperty.GetProperty(E_Property.resilience) >= dashDecreaseRes) return true;
        return false;
    }

    private bool CheckSwallow()
    {
        return curSwallowTime <= 0;
    }

    private bool CheckInteract()
    {
        if (currinteractTime <= 0) return true;
        return false;
    }

    // Damage Receiver

    // 每deltaTime受到一次伤害，每次伤害为extraDamage，共受到count次伤害
    IEnumerator ReceiveExtraDamage(float deltaTime, float extraDamage, int count)
    {
        for (int i = 0; i < count; i++)
        {
            isGettingHurt = true; // 毒弹的时间太长了，需要手动标记该变量

            float san = playerProperty.GetProperty(E_Property.san);
            playerProperty.SetProperty(E_Property.san, san - extraDamage);

            // 最后一次受到伤害不需要再等待，方便后续计时HealingTime
            if (i != count - 1)
            {
                // 先立即受到一次额外伤害，然后等待deltaTime
                yield return new WaitForSeconds(deltaTime);
            }
        }


        Debug.Log("持续伤害结束!");

        StartCoroutine(GettingHurt()); // 开启时长为HealingTime的定时器，定时器结束后开始回血
    }

    // 玩家受到时长为deltaTime的减速效果
    IEnumerator ReceiveIceEffect(float deltaTime)
    {
        playerController.SetSlowSpeed();
        yield return new WaitForSeconds(deltaTime);
        playerController.SetNormalSpeed();
    }
    // 玩家受到时长为deltaTime的键位倒置效果
    IEnumerator ReceiveSpiritPosionEffect(float deltaTime)
    {
        playerController.SetIsSpiritPosioned(true);
        yield return new WaitForSeconds(deltaTime);
        playerController.SetIsSpiritPosioned(false);
    }

    IEnumerator ReceiveExplodeEffect(float damage)
    {
        float san = playerProperty.GetProperty(E_Property.san);
        playerProperty.SetProperty(E_Property.san, san - damage);
        yield return null;
    }

    public void PlayerReceiveDamage(SpecialBullet bullet)
    {
        StartCoroutine(GettingHurt()); // 标记为正在受到伤害状态

        //Debug.LogWarning("In PlayerReceiveDamage + bullet.type: " + bullet.bulletType + bullet.damage);

        playerProperty.SetProperty(E_Property.san, playerProperty.GetProperty(E_Property.san) - bullet.damage);
        switch (bullet.bulletType)
        {
            case E_PoolType.FireBullet:
                break;
            case E_PoolType.ThunderBullet:
                StartCoroutine(ReceiveExtraDamage(0, bullet.extraDamage, 1));
                break;
            case E_PoolType.ExplodeBullet:
                StartCoroutine(ReceiveExplodeEffect(bullet.extraDamage));
                break;
            case E_PoolType.BurnBullet:
                StartCoroutine(ReceiveExtraDamage(0.5f, 5.0f, 6));
                break;
            case E_PoolType.IceBullet: // 冰弹的减速效果，3秒后消失
                StartCoroutine(ReceiveIceEffect(3.0f));
                break;
            case E_PoolType.PoisonBullet:
                StartCoroutine(ReceiveExtraDamage(1.0f, 3.0f, 10));
                break;
            case E_PoolType.SpiritPoisonBullet:
                StartCoroutine(ReceiveSpiritPosionEffect(3.0f));
                break;
        }
    }

    public void PlayerReceiveDamage(float damage)
    {
       // Debug.LogWarning("In PlayerReceiveDamage receive other damage: " + damage);
        float san = playerProperty.GetProperty(E_Property.san);
        playerProperty.SetProperty(E_Property.san, san - damage);
    }


    // interface

    // property control
    public void SetProperty(E_Property eProperty, object property)
    {
        playerProperty.SetProperty(eProperty, property);
    }

    public float GetProperty(E_Property eProperty)
    {
        return playerProperty.GetProperty(eProperty);
    }

    // status control
    public void AddStatus(E_InputStatus inputStatus)
    {
        playerStatus.AddStatus(inputStatus);
    }

    public bool ContainStatus(E_InputStatus inputStatus)
    {
        return playerStatus.ContainStatus(inputStatus);
    }

    public void ClearStatus()
    {
        playerStatus.ClearStatus();
    }

    public void ClearStatus(E_InputStatus inputStatus)
    {
        playerStatus.ClearStatus(inputStatus);
    }

    // player info
    public Transform GetPlayerTransform()
    {
        return this.transform;
    }
}
