using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class SpawnEnemy : MonoBehaviour
{
    
    bool flag = true;


    public List<E_PoolType> enemySpawnType = new List<E_PoolType>(); // Ҫ���ɹ��������




    public NavMeshSurface navMeshSurface; // ��ȡ�������еĵ������񣬶�̬���ɹ�������º決


    public float spawnRadius = 2.0f; // ���ɹ��������ɵ�İ뾶
    public int spawnNum = 4; // ���ɹ��������



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

                // ��λԲ�����һ��ĽǶ�
                float theta = Random.Range(0.0f, Mathf.PI * 2);
                // ��λԲ�����һ��
                Vector3 spawnPosition = new Vector3(Mathf.Sin(theta), 0, Mathf.Cos(theta));

                spawnPosition *= spawnRadius;

                spawnEnemy.transform.SetLocalPositionAndRotation(spawnPosition, Quaternion.identity);

                Debug.Log("spawnEnemy: " + spawnEnemy.transform.position);
            }

            // ���º決��������ʹ�ù����ϵ�AI�����������
            navMeshSurface.BuildNavMesh();



        }

        //Debug.Log("Player Distance: " + dis);
    }
}
