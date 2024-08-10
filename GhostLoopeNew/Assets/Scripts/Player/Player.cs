using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private float dashTime = 1f; // 冲刺的冷却时间
    private float currDashTime = 0f; // 触发下一次冲刺的冷却时间，如果 <= 0则可以冲刺

    [SerializeField]
    private float swallowTime = 1.0f; //吞噬技能的冷却时间
    private float curSwallowTime = 0.0f; // 触发下一次吞噬的冷却时间，如果 <= 0则可以吞噬

    [SerializeField]
    private float interactTime;  // 交互冷却时间
    private float currinteractTime = 0;  //  触发下一次交互的冷却时间，如果 <= 0则可以交互

    [SerializeField]
    private float HealingTime; // 经过HealingTime时长没有收到伤害，主角便会回血

    [SerializeField]
    private float RecoverRatio; // 玩家回血的速率，每秒恢复recoverRatio点生命值

    private bool isGettingHurt = false; // 玩家是否正受到伤害


    private bool isGettingSoul_1 = false; // 是否得到了第一关Boss的灵魂
    private bool isGettingSoul_2 = false; // 是否得到了第二关Boss的灵魂

    public void SetSoul_1(bool value) { isGettingSoul_1 = value; }
    public void SetSoul_2(bool value) { isGettingSoul_2 = value; }
    public bool GetSoul_1() { return isGettingSoul_1; }
    public bool GetSoul_2() { return isGettingSoul_2; }
    
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
    }


    public void Update()
    {
        // 回血
        float san = playerProperty.GetProperty(E_Property.san);
        float maxSan = GlobalSetting.GetInstance().san;
        if (san < maxSan && isGettingHurt == false)
        {
            Debug.Log("Healinginging");
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
            if (CheckSwallowAndFire())
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
                playerProperty.SetProperty(E_Property.resilience, res - 10);

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
        if (currDashTime <= 0 && playerProperty.GetProperty(E_Property.resilience) >= 10) return true;
        return false;
    }

    private bool CheckSwallowAndFire()
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

        //Debug.Log("In PlayerReceiveDamage + bullet.type: " + bullet.bulletType + bullet.damage);

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
        Debug.Log("In PlayerReceiveDamage receive damage: " + damage);
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
