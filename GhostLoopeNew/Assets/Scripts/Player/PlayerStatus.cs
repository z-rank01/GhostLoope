using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_InputStatus
{
    moving,
    firing,
    interacting,
    dashing,
    swallowingAndFiring, 
    die
}

public class PlayerStatus
{
    private HashSet<E_InputStatus> playerInputStatus;

    public void Init()
    {
        playerInputStatus = new HashSet<E_InputStatus>();
    }

    public void AddStatus(E_InputStatus inputStatus)
    {
        playerInputStatus.Add(inputStatus);
    }

    public bool ContainStatus(E_InputStatus neededStatus)
    {
        if (playerInputStatus.Contains(neededStatus)) return true;
        else return false;
    }

    public void ClearStatus()
    {
        playerInputStatus.Clear();
    }

    public void ClearStatus(E_InputStatus inputStatus)
    {
        playerInputStatus.Remove(inputStatus);
    }
}
