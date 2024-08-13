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
    public float hp = 100.0f; // 生成点的HP
    public float spawnAmountHp = 10.0f; //召唤一次怪物所需要的HP

    // AI
    public NavMeshSurface navMeshSurface; // 获取到场景中的导航网格，动态生成怪物后重新烘焙

    // UI
    public Slider spawnHp;
    public Vector3 spawnHpOffset;
    public HintUI ui;

    // Spawn Enemy
    public List<E_PoolType> enemySpawnType = new List<E_PoolType>(); // 要生成怪物的类型
    public float alertDistance = 8.0f; // 生成点与玩家的距离小于该值后方可召唤小怪
    public int spawnNum = 4; // 生成怪物的数量
    public float spawnRadius = 2.0f; // 生成怪物离生成点的半径
    public float spawnTime = 5.0f; // 召唤怪物的固定间隔
    private float curSpawnTime = 0.0f; // 召唤怪物的计时器，<=0 则可以召唤
    private List<GameObject> EnemyList = new List<GameObject>(); // 存放小怪的数组


    public Transform enemySpawnPoint;
    public Transform bossSpawnPoint; // 手动指定Boss生成点

    public int id; // 存档用的该物体的id

    // 目前存活的小怪数量
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
        // 不存在小怪时才能造成伤害
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

    // 召唤一轮小怪
    public void SpawnEnemyRound()
    {
        if (enemySpawnType.Count == 0) return;
        for (int i = 0; i < spawnNum; i++)
        {
            // Random.Range()返回[l,r)的值!!!
            int EnemyType = Random.Range(0, enemySpawnType.Count);
            //Debug.Log("EnemyType: " + EnemyType);
            // 随机产生怪物
            GameObject spawnEnemy = PoolManager.GetInstance().GetObj(enemySpawnType[EnemyType]);
            spawnEnemy.name = "spawnEnemy" + i;

            spawnEnemy.transform.SetParent(enemySpawnPoint, true);
            Debug.Log("After Set Parent : " + spawnEnemy.transform.localPosition);
            // 单位圆上随机一点的角度
            float theta = Random.Range(0.0f, Mathf.PI * 2);
            // 单位圆上随机一点
            Vector3 spawnPosition = new Vector3(Mathf.Sin(theta), 0, Mathf.Cos(theta));
            Debug.Log("SpawnPosition Normal: " + spawnPosition);
            spawnPosition *= spawnRadius;

            // 设置怪物的出生点（相对于父物体）
            spawnEnemy.transform.SetLocalPositionAndRotation(spawnPosition, Quaternion.identity);

            //Debug.Log("i : " + i);
            //Debug.Log("SpawnPosition * Radius: " + spawnPosition);
            //Debug.Log("spawnEnemy.transform.position: " + spawnEnemy.transform.position);
            //Debug.Log("spawnEnemy.transform.localPosition: " + spawnEnemy.transform.localPosition);

            float enemyDis = (spawnPosition).magnitude;
            //Debug.Log("enemyDis: " + enemyDis);

            Enemy enemy = spawnEnemy.GetComponent<Enemy>();
            enemy.isNeedRedHp = false;
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
                enemy.id = 999999999;

                if (enemy.enemyHp.GetComponent<HintUI>() == null)
                {
                    enemy.enemyHp.AddComponent<HintUI>();
                }

                enemy.enemyHp.GetComponent<HintUI>().SetCameraAndFollowingTarget(Camera.main, enemy.transform);
                enemy.enemyHp.GetComponent<HintUI>().SetOffset(Vector3.zero);

                EnemyList.Add(spawnEnemy);
            }

        }

        // 重新烘焙导航网格，使得怪物上的AI组件可以运行

        navMeshSurface.BuildNavMesh();
    }


    // 召唤Boss
    public void spawnBossPosionBomb()
    {
        GameObject spawnEnemy = PoolManager.GetInstance().GetObj(E_PoolType.BossPoisonBomb1);
        spawnEnemy.name = "BossPoisonBomb";

        
        //spawnEnemy.transform.SetParent(bossSpawnPoint, true);

        //// 设置怪物的出生点（相对于父物体）w
        //spawnEnemy.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);


        spawnEnemy.GetComponent<Enemy>().enemyHp.GetComponent<HintUI>().SetCameraAndFollowingTarget(Camera.main, null);


        navMeshSurface.BuildNavMesh();

        // 设置Boss战状态，显示Boss血条
        Player.GetInstance().SetIsFightingBoss(true);

    }
    void Update()
    {
        ui.SetOffset(spawnHpOffset);
        spawnHp.value = hp;
        if (hp <= 0)
        {
            // 一坨
            spawnHp.gameObject.SetActive(false); // 消失怪物召唤点的血条
            //for (int i = 0; i < EnemyList.Count; i++)
            //{
            //    // 消失各小怪的血条
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

            navMeshSurface.BuildNavMesh();
        }

        
        float dis = (transform.position - Player.GetInstance().transform.position).magnitude;
        //Debug.Log("EnemyList.Count: " + CheckEnemyNumber());
        // 小怪都死完时，开始计时
        if (curSpawnTime > 0 && CheckEnemyNumber() == 0) curSpawnTime -= Time.deltaTime;

        // 玩家在附近，产生怪物间歇满足，所有怪物已经死亡，此时可以召唤怪物
        if (dis < alertDistance && curSpawnTime <= 0 && CheckEnemyNumber() == 0)
        {
            hp -= spawnAmountHp;
            SpawnEnemyRound();
            curSpawnTime = spawnTime;
        }
        //Debug.Log("Player and SpawnPoint distance: " + dis);
    }
}
