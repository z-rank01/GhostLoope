using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerProperty playerProperty;
    PlayerStatus playerStatus;
    PlayerController playerController;

    public void Awake()
    {
        // Mono
        playerProperty = gameObject.GetComponent<PlayerProperty>();
        playerController = gameObject.GetComponent<PlayerController>();

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
        if (ContainStatus(E_InputStatus.moving))
        {
            ActInfo.GetInstance().SetActInfo(playerProperty.GetSpeed());
            playerController.Act(E_InputStatus.moving);
        }
        if (ContainStatus(E_InputStatus.firing))
        {
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

    }

    // interface

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
