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
    private int counter = 0;
    private bool isCount = false;

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
        if (ContainStatus(E_InputStatus.swallowingAndFiring))
        {
            playerController.Act(E_InputStatus.swallowingAndFiring);
        }
        if (ContainStatus(E_InputStatus.dashing))
        {
            playerController.Act(E_InputStatus.dashing);
        }

        if (isCount)
            UpdateCounter();
    }

    
    private bool CheckFire()
    {
        if (counter % coldDown == 0) return true;
        return false;
    }

    private void UpdateCounter()
    {
        counter++;
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
