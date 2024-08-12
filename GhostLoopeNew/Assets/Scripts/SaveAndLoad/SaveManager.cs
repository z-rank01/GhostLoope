using BehaviorDesigner.Runtime.Tasks.Unity.UnityPlayerPrefs;
using JetBrains.Annotations;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : BaseSingleton<SaveManager>
{

    public bool loading = false;
    public float x, y, z;
    public List<int> GraveStoneId = new List<int>();
    public List<int> SpawnEnemyId = new List<int>();
    public List<int> ComputerId = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Player.GetInstance().transform.position);
    }


    public void SaveGame()
    {
        // 创建save对象，记录当前场景中要保存的信息
        Save save = new Save();

        Player player = Player.GetInstance();


        save.x = player.transform.position.x;
        save.y = player.transform.position.y;
        save.z = player.transform.position.z;


        Debug.Log("Save Game save.transform: " + player.transform.position);


        // 获取当前场景名字
        save.sceneName = SceneManager.GetActiveScene().name;


        // 保存当前场景中的物品信息（Exist or not)

        // 墓碑
        GameObject[] GraveStoneObject = GameObject.FindGameObjectsWithTag("GraveStone");
        for (int i = 0;i < GraveStoneObject.Length;i++)
        {
            save.GraveStoneId.Add(GraveStoneObject[i].GetComponent<GraveStone>().id);
        }

        // 怪物生成点
        GameObject[] currentSpawnEnemy = GameObject.FindGameObjectsWithTag("SpawnEnemy");
        for (int i = 0; i < currentSpawnEnemy.Length; i++)
        {
            save.SpawnEnemyId.Add(currentSpawnEnemy[i].GetComponent<SpawnEnemy>().id);
        }

        // 另一个怪物生成点
        GameObject[] Computer = GameObject.FindGameObjectsWithTag("computer");
        for (int i = 0; i < Computer.Length; i++)
        {
            // 只有一个computer是放了脚本的
            if (Computer[i].GetComponent<SpawnEnemy>() != null)
            {
                save.ComputerId.Add(Computer[i].GetComponent<SpawnEnemy>().id);
            }
        }

        // 敌人

        // 如果敌人的有无也要保存的话，需要为每一个Enemy都要设置一个不同的id


        //GameObject[] EnemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        //for (int i = 0; i < EnemyArray.Length; i++)
        //{
        //    if (EnemyArray[i].GetComponent<Enemy>() != null)
        //    {
        //        save.EnemyId.Add(EnemyArray[i].GetComponent<Enemy>().id);
        //    }
        //}



        //for (int i = 0; i < EnemyArray.Length; i++)
        //{
        //    save.EnemyActive.Add(EnemyArray[i].GetComponent<Enemy>().isActiveAndEnabled);
        //    Debug.Log("EnemyArray[i]: " + EnemyArray[i].GetComponent<Enemy>().isActiveAndEnabled);
        //}


        //for (int i = 0; i < EnemyHpArray.Length; i++)
        //{
        //    save.EnemyHpActive.Add(EnemyHpArray[i].GetComponent<Slider>().isActiveAndEnabled);
        //    Debug.Log("EnemyHpArray[i]: " + EnemyHpArray[i].GetComponent<Slider>().isActiveAndEnabled);
        //}

        // 将save对象写入json文件
        string filePath = Application.dataPath + "/StreamingAssets" + "/byJson.json";

        Debug.Log("filePath: " + filePath);


        string saveJsonStr = JsonMapper.ToJson(save);

        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(saveJsonStr);
        sw.Close();
        Debug.Log("End Save!");


        Debug.Log("StreamingAssetPath: " + Application.streamingAssetsPath);
    }

    
    public void LoadGame()
    {
        string filePath = Application.dataPath + "/StreamingAssets" + "/byJson.json";
        if (File.Exists(filePath))
        {
            // 读取json文件
            StreamReader sr = new StreamReader(filePath);

            string jsonStr = sr.ReadToEnd();

            sr.Close();

            Save save = JsonMapper.ToObject<Save>(jsonStr);



            // 先加载场景
            SceneManager.LoadScene(save.sceneName);
            //SceneManager.LoadScene("Level1");

            Debug.Log("Current Scene Name: " + SceneManager.GetActiveScene().name);


            // 这里获取到的单例player是上一关的player，不能在这赋值
            // 在Player的开始函数中加载相关信息!
            //Player player = Player.GetInstance();
            //player.SetTransformPosition(new Vector3((float)save.x, (float)save.y, (float)save.z));

            //Debug.Log("Player: " + player.gameObject);
            //Debug.Log("Player.transform:" + player.transform.position);

            //Debug.Log("Player.transform:" + player.transform.position);




            // 把相关信息保存，player唤醒时加载
            x = (float)save.x;
            y = (float)save.y;
            z = (float)save.z;


            GraveStoneId = save.GraveStoneId;
            SpawnEnemyId = save.SpawnEnemyId;
            ComputerId = save.ComputerId;


            loading = true;

            //for (int i = 0; i < EnemyArray.Length; i++)
            //{
            //    EnemyArray[i].gameObject.SetActive(save.EnemyActive[i]);
            //}


            //for (int i = 0; i < EnemyArray.Length; i++)
            //{
            //    EnemyHpArray[i].gameObject.SetActive(save.EnemyHpActive[i]);

            //}
        }
        else
        {
            Debug.Log("存档文件不存在");

            // 本身没有存档的情况下，点继续游戏还是新游戏的效果，加载第一关的场景
            SceneManager.LoadScene("Level1");
        }
    }


}
