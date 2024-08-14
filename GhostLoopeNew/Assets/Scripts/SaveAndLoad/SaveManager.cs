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
    public bool is_Soul1 = false;
    public bool is_Soul2 = false;
    public float san = 300, res = 30;
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
        // ����save���󣬼�¼��ǰ������Ҫ�������Ϣ
        Save save = new Save();

        Player player = Player.GetInstance();


        save.x = player.transform.position.x + 2.0f;
        save.y = player.transform.position.y;
        save.z = player.transform.position.z;

        save.san = GlobalSetting.GetInstance().san;
        save.res = GlobalSetting.GetInstance().resilience;

        san = GlobalSetting.GetInstance().san;
        res = GlobalSetting.GetInstance().resilience;

        save.is_Soul1 = Player.GetInstance().GetSoul_1();
        save.is_Soul2 = Player.GetInstance().GetSoul_2();


        is_Soul1 = Player.GetInstance().GetSoul_1();
        is_Soul2 = Player.GetInstance().GetSoul_2();


        Debug.Log("Save Game save.transform: " + player.transform.position);


        // ��ȡ��ǰ��������
        save.sceneName = SceneManager.GetActiveScene().name;


        // ���浱ǰ�����е���Ʒ��Ϣ��Exist or not)

        // Ĺ��
        GameObject[] GraveStoneObject = GameObject.FindGameObjectsWithTag("GraveStone");
        for (int i = 0;i < GraveStoneObject.Length;i++)
        {
            save.GraveStoneId.Add(GraveStoneObject[i].GetComponent<GraveStone>().id);
        }

        // �������ɵ�
        GameObject[] currentSpawnEnemy = GameObject.FindGameObjectsWithTag("SpawnEnemy");
        for (int i = 0; i < currentSpawnEnemy.Length; i++)
        {
            save.SpawnEnemyId.Add(currentSpawnEnemy[i].GetComponent<SpawnEnemy>().id);
        }

        // ��һ���������ɵ�
        GameObject[] Computer = GameObject.FindGameObjectsWithTag("computer");
        for (int i = 0; i < Computer.Length; i++)
        {
            // ֻ��һ��computer�Ƿ��˽ű���
            if (Computer[i].GetComponent<SpawnEnemy>() != null)
            {
                save.ComputerId.Add(Computer[i].GetComponent<SpawnEnemy>().id);
            }
        }

        // ����

        // ������˵�����ҲҪ����Ļ�����ҪΪÿһ��Enemy��Ҫ����һ����ͬ��id


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

        // ��save����д��json�ļ�
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
            // ��ȡjson�ļ�
            StreamReader sr = new StreamReader(filePath);

            string jsonStr = sr.ReadToEnd();

            sr.Close();

            Save save = JsonMapper.ToObject<Save>(jsonStr);



            // �ȼ��س���
            Debug.Log("Current Scene Name: " + SceneManager.GetActiveScene().name);
            SceneManager.LoadScene(save.sceneName);
            


            // �����ȡ���ĵ���player����һ�ص�player���������⸳ֵ
            // ��Player�Ŀ�ʼ�����м��������Ϣ!
            //Player player = Player.GetInstance();
            //player.SetTransformPosition(new Vector3((float)save.x, (float)save.y, (float)save.z));

            //Debug.Log("Player: " + player.gameObject);
            //Debug.Log("Player.transform:" + player.transform.position);

            //Debug.Log("Player.transform:" + player.transform.position);




            // �������Ϣ���棬player����ʱ����
            x = (float)save.x;
            y = (float)save.y;
            z = (float)save.z;

            san = save.san;
            res = save.res;

            is_Soul1 = save.is_Soul1;
            is_Soul2 = save.is_Soul2;

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
            Debug.Log("�浵�ļ�������");

            // ����û�д浵������£��������Ϸ��������Ϸ��Ч�������ص�һ�صĳ���
            SceneManager.LoadScene("Level1");
        }
    }


}
