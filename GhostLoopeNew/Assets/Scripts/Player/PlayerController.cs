using Microsoft.Win32.SafeHandles;
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


    SpecialBullet swallowBullet; // �����ɵ������ӵ�
    bool isSwallowed = false;
    List<SpecialBullet> specialBullets = new List<SpecialBullet>(); // �������ɷ�Χ�ڵ������ӵ��б�

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
        Vector3 fireOrigin = transform.position + fireDirection * 10.0f;

        fireOrigin += new Vector3(0.0f, 5.0f, 0.0f);

        Bullet bullet = PoolManager.GetInstance().GetObj(E_PoolType.SimpleBullet).GetComponent<Bullet>();
        

        bullet.FireOut(fireOrigin, 
                       fireDirection,
                       GlobalSetting.GetInstance().bulletSpeed);
    }

    // �����Ѿ����ɵ������ӵ�
    private void FireSpecial(SpecialBullet bullet)
    {
        Debug.Log("FireSpecial Before");
        if(bullet == null) return;


        Debug.Log("FireSpecial End");

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
        Vector3 fireOrigin = transform.position + fireDirection * 10.0f;



        fireOrigin += new Vector3(0.0f, 5.0f, 0.0f);


        //SpecialBullet bullet = PoolManager.GetInstance().GetObj(E_PoolType.FireBullet).GetComponent<SpecialBullet>();


        //Debug.Log("Bullet: !!!!!!!!!!!!!!" + bullet);
        // Bullet bullet = PoolManager.GetInstance().GetObj(E_PoolType.SimpleBullet).GetComponent<Bullet>();

        // ֱ�ӽ�������ӵ������ȥ
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
        
        
        // ����Ѿ������������ӵ��������
        if (isSwallowed)
        {
            if (swallowBullet == null) 
            {
                isSwallowed = false;
            }
            isSwallowed = false;
            Debug.Log("����Ѿ����ɵ������ӵ�");
            Debug.Log("swallowBullet.bulletType: " + swallowBullet.bulletType);
            FireSpecial(swallowBullet);
            //swallowBullet = null;
            //GameObject.Destroy(swallowBullet);
        }
        // ���򣬽��������ж�
        else
        {
            // ѡ���һ���������ɷ�Χ�ڵ������ӵ���������
            if (specialBullets.Count > 0)
            {
                // ���ɳɹ���ȥSANֵ
                float SAN = Player.GetInstance().GetProperty(E_Property.san);
                float RES = Player.GetInstance().GetProperty(E_Property.resilience);

                Player.GetInstance().SetProperty(E_Property.san, SAN - 10);
                Player.GetInstance().SetProperty(E_Property.resilience, RES + 10);



                E_PoolType swallowBulletType = specialBullets[0].bulletType;

                swallowBullet = PoolManager.GetInstance().GetObj(swallowBulletType).GetComponent<SpecialBullet>();

                swallowBullet.bulletType = swallowBulletType;

                Debug.Log("ѡ���һ���������ɷ�Χ�ڵ������ӵ���������");



                GameObject.Destroy(specialBullets[0].gameObject);


                specialBullets.RemoveAt(0);

                isSwallowed = true;
            }
        }

    }


    
    // ������ұ�����ײ��ķ�Χ
    public void OnCollisionEnter(Collision collision) 
    { 
        
        Debug.Log("In Player OnCollisionEnter: " + collision.collider.name);
        SpecialBullet bullet = collision.collider.gameObject.GetComponent<SpecialBullet>();
        if (bullet != null)
        {
            Debug.Log("���ӵ���������!!!");
            EventCenter.GetInstance().EventTrigger<SpecialBullet>(E_Event.PlayerReceiveDamage, bullet);


            PoolManager.GetInstance().ReturnObj(bullet.bulletType, bullet.gameObject);
        }

    }
    // �������ɷ�Χ, ���������ɷ�Χ�ڵ��ӵ������б�d
    public void OnTriggerEnter(Collider other)
    {
        SpecialBullet currentBullet = other.GetComponent<SpecialBullet>();
        if (currentBullet != null)
        {
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!In Player OnTriggerEnter: " + other.name);


            Debug.Log(other.gameObject.GetComponent<SpecialBullet>().bulletType);
            specialBullets.Add(other.GetComponent<SpecialBullet>());
        }
    }

    // �˳����ɷ�Χ�����˳����ɷ�Χ�ڵ��ӵ��Ƴ��б�
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
