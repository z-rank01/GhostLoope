using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    Rigidbody rigidBody;
    PlayerInputControl playerInputControl;
    
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        playerInputControl = new PlayerInputControl();
    }


    // interface
    public void Act(E_InputStatus inputStatus)
    {
        switch (inputStatus)
        {
            case E_InputStatus.moving:
                Move();
                break;
            case E_InputStatus.firing:
                Fire();
                break;
            case E_InputStatus.interacting:
                Interact();
                break;
            case E_InputStatus.swallowingAndFiring:
                SwallowAndFire();
                break;
            case E_InputStatus.dashing:
                Dash();
                break;
        }
    }

    private void Move()
    {
        Debug.Log("Move");
        Vector2 moveDirection = ActInfo.GetInstance().moveDirection;
        rigidBody.AddForce(new Vector3(moveDirection.x, 0, moveDirection.y) 
                            * ActInfo.GetInstance().speed);
    }

    private void Fire()
    {
        Debug.Log("Fire");
    }

    private void Interact()
    {
        Debug.Log("Interact");
    }

    private void SwallowAndFire()
    {
        Debug.Log("SwallowAndFire");
    }

    private void Dash()
    {
        Debug.Log("Dash");
    }

    
}
