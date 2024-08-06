using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    PlayerInputControl playerInputControl;
    Rigidbody rb;
    
    // temporary variable
    float speed;
    float dashSpeed;

    float slowSpeed;

    Vector3 fireDirection;

    bool isSpiritPosioned = false; // 玩家是否受到精神毒素伤害


    SwallowRange swallowRange;

    public void Start()
    {
        playerInputControl = new PlayerInputControl();
        rb = GetComponent<Rigidbody>();

        if (GameObject.Find("SwallowRange") != null)
        {
            swallowRange = GameObject.Find("SwallowRange").GetComponent<SwallowRange>();
        }

        speed = Player.GetInstance().GetProperty(E_Property.speed);
        dashSpeed = Player.GetInstance().GetProperty(E_Property.dashSpeed);

        slowSpeed = Player.GetInstance().GetProperty(E_Property.slowSpeed);
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
                Swallow();
                break;
            case E_InputStatus.dashing:
                Dash();
                break;
            case E_InputStatus.die:
                Die();
                break;
        }
    }

    private void Move()
    {
        Debug.Log("Move, " + speed);
        Vector2 moveDirection = ActInfo.GetInstance().moveDirection;
        //transform.position += new Vector3(moveDirection.x, 0, moveDirection.y) * speed;
        float posionDirection = isSpiritPosioned ? -1 : 1; 

        rb.AddForce(new Vector3(moveDirection.x, 0, moveDirection.y) * speed * posionDirection);
    }

    private void Fire()
    {


        if (swallowRange.ReadyToFire())
        {
            Debug.Log("发射特殊子弹");
            swallowRange.FireSpecial();
            return;
        }

        Debug.Log("Fire");

        Debug.Log("发射普通子弹");




        // Get mouse world direction
        Vector3 screenWorldPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 mouseScreenPostion = Mouse.current.position.ReadValue();
        mouseScreenPostion.z = screenWorldPos.z;
        Vector3 mouseWorldPostion = Camera.main.ScreenToWorldPoint(mouseScreenPostion);

        // Set fire direction
        fireDirection = mouseWorldPostion - transform.position;
        fireDirection = Vector3.Normalize(fireDirection);


        fireDirection.y = 0;



        // Set fire origin
        Vector3 fireOrigin = transform.position + fireDirection * 20.0f;

        fireOrigin += new Vector3(0.0f, 5.0f, 0.0f);




       
        Debug.Log("发射普通子弹");
        Bullet bullet = PoolManager.GetInstance().GetObj(E_PoolType.SimpleBullet).GetComponent<Bullet>();
        
        bullet.FireOut(fireOrigin, 
                        fireDirection,
                        GlobalSetting.GetInstance().bulletSpeed);





        MusicManager.GetInstance().PlayFireSound("洛普-普攻-射出"); // 添加子弹音效

        MusicManager.GetInstance().PlayFireSound("洛普-普攻-飞行"); // 添加子弹音效
    }


    private void Interact()
    {
        Debug.Log("Interact");
        EventCenter.GetInstance().EventTrigger(E_Event.Conversation);
    }


    // 右键吞噬
    private void Swallow()
    {
        Debug.Log("SwallowAndFire");

        // 如果没有吞噬子弹，则进行吞噬
        if (!swallowRange.ReadyToFire())
        {
            swallowRange.SwallowBullet();
        }




        //// 如果已经吞噬了特殊子弹，则射击
        //if (swallowRange.ReadyToFire())
        //{
        //    //Debug.Log("射击已经吞噬的特殊子弹");
        //    swallowRange.FireSpecial();
        //}
        //// 否则，进入吞噬判定
        //else
        //{
            
        //}

    }
 
    private void Dash()
    {
        
        Debug.Log("Dash, " + dashSpeed);

        Vector2 moveDirection = ActInfo.GetInstance().moveDirection;
        //Debug.Log("MoveDirection: " +  moveDirection);
        //Debug.Log("dashSpeed: " + dashSpeed);
        //transform.position += new Vector3(moveDirection.x, 0, moveDirection.y) * dashSpeed;


        float posionDirection = isSpiritPosioned ? -1 : 1;


        rb.AddForce(new Vector3(moveDirection.x, 0, moveDirection.y) * dashSpeed * posionDirection);
    }

    private void Die()
    {
        gameObject.SetActive(false);
        // trigger other event
    }


    // 进入玩家的碰撞范围, 玩家收到伤害
    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("In PlayerController OnTriggerEnter~~~~~~~~~~~~~~~~~~~~~~~");

        SpecialBullet bullet = other.GetComponent<SpecialBullet>();
        if (bullet != null && bullet.isSwallowed == false)
        {
            swallowRange.RemoveBulleet(bullet);
            Debug.Log("有子弹击中了你!!!");
            EventCenter.GetInstance().EventTrigger<SpecialBullet>(E_Event.PlayerReceiveDamage, bullet);
            PoolManager.GetInstance().ReturnObj(bullet.bulletType, bullet.gameObject);
        }

    }
    public void SetSlowSpeed()
    {
        speed = Player.GetInstance().GetProperty(E_Property.slowSpeed);
    }
    public void SetNormalSpeed()
    {
        speed = Player.GetInstance().GetProperty(E_Property.speed);
    }

    public void SetIsSpiritPosioned(bool _isSpiritPosioned)
    {
        isSpiritPosioned = _isSpiritPosioned;
    }

}
