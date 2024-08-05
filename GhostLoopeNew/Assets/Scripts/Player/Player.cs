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
    //PlayerAnimator_GhostLoope playerAnimator;

    [SerializeField]
    private float gunHeat;  // 普通射击冷却时间
    private float currGunHeat = 0;  //  触发下一次攻击的冷却时间，如果 <= 0则可以dash

    [SerializeField]
    private float dashTime = 1f; // dash的冷却时间
    private float currDashTime = 0f; // 触发下一次dash的冷却时间，如果 <= 0则可以dash

    [SerializeField]
    private float swallowTime = 1.0f; //吞噬技能的冷却时间
    private float curSwallowTime = 0.0f; // 触发下一次吞噬的冷却时间，如果 <= 0则可以吞噬

    [SerializeField]
    private float interactTime;  // 普通射击冷却时间
    private float currinteractTime = 0;  //  触发下一次攻击的冷却时间，如果 <= 0则可以dash




    

    

    public void Awake()
    {
        

        Init();
        // Mono
        playerProperty = gameObject.AddComponent<PlayerProperty>();
        playerController = gameObject.AddComponent<PlayerController>();
        //playerAnimator = gameObject.AddComponent<PlayerAnimator_GhostLoope>();

        // not Mono
        playerStatus = new PlayerStatus();
        playerStatus.Init();


        // 击中玩家后的响应事件
        EventCenter.GetInstance().AddEventListener<SpecialBullet>(E_Event.PlayerReceiveDamage, PlayerReceiveDamage);
        // MusicManager.GetInstance().PlayBkMusic("TestMusic");
    }


    public void Update()
    {


        // 玩家死亡
        float SAN = GetProperty(E_Property.san);
        if (SAN <= 0)
        {
            Destroy(gameObject);
        }




        // Update coldDown counter
        if (currDashTime > 0) currDashTime -= Time.deltaTime;
        if (currGunHeat > 0) currGunHeat -= Time.deltaTime;
        if(curSwallowTime > 0)curSwallowTime -= Time.deltaTime;
        if (currinteractTime > 0) currinteractTime -= Time.deltaTime;


       






        // Check action
        if (ContainStatus(E_InputStatus.moving))
        {
            playerController.Act(E_InputStatus.moving);
            //playerAnimator.MoveForward();
            //playerAnimator.MoveLeft();
            //playerAnimator.MoveRight();
        }
        else
        {
            //playerAnimator.ClearForwad();
            //playerAnimator.ClearLeft();
            //playerAnimator.ClearRight();
        }

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
                //LookAtMouseDirection();
                //playerAnimator.Attack();
            }
                
        }

        if (ContainStatus(E_InputStatus.interacting))
        {
            if (CheckInteract())
            {
                currinteractTime = interactTime;
                playerController.Act(E_InputStatus.interacting);
            }
        }

        if (ContainStatus(E_InputStatus.swallowingAndFiring))
        {
            if (CheckSwallowAndFire())
            {
                curSwallowTime = swallowTime;
                playerController.Act(E_InputStatus.swallowingAndFiring);

            }
        }
        
        if (ContainStatus(E_InputStatus.dashing))
        {
            if (CheckDash())
            {
                currDashTime = dashTime;


                float res = playerProperty.GetProperty(E_Property.resilience);
                playerProperty.SetProperty(E_Property.resilience, res - 10);

                playerController.Act(E_InputStatus.dashing);
                //playerAnimator.Dash();
            }
        }
    }

    private void LookAtMouseDirection()
    {
        // Get mouse world direction
        Vector3 screenWorldPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 mouseScreenPostion = Mouse.current.position.ReadValue();
        mouseScreenPostion.z = screenWorldPos.z;
        Vector3 mouseWorldPostion = Camera.main.ScreenToWorldPoint(mouseScreenPostion);

        // Set mouse direction
        Vector3 mouseDirection = mouseWorldPostion - transform.position;
        //mouseDirection = Vector3.Normalize(mouseDirection);
        mouseDirection.y = 0;
        transform.LookAt(mouseDirection);
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


    // 每deltaTime受到一次伤害，每次伤害为extraDamage，共受到count次伤害
    IEnumerator ReceiveExtraDamage(float deltaTime, float extraDamage, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float san = playerProperty.GetProperty(E_Property.san);
            playerProperty.SetProperty(E_Property.san, san - extraDamage);

            // 先立即受到一次额外伤害，然后等待deltaTime
            yield return new WaitForSeconds(deltaTime);
        }
    }



    // 玩家受到时长为deltaTime的减速效果
    IEnumerator ReceiveIceEffect(float deltaTime)
    {
        playerController.SetSlowSpeed();
        yield return new WaitForSeconds(deltaTime);
        playerController.SetNormalSpeed();
    }

    IEnumerator ReceiveSpiritPosionEffect(float deltaTime)
    {
        playerController.SetIsSpiritPosioned(true);
        yield return new WaitForSeconds(deltaTime);
        playerController.SetIsSpiritPosioned(false);
    }


    public void PlayerReceiveDamage(SpecialBullet bullet)
    {
        Debug.Log("In PlayerReceiveDamage + bullet.type: " + bullet.bulletType + bullet.damage);

        playerProperty.SetProperty(E_Property.san, playerProperty.GetProperty(E_Property.san) - bullet.damage);



        switch (bullet.bulletType)
        {
            case E_PoolType.FireBullet:
                break;
            case E_PoolType.ThunderBullet:


                StartCoroutine(ReceiveExtraDamage(0, bullet.extraDamage, 1));


                break;
            case E_PoolType.ExplodeBullet:
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
}
