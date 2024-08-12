using Cinemachine.Utility;
using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
        Vector3 fireOrigin = transform.position + fireDirection * 1.0f;

        fireOrigin += new Vector3(0.0f, 1.0f, 0.0f);


        //Debug.Log("FireOrigin: " + fireOrigin);

       
        Debug.Log("发射普通子弹");

        Bullet bullet = PoolManager.GetInstance().GetObj(E_PoolType.SimpleBullet).GetComponent<Bullet>();

        


        bullet.FireOut(fireOrigin, 
                        fireDirection,
                        GlobalSetting.GetInstance().bulletSpeed);


        // 10连发，测试用
        for (int i = 0; i < 10; i++)
        {
            Bullet bullet1 = PoolManager.GetInstance().GetObj(E_PoolType.SimpleBullet).GetComponent<Bullet>();




            bullet1.FireOut(fireOrigin,
                            fireDirection,
                            GlobalSetting.GetInstance().bulletSpeed);
        }


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

    IEnumerator PlayParticle(ParticleSystem particle, float deltaTime)
    {
        particle.Play();
        yield return new WaitForSeconds(deltaTime);
        particle.Stop();
    }
    // 进入玩家的碰撞范围, 玩家收到伤害
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("In Player OnTriggerEnter: " + other);
        Debug.Log("other.tag: " + other.tag);
        if (other.tag == "SavePoint")
        {
            Debug.Log(SceneManager.GetActiveScene().name);

            // 直接获取保存点的子物体的粒子特效
            if (other.transform.childCount != 0)
            {
                ParticleSystem particle = other.transform.GetChild(0).GetComponent<ParticleSystem>();
                if (particle != null)
                {
                    StartCoroutine(PlayParticle(particle, 1.0f));
                }
            }
            SaveManager.GetInstance().SaveGame();
        }



        SpecialBullet bullet = other.GetComponent<SpecialBullet>();
        if (bullet != null && bullet.isSwallowed == false)
        {
            swallowRange.RemoveBulleet(bullet);
            Debug.Log("有子弹击中了你!!!");
            Player.GetInstance().PlayerReceiveDamage(bullet);
            // 场景切换时用单例好麻烦~
            //EventCenter.GetInstance().EventTrigger<SpecialBullet>(E_Event.PlayerReceiveDamage, bullet);
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
