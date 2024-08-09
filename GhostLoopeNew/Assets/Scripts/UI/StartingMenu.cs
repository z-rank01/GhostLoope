using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartingMenu : MonoBehaviour
{


    public Button Setting;
    public Image SettingImage;
    public Button ExitSetting;

    public Slider BackMusicSlider;
    public Slider EnviromentSlider;
    public Slider FireSlider;


    public Slider San;

    //// ʹ������Sliderʵ����ͷ�仯��Ѫ��
    //public Slider San_left;
    //public Slider San_Right;


    public Slider Resilience;




    // ��ͣ��Ϸ�İ�ť���Լ����������ֵ�4����ť
    public Button PauseGame;
    public Button ContinueGame;
    public Button OperationGuide;
    public Button MusicSetting;
    public Button ReturnMainMenu;


    // ���²���ָ�ϰ�ť����ֵ������ؼ�
    public Image GuideImage;
    public Button GuideReturn;



    public Image BlurImage; // ��ͣ�����º��ģ��ͼƬ

    public Image FadeImage; // ��ʼ��Ϸ���ɺ�ɫ�𽥱�Ϊ͸������¶������
    //public Image UpperImage; // ��������ϰ벿��
    //public Image LowerImage; // ��������°벿��





    public Button Save;
    public Button Load;




    GameObject[] EnemyArray;

    GameObject[] EnemyHpArray;


    // Start is called before the first frame update
    void Start()
    {
        EnemyArray = GameObject.FindGameObjectsWithTag("Enemy");

        EnemyHpArray = GameObject.FindGameObjectsWithTag("HP");

        San.maxValue = GlobalSetting.GetInstance().san;
        //San_left.maxValue = GlobalSetting.GetInstance().san / 2;
        //San_Right.maxValue = GlobalSetting.GetInstance().san / 2;


        Resilience.maxValue = 40;

        Setting.onClick.AddListener(this.SettingButtonClicked);
        ExitSetting.onClick.AddListener(this.ExitSettingClicked);


        PauseGame.onClick.AddListener(this.PauseGameClicked);
        ContinueGame.onClick.AddListener(this.ContinueGameClicked);
        OperationGuide.onClick.AddListener(this.OperationGuideClicked);
        MusicSetting.onClick.AddListener(this.SettingButtonClicked); // ������������ð�ťһ����Ч��
        ReturnMainMenu.onClick.AddListener(this.ReturnMainMenuClicked);


        GuideReturn.onClick.AddListener(this.GuideReturnClicked);


        Save.onClick.AddListener(this.SaveButtonClicked);
        Load.onClick.AddListener(this.LoadButtonClicked);



        // 
        if (BeginGame.isLoadGameClicked)
        {
            LoadGame();
        }
        if (BeginGame.isNewGameClicked || BeginGame.isLoadGameClicked)
        {
            MusicManager.GetInstance().PlayBackgroundMusic("��һ��-����");

            BeginGame.isNewGameClicked = false;
            BeginGame.isLoadGameClicked = false;

        }


        FadeImage.gameObject.SetActive(true);
    }


    public void SaveGame()
    {
        // ����save���󣬼�¼��ǰ������Ҫ�������Ϣ
        Save save = new Save();

        Player player = Player.GetInstance();


        save.x = player.transform.position.x;
        save.y = player.transform.position.y;
        save.z = player.transform.position.z;


        for (int i = 0; i < EnemyArray.Length; i++)
        {
            save.EnemyActive.Add(EnemyArray[i].GetComponent<Enemy>().isActiveAndEnabled);
            Debug.Log("EnemyArray[i]: " + EnemyArray[i].GetComponent<Enemy>().isActiveAndEnabled);
        }


        for (int i = 0; i < EnemyHpArray.Length; i++)
        {
            save.EnemyHpActive.Add(EnemyHpArray[i].GetComponent<Slider>().isActiveAndEnabled);
            Debug.Log("EnemyHpArray[i]: " + EnemyHpArray[i].GetComponent<Slider>().isActiveAndEnabled);
        }

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


            Player player = Player.GetInstance();
            player.SetTransformPosition(new Vector3((float)save.x, (float)save.y, (float)save.z));


            for (int i = 0; i < EnemyArray.Length; i++)
            {
                EnemyArray[i].gameObject.SetActive(save.EnemyActive[i]);
            }


            for (int i = 0; i < EnemyArray.Length; i++)
            {
                EnemyHpArray[i].gameObject.SetActive(save.EnemyHpActive[i]);

            }
        }
        else
        {
            Debug.Log("�浵�ļ�������");
        }
    }


    int FadeAlpha = 10;
    // Update is called once per frame
    void Update()
    {
        San.value = Player.GetInstance().GetProperty(E_Property.san);
        //San_left.value = Player.GetInstance().GetProperty(E_Property.san) / 2;
        //San_Right.value = Player.GetInstance().GetProperty(E_Property.san) / 2;

        Debug.Log("Resilience.fillRect: " + Resilience.fillRect.transform.position);
        
        Resilience.value = Player.GetInstance().GetProperty(E_Property.resilience);


        MusicManager.GetInstance().ChangeBackgroundValue(BackMusicSlider.value);
        MusicManager.GetInstance().ChangeEnvironmentValue(EnviromentSlider.value);
        MusicManager.GetInstance().ChangeFireValue(FireSlider.value);


        if (FadeAlpha > 0)
        {
            FadeAlpha--;
            FadeImage.GetComponent<Image>().color = new Color(0, 0, 0, (FadeAlpha * 1.0f / 10));
            if (FadeAlpha == 0)
            {
                GameObject.Destroy(FadeImage);
            }
        }


    }


    public void SaveButtonClicked()
    {
        Debug.Log("SaveButtonClicked");


        SaveGame();

    }
    public void LoadButtonClicked()
    {
        Debug.Log("In StartingMenu Load Button Clicked");

        LoadGame();
    }





    // ���Ű�ť���µ�����
    public void PlayButtonClickedSound()
    {
        MusicManager.GetInstance().PlayEnvironmentSound("����ѡ����");
    }
    
    public void NewGameButtonClicked()
    {
        SceneManager.LoadScene("TestScene");

        Time.timeScale = 1.0f; // ����������Ϸ
        Player.GetInstance().enabled = true;



        PlayButtonClickedSound();



        Debug.Log("new GameButtonClicked");
        MusicManager.GetInstance().PlayBackgroundMusic("��һ��-����");

        Setting.gameObject.SetActive(false);

        PauseGame.gameObject.SetActive(true);
    }
    
    public void SettingButtonClicked()
    {
        PlayButtonClickedSound();
        SettingImage.gameObject.SetActive(true);


        ContinueGame.gameObject.SetActive(false);
        OperationGuide.gameObject.SetActive(false);
        MusicSetting.gameObject.SetActive(false);
        ReturnMainMenu.gameObject.SetActive(false);



    }
    public void ExitButtonClicked()
    {
        PlayButtonClickedSound();
        MusicManager.GetInstance().PlayEnvironmentSound("����ѡ����");

        // �����������˳���ť�������˳���Ϸ

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif

    }
    public void PauseGameClicked()
    {
        Time.timeScale = 0.0f; // ��ͣ��Ϸ��timescaleֻӰ��fixupdate������Ӱ��update��lateupdate

        Player.GetInstance().enabled = false;







        PlayButtonClickedSound();

        ContinueGame.gameObject.SetActive(true);
        OperationGuide.gameObject.SetActive(true);
        MusicSetting.gameObject.SetActive(true);
        ReturnMainMenu.gameObject.SetActive(true);


        BlurImage.gameObject.SetActive(true);
    }
    public void ContinueGameClicked()
    {
        Time.timeScale = 1.0f; // ����������Ϸ
        Player.GetInstance().enabled = true;






        PlayButtonClickedSound();

        ContinueGame.gameObject.SetActive(false);
        OperationGuide.gameObject.SetActive(false);
        MusicSetting.gameObject.SetActive(false);
        ReturnMainMenu.gameObject.SetActive(false);


        BlurImage.gameObject.SetActive(false);
    }

    //
    public void OperationGuideClicked()
    {
        PlayButtonClickedSound();


        ContinueGame.gameObject.SetActive(false);
        OperationGuide.gameObject.SetActive(false);
        MusicSetting.gameObject.SetActive(false);
        ReturnMainMenu.gameObject.SetActive(false);

        GuideImage.gameObject.SetActive(true);
        GuideReturn.gameObject.SetActive(true);


    }


    public void GuideReturnClicked()
    {
        PlayButtonClickedSound();

        ContinueGame.gameObject.SetActive(true);
        OperationGuide.gameObject.SetActive(true);
        MusicSetting.gameObject.SetActive(true);
        ReturnMainMenu.gameObject.SetActive(true);


        GuideImage.gameObject.SetActive(false);
        GuideReturn.gameObject.SetActive(false);
    }
    public void ReturnMainMenuClicked()
    {
        PlayButtonClickedSound();

        // �ص�������ĳ���
        SceneManager.LoadScene("BeginGame");
        //SceneManager.UnloadSceneAsync("TestScene");

    }



    public void ExitSettingClicked()
    {
        PlayButtonClickedSound();

        MusicManager.GetInstance().PlayEnvironmentSound("����ѡ����");
        SettingImage.gameObject.SetActive(false);
        
        if (PauseGame.IsActive())
        {
            ContinueGame.gameObject.SetActive(true);
            OperationGuide.gameObject.SetActive(true);
            MusicSetting.gameObject.SetActive(true);
            ReturnMainMenu.gameObject.SetActive(true);
        }
    }
    
}
