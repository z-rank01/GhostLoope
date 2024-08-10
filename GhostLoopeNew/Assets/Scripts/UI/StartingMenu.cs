using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class StartingMenu : MonoBehaviour
{


    public Button Setting;
    public Image SettingImage;
    public Button ExitSetting;

    public Slider BackMusicSlider;
    public Slider EnviromentSlider;
    public Slider FireSlider;

    // 玩家的血条和韧性条
    public Slider San;
    public Slider Resilience;

    // Boss的血条和韧性条
    public Slider Enemy_San;
    public Slider Enemy_Res;

    // 玩家状态是否处于Boss战，该变量可放到Player相关类中
    bool isFightingBoss = true;

    // 暂停游戏的按钮，以及接下来出现的4个按钮
    public Button PauseGame;
    public Button ContinueGame;
    public Button OperationGuide;
    public Button MusicSetting;
    public Button ReturnMainMenu;


    // 按下操作指南按钮后出现的两个控件
    public Image GuideImage;
    public Button GuideReturn;



    public Image BlurImage; // 暂停键按下后的模糊图片


    int FadeAlpha = 10; // 显露出场景的速度，FadeAlpha 帧后FadeImage彻底消失
    public Image FadeImage; // 开始游戏后由黑色逐渐变为透明，显露出场景


    // 存档功能的测试按钮
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
        Resilience.maxValue = GlobalSetting.GetInstance().resilience;



        Setting.onClick.AddListener(this.SettingButtonClicked);
        ExitSetting.onClick.AddListener(this.ExitSettingClicked);


        PauseGame.onClick.AddListener(this.PauseGameClicked);
        ContinueGame.onClick.AddListener(this.ContinueGameClicked);
        OperationGuide.onClick.AddListener(this.OperationGuideClicked);
        MusicSetting.onClick.AddListener(this.SettingButtonClicked); // 和主界面的设置按钮一样的效果
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
            MusicManager.GetInstance().PlayBackgroundMusic("第一关-配乐");

            BeginGame.isNewGameClicked = false;
            BeginGame.isLoadGameClicked = false;

        }


        FadeImage.gameObject.SetActive(true);
    }

    void UpdateSanAndRes(Slider san, Slider res, float maxSan, float maxRes)
    {
        



        // 整个血条的长度
        float width_San = san.GetComponent<RectTransform>().sizeDelta.x;

        // 整个韧性条的长度
        float width_Res = res.GetComponent<RectTransform>().sizeDelta.x;

        // 血条削减长度
        float left_San = width_San / 2 - san.value / maxSan * 0.5f * width_San;

        // 韧性条削减长度
        float left_Res = width_Res / 2 - res.value / maxRes * 0.5f * width_Res;

        // 固定锚点
        san.fillRect.anchorMin = new Vector2(0, 0);
        san.fillRect.anchorMax = new Vector2(1, 1);
        res.fillRect.anchorMin = new Vector2(0, 0);
        res.fillRect.anchorMax = new Vector2(1, 1);

        // 修改left, bottom
        san.fillRect.offsetMin = new Vector2(left_San, 0);
        res.fillRect.offsetMin = new Vector2(left_Res, 0);

        // 修改right, top
        san.fillRect.offsetMax = new Vector2(-left_San, 0);
        res.fillRect.offsetMax = new Vector2(-left_Res, 0);
    }




    void UpdateMusicVolume()
    {
        MusicManager.GetInstance().ChangeBackgroundValue(BackMusicSlider.value);
        MusicManager.GetInstance().ChangeEnvironmentValue(EnviromentSlider.value);
        MusicManager.GetInstance().ChangeFireValue(FireSlider.value);
    }


    void UpdateFadeImage()
    {
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
    // Update is called once per frame
    void Update()
    {
        // 动态更新最大值
        San.maxValue = GlobalSetting.GetInstance().san;
        Resilience.maxValue = GlobalSetting.GetInstance().resilience;

        float height = San.GetComponent<RectTransform>().sizeDelta.y;

        // 长度根据玩家的属性按比例增加， 高度保持不变 (目前的比例为4、10，即100点生命值对应400个单位的生命条长度、40点韧性值对应400个单位的韧性条长度）
        San.GetComponent<RectTransform>().sizeDelta = new Vector2(San.maxValue * 4, height);
        Resilience.GetComponent<RectTransform>().sizeDelta = new Vector2(Resilience.maxValue * 10, height);



        San.value = Player.GetInstance().GetProperty(E_Property.san);
        Resilience.value = Player.GetInstance().GetProperty(E_Property.resilience);

        // 更新玩家的血条和韧性条
        UpdateSanAndRes(San, Resilience, San.maxValue, Resilience.maxValue);

        // 处于Boss战状态，显示和更新Boss的血条和韧性条
        if (isFightingBoss)
        {
            //UpdateSanAndRes(Enemy_San, Enemy_Res, 100, 40);
        }

        // 动态修改音量大小
        UpdateMusicVolume();

        // 播放场景显现动画
        UpdateFadeImage();
    }




    public void SaveGame()
    {
        // 创建save对象，记录当前场景中要保存的信息
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
            Debug.Log("存档文件不存在");
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





    // 播放按钮按下的声音
    public void PlayButtonClickedSound()
    {
        MusicManager.GetInstance().PlayEnvironmentSound("界面选择音");
    }
    
    public void NewGameButtonClicked()
    {
        SceneManager.LoadScene("TestScene");

        Time.timeScale = 1.0f; // 正常继续游戏
        Player.GetInstance().enabled = true;



        PlayButtonClickedSound();



        Debug.Log("new GameButtonClicked");
        MusicManager.GetInstance().PlayBackgroundMusic("第一关-配乐");

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
        MusicManager.GetInstance().PlayEnvironmentSound("界面选择音");

        // 点击主界面的退出按钮后立即退出游戏

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif

    }
    public void PauseGameClicked()
    {
        Time.timeScale = 0.0f; // 暂停游戏，timescale只影响fixupdate，而不影响update和lateupdate

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
        Time.timeScale = 1.0f; // 正常继续游戏
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

        // 回到主界面的场景
        SceneManager.LoadScene("BeginGame");
        //SceneManager.UnloadSceneAsync("TestScene");

    }



    public void ExitSettingClicked()
    {
        PlayButtonClickedSound();

        MusicManager.GetInstance().PlayEnvironmentSound("界面选择音");
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
