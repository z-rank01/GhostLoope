using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
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


    SpecialBullet swallowBullet; // 所吞噬的特殊子弹
    bool isSwallowed = false;
    List<SpecialBullet> specialBullets = new List<SpecialBullet>(); // 进入吞噬范围内的特殊子弹列表

    private void Awake()
    {
        playerInputControl = new PlayerInputControl();
    }

    private void Start()
    {
        speed = Player.GetInstance().GetProperty(E_Property.speed);
        dashSpeed = Player.GetInstance().GetProperty(E_Property.dashSpeed);

        //swallowBullet = Player.GetInstance().AddComponent<SpecialBullet>();


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
        Vector3 screenWorldPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 mouseScreenPostion = Mouse.current.position.ReadValue();
        mouseScreenPostion.z = screenWorldPos.z;
        Vector3 mouseWorldPostion = Camera.main.ScreenToWorldPoint(mouseScreenPostion);

        // Set fire direction
        fireDirection = mouseWorldPostion - transform.position;
        fireDirection = Vector3.Normalize(fireDirection);
        fireDirection.y = 0;

        // Set fire origin
        Vector3 fireOrigin = transform.position + fireDirection * 2.0f;

        Bullet bullet = PoolManager.GetInstance().GetObj(E_PoolType.SimpleBullet).GetComponent<Bullet>();
        

        bullet.FireOut(fireOrigin, 
                       fireDirection,
                       GlobalSetting.GetInstance().bulletSpeed);
    }

    // 发射已经吞噬的特殊子弹
    private void FireSpecial(SpecialBullet bullet)
    {
        if(bullet == null) return;


        Debug.Log("FireSpecial");

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
        Vector3 fireOrigin = transform.position + fireDirection * 2.0f;


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
        //EventCenter.GetInstance().EventTrigger(E_Event, );
    }

    private void SwallowAndFire()
    {
        Debug.Log("SwallowAndFire");
        
        
        // 如果已经吞噬了特殊子弹，则射击
        if (isSwallowed)
        {
            isSwallowed = false;
            Debug.Log("射击已经吞噬的特殊子弹");

            FireSpecial(swallowBullet);
            //swallowBullet = null;
            //GameObject.Destroy(swallowBullet);
        }
        // 否则，进入吞噬判定
        else
        {
            // 选择第一个进入吞噬范围内的特殊子弹进行吞噬
            if (specialBullets.Count > 0)
            {
                // 吞噬成功减去SAN值
                float SAN = Player.GetInstance().GetProperty(E_Property.san);
                Player.GetInstance().SetProperty(E_Property.san, SAN - 10);

                E_PoolType swallowBulletType = specialBullets[0].bulletType;

                swallowBullet = PoolManager.GetInstance().GetObj(swallowBulletType).GetComponent<SpecialBullet>();



                Debug.Log("选择第一个进入吞噬范围内的特殊子弹进行吞噬");
                

                GameObject.Destroy(specialBullets[0].gameObject);


                specialBullets.RemoveAt(0);

                isSwallowed = true;
            }
        }

    }


    
    // 进入玩家本身碰撞体的范围
    public void OnCollisionEnter(Collision collision) 
    { 
        
        Debug.Log("In Player OnCollisionEnter: " + collision.collider.name);
        SpecialBullet bullet = collision.collider.gameObject.GetComponent<SpecialBullet>();
        if (bullet != null)
        {
            Debug.Log("有子弹击中了你!!!");
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
            specialBullets.Add(other.GetComponent<SpecialBullet>());
        }
        Debug.Log("In Player OnTriggerEnter: " + other.name);
    }

    // 退出吞噬范围，将退出吞噬范围内的子弹移除列表
    public void OnTriggerExit(Collider other)
    {
        SpecialBullet currentBullet = other.GetComponent<SpecialBullet>();
        if (currentBullet != null && specialBullets.Contains(currentBullet))
        {
            specialBullets.Remove(currentBullet);
        }
        Debug.Log("In Player OnTriggerExit: " + other.name);
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
