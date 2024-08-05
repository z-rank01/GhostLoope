using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : BaseSingletonMono<Player>
{
    PlayerProperty playerProperty;
    PlayerStatus playerStatus;
    PlayerController playerController;
    PlayerAnimator_GhostLoope playerAnimator;

    [SerializeField]
    private float gunHeat;  // ��ͨ�����ȴʱ��
    private float currGunHeat = 0;  //  ������һ�ι�������ȴʱ�䣬��� <= 0�����dash

    [SerializeField]
    private float dashTime = 1f; // dash����ȴʱ��
    private float currDashTime = 0f; // ������һ��dash����ȴʱ�䣬��� <= 0�����dash

    [SerializeField]
    private float swallowTime = 1.0f; //���ɼ��ܵ���ȴʱ��
    private float curSwallowTime = 0.0f; // ������һ�����ɵ���ȴʱ�䣬��� <= 0���������

    [SerializeField]
    private float interactTime;  // ��ͨ�����ȴʱ��
    private float currinteractTime = 0;  //  ������һ�ι�������ȴʱ�䣬��� <= 0�����dash




    

    

    public void Awake()
    {
        

        Init();
        // Mono
        playerProperty = gameObject.AddComponent<PlayerProperty>();
        playerController = gameObject.AddComponent<PlayerController>();
        playerAnimator = gameObject.AddComponent<PlayerAnimator_GhostLoope>();

        // not Mono
        playerStatus = new PlayerStatus();
        playerStatus.Init();


        // ������Һ����Ӧ�¼�
        EventCenter.GetInstance().AddEventListener<SpecialBullet>(E_Event.PlayerReceiveDamage, PlayerReceiveDamage);
        // MusicManager.GetInstance().PlayBkMusic("TestMusic");
    }


    public void Update()
    {


        // �������
        float SAN = GetProperty(E_Property.san);
        if (SAN <= 0)
        {
            Destroy(gameObject);
        }




        // Update coldDown counter
        if (currDashTime > 0) currDashTime -= Time.deltaTime;
        if (currGunHeat > 0) currGunHeat -= Time.deltaTime;
        if(curSwallowTime > 0)curSwallowTime -= Time.deltaTime;
        if (currinteractTime > 0) currinteractTime -= Time.deltaTime;


       






        // Check action
        if (ContainStatus(E_InputStatus.moving))
        {
            playerController.Act(E_InputStatus.moving);
            playerAnimator.Move();
        }
        else playerAnimator.Idle();

        if (ContainStatus(E_InputStatus.firing))
        {

            // �ж�����Ƿ�ŵ���UI��
            bool isMouseOnUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

            if (isMouseOnUI) // ����Ƿ���UI
            {
                return;
            }

            Debug.Log("Mouse is clicked on : " + isMouseOnUI);


            if (CheckFire())
            {
                currGunHeat = gunHeat;
                playerController.Act(E_InputStatus.firing);
                playerAnimator.Attack();
            }
                
        }
        else playerAnimator.ClearAttack();

        if (ContainStatus(E_InputStatus.interacting))
        {
            if (CheckInteract())
            {
                currinteractTime = interactTime;
                playerController.Act(E_InputStatus.interacting);
                playerAnimator.TakeDamage();
            }
        }
        else playerAnimator.ClearTakeDamage();

        if (ContainStatus(E_InputStatus.swallowingAndFiring))
        {
            if (CheckSwallowAndFire())
            {
                curSwallowTime = swallowTime;
                playerController.Act(E_InputStatus.swallowingAndFiring);
                playerAnimator.TakeDamage();
            }
        }
        else playerAnimator.ClearTakeDamage();

        if (ContainStatus(E_InputStatus.dashing))
        {
            if (CheckDash())
            {
                currDashTime = dashTime;


                float res = playerProperty.GetProperty(E_Property.resilience);
                playerProperty.SetProperty(E_Property.resilience, res - 10);

                playerController.Act(E_InputStatus.dashing);
                playerAnimator.Dash();
            }
        }
        else playerAnimator.ClearDash();

        TowardMouseDirection();
        //SetForwardDirection();
    }

    private void SetForwardDirection()
    {
        // Get mouse world direction
        Vector3 screenWorldPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 mouseScreenPostion = Mouse.current.position.ReadValue();
        mouseScreenPostion.z = screenWorldPos.z;
        Vector3 mouseWorldPostion = Camera.main.ScreenToWorldPoint(mouseScreenPostion);
        mouseWorldPostion.y = 0;

        // Set mouse direction
        Vector3 mouseDirection = mouseWorldPostion - transform.position;
        //mouseDirection = Vector3.Normalize(mouseDirection);
        //mouseDirection.y = 0;
        transform.forward = mouseWorldPostion.normalized;
    }

    private void TowardMouseDirection()
    {
        // Get mouse world direction
        Vector3 screenWorldPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 mouseScreenPostion = Mouse.current.position.ReadValue();
        mouseScreenPostion.z = screenWorldPos.z;
        Vector3 mouseWorldPostion = Camera.main.ScreenToWorldPoint(mouseScreenPostion);
        mouseWorldPostion.y = transform.position.y;
        // Set mouse direction
        Vector3 mouseDirection = mouseWorldPostion - transform.position;
        mouseDirection = Vector3.Normalize(mouseDirection);
        
        transform.LookAt(mouseWorldPostion);
        Debug.Log("Transform looking at: " + mouseWorldPostion);
        //transform.Rotate(new Vector3(0, -90, 0));
    }

    
    private bool CheckFire()
    {
        if (currGunHeat <= 0) return true;
        return false;
    }


    private bool CheckDash()
    {
        if (currDashTime <= 0 && playerProperty.GetProperty(E_Property.resilience) >= 10) return true;
        return false;
    }

    private bool CheckSwallowAndFire()
    {
        return curSwallowTime <= 0;
    }

    private bool CheckInteract()
    {
        if (currinteractTime <= 0) return true;
        return false;
    }


    // ÿdeltaTime�ܵ�һ���˺���ÿ���˺�ΪextraDamage�����ܵ�count���˺�
    IEnumerator ReceiveExtraDamage(float deltaTime, float extraDamage, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float san = playerProperty.GetProperty(E_Property.san);
            playerProperty.SetProperty(E_Property.san, san - extraDamage);

            // �������ܵ�һ�ζ����˺���Ȼ��ȴ�deltaTime
            yield return new WaitForSeconds(deltaTime);
        }
    }



    // ����ܵ�ʱ��ΪdeltaTime�ļ���Ч��
    IEnumerator ReceiveIceEffect(float deltaTime)
    {
        playerController.SetSlowSpeed();
        yield return new WaitForSeconds(deltaTime);
        playerController.SetNormalSpeed();
    }

    IEnumerator ReceiveSpiritPosionEffect(float deltaTime)
    {
        playerController.SetIsSpiritPosioned(true);
        yield return new WaitForSeconds(deltaTime);
        playerController.SetIsSpiritPosioned(false);
    }


    public void PlayerReceiveDamage(SpecialBullet bullet)
    {
        Debug.Log("In PlayerReceiveDamage + bullet.type: " + bullet.bulletType + bullet.damage);

        playerProperty.SetProperty(E_Property.san, playerProperty.GetProperty(E_Property.san) - bullet.damage);



        switch (bullet.bulletType)
        {
            case E_PoolType.FireBullet:
                break;
            case E_PoolType.ThunderBullet:


                StartCoroutine(ReceiveExtraDamage(0, bullet.extraDamage, 1));


                break;
            case E_PoolType.ExplodeBullet:
                break;
            case E_PoolType.BurnBullet:

                StartCoroutine(ReceiveExtraDamage(0.5f, 5.0f, 6));

                break;
            case E_PoolType.IceBullet: // �����ļ���Ч����3�����ʧ
                StartCoroutine(ReceiveIceEffect(3.0f));
                break;

            case E_PoolType.PoisonBullet:

                
                StartCoroutine(ReceiveExtraDamage(1.0f, 3.0f, 10));


                break;
            case E_PoolType.SpiritPoisonBullet:

                StartCoroutine(ReceiveSpiritPosionEffect(3.0f));
                break;
        }
    }






    // interface

    // property control
    public void SetProperty(E_Property eProperty, object property)
    {
        playerProperty.SetProperty(eProperty, property);
    }

    public float GetProperty(E_Property eProperty)
    {
        return playerProperty.GetProperty(eProperty);
    }

    // status control
    public void AddStatus(E_InputStatus inputStatus)
    {
        playerStatus.AddStatus(inputStatus);
    }

    public bool ContainStatus(E_InputStatus inputStatus)
    {
        return playerStatus.ContainStatus(inputStatus);
    }

    public void ClearStatus()
    {
        playerStatus.ClearStatus();
    }

    public void ClearStatus(E_InputStatus inputStatus)
    {
        playerStatus.ClearStatus(inputStatus);
    }
}
