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


    SpecialBullet swallowBullet; // 所吞噬的特殊子弹
    List<SpecialBullet> specialBullets = new List<SpecialBullet>(); // 进入吞噬范围内的特殊子弹列表

    public void Start()
    {
        playerInputControl = new PlayerInputControl();
        rb = GetComponent<Rigidbody>();

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

    // 发射已经吞噬的特殊子弹
    private void FireSpecial(SpecialBullet bullet)
    {
        //Debug.Log("FireSpecial Before");
        if(bullet == null) return;


        //Debug.Log("FireSpecial End");

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


        //SpecialBullet bullet = PoolManager.GetInstance().GetObj(E_PoolType.FireBullet).GetComponent<SpecialBullet>();


        //Debug.Log("Bullet: !!!!!!!!!!!!!!" + bullet);
        // Bullet bullet = PoolManager.GetInstance().GetObj(E_PoolType.SimpleBullet).GetComponent<Bullet>();

        // 直接将传入的子弹发射出去
        bullet.FireOut(fireOrigin,
                       fireDirection,
                       GlobalSetting.GetInstance().specialBulletSpeed);


        
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
        if (swallowBullet != null)
        {
            
            //Debug.Log("射击已经吞噬的特殊子弹");
            //Debug.Log("swallowBullet.bulletType: " + swallowBullet.bulletType);
            FireSpecial(swallowBullet);


            swallowBullet = null;
        }
        // 否则，进入吞噬判定
        else
        {
            // 选择第一个进入吞噬范围内的特殊子弹进行吞噬
            if (specialBullets.Count > 0)
            {
                
                // 吞噬成功减去SAN值
                float SAN = Player.GetInstance().GetProperty(E_Property.san);
                float RES = Player.GetInstance().GetProperty(E_Property.resilience);

                Player.GetInstance().SetProperty(E_Property.san, SAN - 10);
                Player.GetInstance().SetProperty(E_Property.resilience, RES + 10);



                E_PoolType swallowBulletType = specialBullets[0].bulletType;


                
                swallowBullet = PoolManager.GetInstance().GetObj(swallowBulletType).GetComponent<SpecialBullet>();

                swallowBullet.bulletType = swallowBulletType;
                //Debug.Log("swallowBulletType: " + swallowBulletType);
                //Debug.Log("选择第一个进入吞噬范围内的特殊子弹进行吞噬");


                PoolManager.GetInstance().ReturnObj(specialBullets[0].bulletType, specialBullets[0].gameObject);

                specialBullets.RemoveAt(0);

            }
        }

    }


    
    // 进入玩家本身碰撞体的范围
    public void OnCollisionEnter(Collision collision) 
    { 
        
        //Debug.Log("In Player OnCollisionEnter: " + collision.collider.name);
        SpecialBullet bullet = collision.collider.gameObject.GetComponent<SpecialBullet>();
        if (bullet != null)
        {
            if (specialBullets.Contains(bullet))
            {
                specialBullets.Remove(bullet);
            }
            //Debug.Log("有子弹击中了你!!!");
            EventCenter.GetInstance().EventTrigger<SpecialBullet>(E_Event.PlayerReceiveDamage, bullet);


            PoolManager.GetInstance().ReturnObj(bullet.bulletType, bullet.gameObject);


        }

    }
    // 进入吞噬范围, 将进入吞噬范围内的子弹放入列表d
    public void OnTriggerEnter(Collider other)
    {
        SpecialBullet currentBullet = other.GetComponent<SpecialBullet>();
        if (currentBullet != null)
        {
            //Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!In Player OnTriggerEnter: " + other.name);


            //Debug.Log(other.gameObject.GetComponent<SpecialBullet>().bulletType);
            specialBullets.Add(other.GetComponent<SpecialBullet>());
        }
    }

    // 退出吞噬范围，将退出吞噬范围内的子弹移除列表
    public void OnTriggerExit(Collider other)
    {
        SpecialBullet currentBullet = other.GetComponent<SpecialBullet>();
        if (currentBullet != null && specialBullets.Contains(currentBullet))
        {
            specialBullets.Remove(currentBullet);
        }
       // Debug.Log("In Player OnTriggerExit: " + other.name);
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
