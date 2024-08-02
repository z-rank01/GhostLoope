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
    Vector3 fireDirection;


    //SpecialBullet swallowBullet; // 所吞噬的特殊子弹
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
        //swallowBullet = Player.GetInstance().AddComponent<SpecialBullet>();
    }

    public void Update()
    {
        float SAN = Player.GetInstance().GetProperty(E_Property.san);
        if (SAN <= 0)
        {
            Destroy(Player.GetInstance().gameObject);
        }
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
        Debug.Log("Move, " + speed);
        Vector2 moveDirection = ActInfo.GetInstance().moveDirection;
        //transform.position += new Vector3(moveDirection.x, 0, moveDirection.y) * speed;
        rb.AddForce(new Vector3(moveDirection.x, 0, moveDirection.y) * speed);
    }

    private void Fire()
    {
        Debug.Log("Fire");

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

        Bullet bullet = PoolManager.GetInstance().GetObj(E_PoolType.SimpleBullet).GetComponent<Bullet>();
        
        bullet.FireOut(fireOrigin, 
                       fireDirection,
                       GlobalSetting.GetInstance().bulletSpeed);
    }

    

    private void Interact()
    {
        Debug.Log("Interact");
        EventCenter.GetInstance().EventTrigger(E_Event.Conversation);
    }

    private void SwallowAndFire()
    {
        Debug.Log("SwallowAndFire");


        // 如果已经吞噬了特殊子弹，则射击
        if (swallowRange.ReadyToFire())
        {
            //Debug.Log("射击已经吞噬的特殊子弹");
            swallowRange.FireSpecial();
        }
        // 否则，进入吞噬判定
        else
        {
            swallowRange.SwallowBullet();
        }

    }





    // 进入玩家的碰撞范围, 玩家收到傷害
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

    
    private void Dash()
    {
        
        Debug.Log("Dash, " + dashSpeed);

        Vector2 moveDirection = ActInfo.GetInstance().moveDirection;
        //Debug.Log("MoveDirection: " +  moveDirection);
        //Debug.Log("dashSpeed: " + dashSpeed);
        //transform.position += new Vector3(moveDirection.x, 0, moveDirection.y) * dashSpeed;
        rb.AddForce(new Vector3(moveDirection.x, 0, moveDirection.y) * dashSpeed);
    }

    
}
