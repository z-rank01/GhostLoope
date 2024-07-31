using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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


    SpecialBullet swallowBullet = null; // �����ɵ������ӵ�

    List<SpecialBullet> specialBullets = new List<SpecialBullet>(); // �������ɷ�Χ�ڵ������ӵ��б�

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
        Vector3 screenWorldPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 mouseScreenPostion = Mouse.current.position.ReadValue();
        mouseScreenPostion.z = screenWorldPos.z;
        Vector3 mouseWorldPostion = Camera.main.ScreenToWorldPoint(mouseScreenPostion);

        // Set fire direction
        fireDirection = new Vector3(mouseWorldPostion.x, 0, mouseWorldPostion.z) - transform.position;
        fireDirection = Vector3.Normalize(fireDirection);

        // Set fire origin
        Vector3 fireOrigin = new Vector3(transform.position.x, 0.1f, transform.position.z) + fireDirection * 2.0f;

        Bullet bullet = PoolManager.GetInstance().GetObj(E_PoolType.SimpleBullet).GetComponent<Bullet>();
        bullet.FireOut(fireOrigin, 
                       fireDirection, 
                       ActInfo.GetInstance().bulletSpeed);
    }

    private void Interact()
    {
        Debug.Log("Interact");
        EventCenter.GetInstance().EventTrigger(E_Event.Conversation, null);
    }

    private void SwallowAndFire()
    {
        Debug.Log("SwallowAndFire");
        float SAN = Player.GetInstance().GetProperty(E_Property.san);
        Player.GetInstance().SetProperty(E_Property.san, SAN - 10);
        
        // ����Ѿ������������ӵ��������
        if (swallowBullet != null)
        {
            Debug.Log("����Ѿ����ɵ������ӵ�");

            swallowBullet = null;
        }
        // ���򣬽��������ж�
        else
        {
            // ѡ���һ���������ɷ�Χ�ڵ������ӵ���������
            if (specialBullets.Count > 0)
            {
                Debug.Log("ѡ���һ���������ɷ�Χ�ڵ������ӵ���������");
                swallowBullet = specialBullets[0];
                specialBullets.RemoveAt(0);
            }
        }

    }


    
    // ������ұ�����ײ��ķ�Χ
    public void OnCollisionEnter(Collision collision) 
    { 
        
        Debug.Log("In Player OnCollisionEnter: " + collision.collider.name);
    }
    // �������ɷ�Χ, ���������ɷ�Χ�ڵ��ӵ������б�
    public void OnTriggerEnter(Collider other)
    {
        SpecialBullet currentBullet = other.GetComponent<SpecialBullet>();
        if (currentBullet != null)
        {
            specialBullets.Add(other.GetComponent<SpecialBullet>());
        }
        Debug.Log("In Player OnTriggerEnter: " + other.name);
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
