using BehaviorDesigner.Runtime.Tasks.Unity.UnityPlayerPrefs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : BaseSingletonMono<Player>
{
    PlayerProperty playerProperty;
    PlayerStatus playerStatus;
    PlayerController playerController;
    PlayerAnimator playerAnimator;

    [SerializeField]
    private float gunHeat;  // ��ͨ�����ȴʱ��
    private float currGunHeat = 0;  //  ������һ�ι�������ȴʱ�䣬��� <= 0��������

    [SerializeField]
    private float interactTime;  // ������ȴʱ��
    private float currinteractTime = 0;  //  ������һ�ν�������ȴʱ�䣬��� <= 0����Խ���

    [SerializeField]
    private float dashTime = 1f; // ��̵���ȴʱ��
    private float currDashTime = 0f; // ������һ�γ�̵���ȴʱ�䣬��� <= 0����Գ��

    [SerializeField]
    private float dashDecreaseRes = 10.0f; // �����Ҫ���ĵ�Resֵ

    [SerializeField]
    private float swallowTime = 1.0f; //���ɼ��ܵ���ȴʱ��
    private float curSwallowTime = 0.0f; // ������һ�����ɵ���ȴʱ�䣬��� <= 0���������

    [SerializeField]
    private float swallowRadius = 2.0f; // ������ɼ��ܵİ뾶

    [SerializeField]
    private float swallowDecreaseSan = 10.0f; // ���ɳɹ����ĵ�Sanֵ

    [SerializeField]
    private float swallowIncreaseRes = 10.0f; // ���ɳɹ����ӵ�Resֵ


    [SerializeField]
    private float HealingTime; // ����HealingTimeʱ��û���յ��˺������Ǳ���Ѫ

    [SerializeField]
    private float RecoverRatio; // ��һ�Ѫ�����ʣ�ÿ��ָ�recoverRatio������ֵ


    public float GetSwallowDecreaseSan() { return swallowDecreaseSan; }
    public float GetSwallowIncreaseRes() { return swallowIncreaseRes; }

    public float GetSwallowRadius() { return swallowRadius; }

    private bool isGettingHurt = false; // ����Ƿ����ܵ��˺�
    private bool isFightingBoss = false; // ����Ƿ�����Bossս

    private bool isGettingSoul_1 = false; // �Ƿ�õ��˵�һ��Boss�����
    private bool isGettingSoul_2 = false; // �Ƿ�õ��˵ڶ���Boss�����


    private bool isGameEnd = false; // ��Ϸ�Ƿ����

    public bool isNeedToShowText = false; // �Ƿ���Ҫ��ʾ�����ı�
    public string showText; // ����ʾ�������ı�
    public void SetIsGameEnd(bool value) { isGameEnd = value; }
    public bool GetIsGameEnd() { return isGameEnd; }
    public void SetSoul_1(bool value) { isGettingSoul_1 = value; }
    public void SetSoul_2(bool value) { isGettingSoul_2 = value; }
    public bool GetSoul_1() { return isGettingSoul_1; }
    public bool GetSoul_2() { return isGettingSoul_2; }

    public NavMeshSurface surface;
    public void SetIsFightingBoss(bool value) 
    { 
        isFightingBoss = value; 
        

        if (isFightingBoss)
        {
            MusicManager.GetInstance().PlayFireSound("Bossս����");
        }
    }
    public bool GetIsFightingBoss() { return isFightingBoss; }
    
    // ���ֻҪ�ܵ��˺����ͽ�isGettingHurt��Ϊtrue������ʱ��ΪHealingTime�Ķ�ʱ������ʱ��������isGettingHurt��Ϊfalse
    public IEnumerator GettingHurt()
    {
        isGettingHurt = true;
        //Debug.Log("GettingHurt Begin " + HealingTime + " " + isGettingHurt);
        // �������ܵ�һ�ζ����˺���Ȼ��ȴ�deltaTime
        yield return new WaitForSeconds(HealingTime);

        //Debug.Log("GettingHurt End");
        isGettingHurt = false;
    }



    public void SetTransformPosition(Vector3 position)
    {
        transform.position = position;
    }
    public void Awake()
    {

        


        Init();
        // Mono
        playerProperty = gameObject.AddComponent<PlayerProperty>();
        playerController = gameObject.AddComponent<PlayerController>();
        playerAnimator = gameObject.AddComponent<PlayerAnimator>();

        // not Mono
        playerStatus = new PlayerStatus();
        playerStatus.Init();


        // ������Һ����Ӧ�¼�
        EventCenter.GetInstance().AddEventListener<SpecialBullet>(E_Event.PlayerReceiveDamage, PlayerReceiveDamage);



        // ��ԭ����
        if (SaveManager.GetInstance().loading)
        {
            LoadSceneInfo();
            SaveManager.GetInstance().loading = false;
        }


        Debug.Log("LoadSceneInfo: " + SaveManager.GetInstance().san);
        GlobalSetting.GetInstance().san = SaveManager.GetInstance().san;
        GlobalSetting.GetInstance().resilience = SaveManager.GetInstance().res;

        SetProperty(E_Property.san, GlobalSetting.GetInstance().san);
        SetProperty(E_Property.resilience, 0.0f); // ���Ĭ�ϵ�����ֵΪ0

        isGettingSoul_1 = SaveManager.GetInstance().is_Soul1;
        isGettingSoul_2 = SaveManager.GetInstance().is_Soul2;


        string levelName = SceneManager.GetActiveScene().name;

        if (levelName == "Level1")
        {
            MusicManager.GetInstance().PlayBackgroundMusic("��һ��-����");
            MusicManager.GetInstance().PlayEnvironmentSound("��һ��-������-΢��");
        }
        else if (levelName == "Level2")
        {
            MusicManager.GetInstance().PlayBackgroundMusic("�ڶ���-����");
            MusicManager.GetInstance().PlayEnvironmentSound("�ڶ���-������-С��");
        }
        else if (levelName == "Level3")
        {
            MusicManager.GetInstance().PlayBackgroundMusic("������-����");
            MusicManager.GetInstance().PlayEnvironmentSound("������-������-ɭ��");
        }

    }

    public void LoadSceneInfo()
    {
        // position
        transform.position = new Vector3(SaveManager.GetInstance().x, SaveManager.GetInstance().y, SaveManager.GetInstance().z);


        

        Debug.Log("Player Loading");
        Debug.Log(SaveManager.GetInstance().GraveStoneId.Count);
        // gravestone
        GameObject[] currentGraveStone = GameObject.FindGameObjectsWithTag("GraveStone");
        for (int i = 0; i < currentGraveStone.Length; i++)
        {
            Debug.Log("StoneID: " + currentGraveStone[i].GetInstanceID());
            if (!SaveManager.GetInstance().GraveStoneId.Contains(currentGraveStone[i].GetComponent<GraveStone>().id))
            {
                Destroy(currentGraveStone[i]);
            }
        }

        // spawnEnemyPoint
        GameObject[] currentSpawnEnemy = GameObject.FindGameObjectsWithTag("SpawnEnemy");
        for (int i = 0; i < currentSpawnEnemy.Length; i++)
        {
            if (!SaveManager.GetInstance().SpawnEnemyId.Contains(currentSpawnEnemy[i].GetComponent<SpawnEnemy>().id))
            {
                Destroy(currentSpawnEnemy[i]);
            }
        }

        // computer(the other spawnEnemyPoint)
        GameObject[] Computer = GameObject.FindGameObjectsWithTag("computer");
        bool flag = false; // �ù������ɵ��ڱ����ļ����Ƿ񻹴���
        for (int i = 0; i < Computer.Length; i++)
        {
            if (Computer[i].GetComponent<SpawnEnemy>() != null)
            {
                if (!SaveManager.GetInstance().ComputerId.Contains(Computer[i].GetComponent<SpawnEnemy>().id))
                {
                    flag = true;
                    break;
                }
            }
        }
        // Ҫ�ݻ���������
        if (flag)
        {
            for (int i = 0; i < Computer.Length; i++)
            {
                Destroy(Computer[i]);
            }
        }
    }

    public void Update()
    {
        // �����������ص���һ���浵��
        if (playerProperty.GetProperty(E_Property.san) <= 0)
        {
            //SaveManager.GetInstance().SaveGame();
            EventCenter.GetInstance().Clear();
            SaveManager.GetInstance().LoadGame();
            return;
        }
        //Debug.Log("Player.Transform: " + transform.position);
        // ��Ѫ
        float san = playerProperty.GetProperty(E_Property.san);
        float maxSan = GlobalSetting.GetInstance().san;
        if (san < maxSan && isGettingHurt == false)
        {
            //Debug.Log("Healinginging");
            playerProperty.SetProperty(E_Property.san, san + Time.deltaTime * RecoverRatio);
        }



        // per frame updating
        TowardMouseDirection();
        CheckPlayerSan();

        // Update coldDown counter
        if (currDashTime > 0) currDashTime -= Time.deltaTime;
        if (currGunHeat > 0) currGunHeat -= Time.deltaTime;
        if(curSwallowTime > 0)curSwallowTime -= Time.deltaTime;
        if (currinteractTime > 0) currinteractTime -= Time.deltaTime;

        // Check move action
        if (ContainStatus(E_InputStatus.moving))
        {
            playerController.Act(E_InputStatus.moving);
            playerAnimator.Move();
        }
        else playerAnimator.Idle();

        // Check fire action
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

        // Check interact action
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

        // Check special action
        if (ContainStatus(E_InputStatus.swallowingAndFiring))
        {
            if (CheckSwallow())
            {
                curSwallowTime = swallowTime;
                playerController.Act(E_InputStatus.swallowingAndFiring);
                playerAnimator.TakeDamage();
            }
        }
        else playerAnimator.ClearTakeDamage();

        // Check dash action
        if (ContainStatus(E_InputStatus.dashing))
        {
            if (CheckDash())
            {
                currDashTime = dashTime;

                float res = playerProperty.GetProperty(E_Property.resilience);
                playerProperty.SetProperty(E_Property.resilience, res - dashDecreaseRes);

                playerController.Act(E_InputStatus.dashing);
                playerAnimator.Dash();
            }
        }
        else playerAnimator.ClearDash();

        // Check dead action
        if (ContainStatus(E_InputStatus.die))
        {
            playerAnimator.Die();
            playerController.Act(E_InputStatus.die);
        }

        //Debug.Log("Build NavMeshSurface");
        //surface.BuildNavMesh();
    }

    // per frame update function

    // update player's forward direction
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
    }

    // check player's property and colddown counter
    private void CheckPlayerSan()
    {
        if (playerProperty.GetProperty(E_Property.san) <= 0)
            AddStatus(E_InputStatus.die);
    }

    
    private bool CheckFire()
    {
        if (currGunHeat <= 0) return true;
        return false;
    }

    private bool CheckDash()
    {
        if (currDashTime <= 0 && playerProperty.GetProperty(E_Property.resilience) >= dashDecreaseRes) return true;
        return false;
    }

    private bool CheckSwallow()
    {
        return curSwallowTime <= 0;
    }

    private bool CheckInteract()
    {
        if (currinteractTime <= 0) return true;
        return false;
    }

    // Damage Receiver

    // ÿdeltaTime�ܵ�һ���˺���ÿ���˺�ΪextraDamage�����ܵ�count���˺�
    IEnumerator ReceiveExtraDamage(float deltaTime, float extraDamage, int count)
    {
        for (int i = 0; i < count; i++)
        {
            isGettingHurt = true; // ������ʱ��̫���ˣ���Ҫ�ֶ���Ǹñ���

            float san = playerProperty.GetProperty(E_Property.san);
            playerProperty.SetProperty(E_Property.san, san - extraDamage);

            // ���һ���ܵ��˺�����Ҫ�ٵȴ������������ʱHealingTime
            if (i != count - 1)
            {
                // �������ܵ�һ�ζ����˺���Ȼ��ȴ�deltaTime
                yield return new WaitForSeconds(deltaTime);
            }
        }


        Debug.Log("�����˺�����!");

        StartCoroutine(GettingHurt()); // ����ʱ��ΪHealingTime�Ķ�ʱ������ʱ��������ʼ��Ѫ
    }

    // ����ܵ�ʱ��ΪdeltaTime�ļ���Ч��
    IEnumerator ReceiveIceEffect(float deltaTime)
    {
        playerController.SetSlowSpeed();
        yield return new WaitForSeconds(deltaTime);
        playerController.SetNormalSpeed();
    }
    // ����ܵ�ʱ��ΪdeltaTime�ļ�λ����Ч��
    IEnumerator ReceiveSpiritPosionEffect(float deltaTime)
    {
        playerController.SetIsSpiritPosioned(true);
        yield return new WaitForSeconds(deltaTime);
        playerController.SetIsSpiritPosioned(false);
    }

    IEnumerator ReceiveExplodeEffect(float damage)
    {
        float san = playerProperty.GetProperty(E_Property.san);
        playerProperty.SetProperty(E_Property.san, san - damage);
        yield return null;
    }

    public void PlayerReceiveDamage(SpecialBullet bullet)
    {
        StartCoroutine(GettingHurt()); // ���Ϊ�����ܵ��˺�״̬

        //Debug.LogWarning("In PlayerReceiveDamage + bullet.type: " + bullet.bulletType + bullet.damage);

        playerProperty.SetProperty(E_Property.san, playerProperty.GetProperty(E_Property.san) - bullet.damage);
        switch (bullet.bulletType)
        {
            case E_PoolType.FireBullet:
                break;
            case E_PoolType.ThunderBullet:
                StartCoroutine(ReceiveExtraDamage(0, bullet.extraDamage, 1));
                break;
            case E_PoolType.ExplodeBullet:
                StartCoroutine(ReceiveExplodeEffect(bullet.extraDamage));
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

    public void PlayerReceiveDamage(float damage)
    {
       // Debug.LogWarning("In PlayerReceiveDamage receive other damage: " + damage);
        float san = playerProperty.GetProperty(E_Property.san);
        playerProperty.SetProperty(E_Property.san, san - damage);
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

    // player info
    public Transform GetPlayerTransform()
    {
        return this.transform;
    }
}
