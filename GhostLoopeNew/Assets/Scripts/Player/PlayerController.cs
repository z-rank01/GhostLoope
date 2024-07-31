using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    PlayerInputControl playerInputControl;
    
    // temporary variable
    float speed;
    float dashSpeed;
    Vector3 fireDirection;
    
    private void Awake()
    {
        playerInputControl = new PlayerInputControl();
    }

    private void Start()
    {
        speed = Player.GetInstance().GetProperty(E_Property.speed);
        dashSpeed = Player.GetInstance().GetProperty(E_Property.dashSpeed);
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
        transform.position += new Vector3(moveDirection.x, 0, moveDirection.y) * speed;
    }

    private void Fire()
    {
        Debug.Log("Fire");
        
        // Get mouse world direction
        Vector3 mouseWorldPostion = Input.mousePosition;


        Bullet bullet = PoolManager.GetInstance().GetObj(E_PoolType.SimpleBullet).GetComponent<Bullet>();
        bullet.FireOut(transform.position + new Vector3(1, 0, 0), 
                       new Vector3(1, 0, 0), 
                       ActInfo.GetInstance().bulletSpeed);
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

        Vector2 moveDirection = ActInfo.GetInstance().moveDirection;
        Debug.Log("MoveDirection: " +  moveDirection);
        Debug.Log("dashSpeed: " + dashSpeed);
        transform.position += new Vector3(moveDirection.x, 0, moveDirection.y) * dashSpeed;
    }

    
}
