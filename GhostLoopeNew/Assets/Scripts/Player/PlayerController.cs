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

    bool isSpiritPosioned = false; // ����Ƿ��ܵ��������˺�


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
            Debug.Log("���������ӵ�");
            swallowRange.FireSpecial();
            return;
        }

        Debug.Log("Fire");

        Debug.Log("������ͨ�ӵ�");




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




       
        Debug.Log("������ͨ�ӵ�");
        Bullet bullet = PoolManager.GetInstance().GetObj(E_PoolType.SimpleBullet).GetComponent<Bullet>();
        
        bullet.FireOut(fireOrigin, 
                        fireDirection,
                        GlobalSetting.GetInstance().bulletSpeed);





        MusicManager.GetInstance().PlayFireSound("����-�չ�-���"); // ����ӵ���Ч

        MusicManager.GetInstance().PlayFireSound("����-�չ�-����"); // ����ӵ���Ч
    }


    private void Interact()
    {
        Debug.Log("Interact");
        EventCenter.GetInstance().EventTrigger(E_Event.Conversation);
    }


    // �Ҽ�����
    private void Swallow()
    {
        Debug.Log("SwallowAndFire");

        // ���û�������ӵ������������
        if (!swallowRange.ReadyToFire())
        {
            swallowRange.SwallowBullet();
        }




        //// ����Ѿ������������ӵ��������
        //if (swallowRange.ReadyToFire())
        //{
        //    //Debug.Log("����Ѿ����ɵ������ӵ�");
        //    swallowRange.FireSpecial();
        //}
        //// ���򣬽��������ж�
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


    // ������ҵ���ײ��Χ, ����յ��˺�
    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("In PlayerController OnTriggerEnter~~~~~~~~~~~~~~~~~~~~~~~");

        SpecialBullet bullet = other.GetComponent<SpecialBullet>();
        if (bullet != null && bullet.isSwallowed == false)
        {
            swallowRange.RemoveBulleet(bullet);
            Debug.Log("���ӵ���������!!!");
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
