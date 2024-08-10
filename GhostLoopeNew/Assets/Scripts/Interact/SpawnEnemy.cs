using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class SpawnEnemy : MonoBehaviour
{
    
    bool flag = true;


    public List<E_PoolType> enemySpawnType = new List<E_PoolType>(); // 要生成怪物的类型




    public NavMeshSurface navMeshSurface; // 获取到场景中的导航网格，动态生成怪物后重新烘焙


    public float spawnRadius = 2.0f; // 生成怪物离生成点的半径
    public int spawnNum = 4; // 生成怪物的数量



    public void OnTriggerEnter(Collider other)
    {

        Debug.Log("OnTriggerEnter: SpawnEnemy:");
    }
    void Start()
    {
        
    }
    void Update()
    {
        float dis = (transform.position - Player.GetInstance().transform.position).magnitude;

        
        if (dis < 8 && flag)
        {
            flag = false;



            //GameObject go = new GameObject("go");
            //go.transform.SetParent(transform, false);



            //Debug.Log("go: " + go.transform.position);

            //Debug.Log("go: " + go.transform.position);




            for (int i = 0; i < spawnNum; i++)
            {

                GameObject spawnEnemy = PoolManager.GetInstance().GetObj(E_PoolType.spawnEnemy);
                spawnEnemy.name = "spawnEnemy1";
                spawnEnemy.transform.SetParent(transform);

                // 单位圆上随机一点的角度
                float theta = Random.Range(0.0f, Mathf.PI * 2);
                // 单位圆上随机一点
                Vector3 spawnPosition = new Vector3(Mathf.Sin(theta), 0, Mathf.Cos(theta));

                spawnPosition *= spawnRadius;

                spawnEnemy.transform.SetLocalPositionAndRotation(spawnPosition, Quaternion.identity);

                Debug.Log("spawnEnemy: " + spawnEnemy.transform.position);
            }

            // 重新烘焙导航网格，使得怪物上的AI组件可以运行
            navMeshSurface.BuildNavMesh();



        }

        //Debug.Log("Player Distance: " + dis);
    }
}
