using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : BaseSingletonMono<Player>
{
    PlayerProperty playerProperty;
    PlayerStatus playerStatus;
    PlayerController playerController;

    [SerializeField]
    private float gunHeat;
    private float currGunHeat = 0;

    [SerializeField]
    private float dashTime = 1f; // dash的冷却时间
    private float currDashTime = 0f; // 触发下一次dash的冷却时间，如果 <= 0则可以dash


    float swallowTime = 1.0f; //吞噬技能的冷却时间
    float curSwallowTime = 0.0f; // 触发下一次吞噬的冷却时间，如果 <= 0则可以吞噬


    public void Awake()
    {
        // Mono
        playerProperty = gameObject.AddComponent<PlayerProperty>();
        playerController = gameObject.AddComponent<PlayerController>();

        // not Mono
        playerStatus = new PlayerStatus();
    }

    public void Start()
    {
        // not Mono
        playerStatus.Init();


        // MusicManager.GetInstance().PlayBkMusic("TestMusic");

    }

    public void Update()
    {
        // Update coldDown counter
        if (currDashTime > 0) currDashTime -= Time.deltaTime;
        if (currGunHeat > 0) currGunHeat -= Time.deltaTime;

        // Check action
        if (ContainStatus(E_InputStatus.moving))
        {
            playerController.Act(E_InputStatus.moving);
        }

        if (ContainStatus(E_InputStatus.firing))
        {
            if (CheckFire())
            {
                currGunHeat = gunHeat;
                playerController.Act(E_InputStatus.firing);
            }
                
        }

        if (ContainStatus(E_InputStatus.interacting))
        {
            playerController.Act(E_InputStatus.interacting);
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
            }
        }
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
