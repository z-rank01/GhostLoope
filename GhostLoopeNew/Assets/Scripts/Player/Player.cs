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
    private int coldDown;

    [SerializeField]
    private int dashColdDown;


    private int counter = 0;
    private bool isCount = false;


    private int counterDash = 0;
    private bool isCountDash = false;


    float dashTime = 0.5f; // ��̵���ȴʱ��
    float curDashTime = 0.0f; // ������һ�γ�̵���ȴʱ�䣬��� <= 0����Գ��


    float swallowTime = 1.0f; //���ɼ��ܵ���ȴʱ��
    float curSwallowTime = 0.0f; // ������һ�����ɵ���ȴʱ�䣬��� <= 0���������


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





        MusicManager.GetInstance().PlayBkMusic("TestMusic");

    }

    public void Update()
    {
        // Check action
        if (ContainStatus(E_InputStatus.moving))
        {
            playerController.Act(E_InputStatus.moving);
        }
        if (ContainStatus(E_InputStatus.firing))
        {
            if (CheckFire())
                playerController.Act(E_InputStatus.firing);
        }
        if (ContainStatus(E_InputStatus.interacting))
        {
            playerController.Act(E_InputStatus.interacting);
        }
        if (curSwallowTime > 0) curSwallowTime -= Time.deltaTime;
        if (ContainStatus(E_InputStatus.swallowingAndFiring))
        {
            if (CheckSwallowAndFire())
            {
                curSwallowTime = swallowTime;
                playerController.Act(E_InputStatus.swallowingAndFiring);

            }
        }
        if (curDashTime > 0) curDashTime -= Time.deltaTime;

        if (ContainStatus(E_InputStatus.dashing))
        {
            if (CheckDash())
            {
                curDashTime = dashTime;


                float res = playerProperty.GetProperty(E_Property.resilience);
                playerProperty.SetProperty(E_Property.resilience, res - 10);

                playerController.Act(E_InputStatus.dashing);
            }
        }

        if (isCount)
            UpdateCounter();

    }

    
    private bool CheckFire()
    {
        if (counter % coldDown == 0) return true;
        return false;
    }


    private bool CheckDash()
    {
        if (curDashTime <= 0 && playerProperty.GetProperty(E_Property.resilience) >= 10) return true;
        return false;


        //if (counterDash % dashColdDown == 0 && playerProperty.GetProperty(E_Property.resilience) >= 10)
        //{
        //    Debug.Log("couterDash: " + counterDash);
        //    Debug.Log("dashColdDown: " + dashColdDown);
        //    return true;
        //}
        //return false;
    }

    private bool CheckSwallowAndFire()
    {
        return curSwallowTime <= 0;
    }
    private void UpdateCounter()
    {
        counter++;
    }
    private void UpdateCounterDash()
    {
        counterDash++;
    }

    // interface

    // Counter
    public void StartCounter()
    {
        isCount = true;
    }

    public void ClearCounter()
    {
        counter = 0;
        isCount = false;
    }


    public void StartCounterDash()
    {
        isCountDash = true;
    }

    public void ClearCounterDash()
    {
        counterDash = 0;
        isCountDash = false;
    }




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
