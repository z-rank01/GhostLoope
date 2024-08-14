using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class SpawnEnemy : MonoBehaviour
{
    // HP
    public float hp = 100.0f; // ���ɵ��HP
    public float spawnAmountHp = 10.0f; //�ٻ�һ�ι�������Ҫ��HP

    // AI
    //public NavMeshSurface navMeshSurface; // ��ȡ�������еĵ������񣬶�̬���ɹ�������º決

    // UI
    public Slider spawnHp;
    public Vector3 spawnHpOffset;
    public HintUI ui;

    // Spawn Enemy
    public List<E_PoolType> enemySpawnType = new List<E_PoolType>(); // Ҫ���ɹ��������
    public float alertDistance = 8.0f; // ���ɵ�����ҵľ���С�ڸ�ֵ�󷽿��ٻ�С��
    public int spawnNum = 4; // ���ɹ��������
    public float spawnRadius = 2.0f; // ���ɹ��������ɵ�İ뾶
    public float spawnTime = 5.0f; // �ٻ�����Ĺ̶����
    private float curSpawnTime = 0.0f; // �ٻ�����ļ�ʱ����<=0 ������ٻ�
    private List<GameObject> EnemyList = new List<GameObject>(); // ���С�ֵ�����


    public Transform enemySpawnPoint;
    public Transform bossSpawnPoint; // �ֶ�ָ��Boss���ɵ�

    public int id; // �浵�õĸ������id

    // Ŀǰ����С������
    public int CheckEnemyNumber()
    {
        int count = 0;
        for (int i = 0; i < EnemyList.Count; i++)
        {
            if (EnemyList[i].activeSelf == true)
            {
                count++;
            }
        }
        return count;
    }

    public void OnTriggerEnter(Collider other)
    {

        Debug.Log("OnTriggerEnter: SpawnEnemy:");
        Bullet bullet = other.gameObject.GetComponent<Bullet>();
        // ������С��ʱ��������˺�
        if (bullet != null && CheckEnemyNumber() == 0)
        {
            hp -= bullet.playerDamage;
            PoolManager.GetInstance().ReturnObj(bullet.bulletType, bullet.gameObject);
        }
    }
    void Start()
    {
        //spawnHp = GameObject.Find("SpawnHp1").GetComponent<Slider>();

        if (spawnHp != null)
        {
            ui = spawnHp.AddComponent<HintUI>();
            ui.SetCameraAndFollowingTarget(Camera.main, transform);
            spawnHp.maxValue = hp;
            spawnHp.value = hp;
        }
    }
    // �ٻ�һ��С��
    public void SpawnEnemyRound()
    {
       
        if (enemySpawnType.Count == 0) return;
        for (int i = 0; i < spawnNum; i++)
        {
            // Random.Range()����[l,r)��ֵ!!!
            int EnemyType = Random.Range(0, enemySpawnType.Count);
            //Debug.Log("EnemyType: " + EnemyType);
            // �����������
            GameObject spawnEnemy = PoolManager.GetInstance().GetObj(enemySpawnType[EnemyType]);
            spawnEnemy.name = "spawnEnemy" + i;

            spawnEnemy.transform.SetParent(enemySpawnPoint, true);

            



            Debug.Log("After Set Parent : " + spawnEnemy.transform.localPosition);
            // ��λԲ�����һ��ĽǶ�
            float theta = Random.Range(0.0f, Mathf.PI * 2);
            // ��λԲ�����һ��
            Vector3 spawnPosition = new Vector3(Mathf.Sin(theta), 0, Mathf.Cos(theta));
            Debug.Log("SpawnPosition Normal: " + spawnPosition);
            spawnPosition *= spawnRadius;

            // ���ù���ĳ����㣨����ڸ����壩
            spawnEnemy.transform.SetLocalPositionAndRotation(spawnPosition, Quaternion.identity);

            //Debug.Log("i : " + i);
            //Debug.Log("SpawnPosition * Radius: " + spawnPosition);
            //Debug.Log("spawnEnemy.transform.position: " + spawnEnemy.transform.position);
            //Debug.Log("spawnEnemy.transform.localPosition: " + spawnEnemy.transform.localPosition);

            float enemyDis = (spawnPosition).magnitude;
            //Debug.Log("enemyDis: " + enemyDis);



            EnemyMob enemy = spawnEnemy.GetComponent<EnemyMob>();
            enemy.isNeedRedHp = false;
            // �ֶ�����AI���
            if (enemy.isNeedAIAgent == false)
            {
                enemy.enemyAgent = enemy.AddComponent<NavMeshAgent>();
                enemy.enemyAgent.stoppingDistance = 2.0f;
            }
            

            if(GameObject.Find("SpawnEnemyHP" + i))
            {
                if (enemy.enemyHp != null)
                {
                    enemy.enemyHp.GetComponent<HintUI>().SetCameraAndFollowingTarget(Camera.main, null);
                    enemy.enemyHp.GetComponent<HintUI>().SetOffset(Vector3.zero);
                }
                enemy.enemyHp = GameObject.Find("SpawnEnemyHP" + i).GetComponent<Slider>();
                enemy.enemyHp.maxValue = enemy.maxHp;
                enemy.enemyHp.value = enemy.maxHp;
                enemy.id = 9999999;

                if (enemy.enemyHp.GetComponent<HintUI>() == null)
                {
                    enemy.enemyHp.AddComponent<HintUI>();
                }

                enemy.enemyHp.GetComponent<HintUI>().SetCameraAndFollowingTarget(Camera.main, enemy.transform);
                enemy.enemyHp.GetComponent<HintUI>().SetOffset(Vector3.zero);

                EnemyList.Add(spawnEnemy);
            }

        }

        // ���º決��������ʹ�ù����ϵ�AI�����������

        //navMeshSurface.BuildNavMesh();
    }


    // �ٻ�Boss
    public void spawnBossPosionBomb()
    {
        GameObject spawnEnemy = PoolManager.GetInstance().GetObj(E_PoolType.BossPoisonBomb1);
        spawnEnemy.name = "BossPoisonBomb";

        
        //spawnEnemy.transform.SetParent(bossSpawnPoint, true);

        //// ���ù���ĳ����㣨����ڸ����壩w
        //spawnEnemy.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);


        spawnEnemy.GetComponent<Enemy>().enemyHp.GetComponent<HintUI>().SetCameraAndFollowingTarget(Camera.main, null);


        //navMeshSurface.BuildNavMesh();

        // ����Bossս״̬����ʾBossѪ��
        Player.GetInstance().SetIsFightingBoss(true);

    }
    void Update()
    {
        ui.SetOffset(spawnHpOffset);
        spawnHp.value = hp;
        if (hp <= 0)
        {
            // һ��
            spawnHp.gameObject.SetActive(false); // ��ʧ�����ٻ����Ѫ��
            //for (int i = 0; i < EnemyList.Count; i++)
            //{
            //    // ��ʧ��С�ֵ�Ѫ��
            //    //EnemyList[i].GetComponent<Enemy>().enemyHp.GetComponent<HintUI>().offset = new Vector3(0, 1000000000, 0);
            //    //EnemyList[i].GetComponent<Enemy>().enemyHp.GetComponent<HintUI>().transform.position = new Vector3(0, 1000000000, 0); 
            //}
            if (gameObject.tag == "computer")
            {
                spawnBossPosionBomb();

                GameObject[] computer = GameObject.FindGameObjectsWithTag("computer");
                for (int i = 0; i < computer.Length; i++)
                {
                    Destroy(computer[i]);
                }

            }
            else
            {
                Destroy(gameObject);
            }

            //navMeshSurface.BuildNavMesh();
        }

        
        float dis = (transform.position - Player.GetInstance().transform.position).magnitude;
        //Debug.Log("EnemyList.Count: " + CheckEnemyNumber());
        // С�ֶ�����ʱ����ʼ��ʱ
        if (curSpawnTime > 0 && CheckEnemyNumber() == 0) curSpawnTime -= Time.deltaTime;

        // ����ڸ��������������Ъ���㣬���й����Ѿ���������ʱ�����ٻ�����
        if (dis < alertDistance && curSpawnTime <= 0 && CheckEnemyNumber() == 0)
        {
            hp -= spawnAmountHp;
            SpawnEnemyRound();
            curSpawnTime = spawnTime;
        }
        //Debug.Log("Player and SpawnPoint distance: " + dis);
    }
}
