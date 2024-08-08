using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LitJson;
using System.IO;

public class BeginGame : MonoBehaviour
{
    public Button NewGame;


    public Button LoadGame;

    public Button MusicSetting;
    public Button ExitGame;


    public Image UpperImage; // 主界面的上半部分
    public Image LowerImage; // 主界面的下半部分

    public Image SettingImage; // 音效设置


    public Button ExitSetting; // 退出音效设置 

    public Slider BackMusicSlider;
    public Slider EnviromentSlider;
    public Slider FireSlider;

    public Image SelectImage; // 信封的贴纸组件

    // 使用静态变量，在StartingMenu类中访问该变量
    public static bool isNewGameClicked = false; //新游戏，加载初始场景
    public static bool isLoadGameClicked = false; // 继续游戏，加载初始场景后需要额外加载Json文件



    int frame_count = 0;
    public void Start()
    {
        Time.timeScale = 1.0f;
        NewGame.onClick.AddListener(this.NewButtonClicked);

        LoadGame.onClick.AddListener(this.LoadButtonClicked);
        MusicSetting.onClick.AddListener(this.MusicButtonClicked);
        ExitGame.onClick.AddListener(this.ExitButtonClicked);
        ExitSetting.onClick.AddListener(this.ExitMusicClicked);





        BackMusicSlider.value = 0.5f;
        EnviromentSlider.value = 0.5f;
        FireSlider.value = 0.5f;

    }

    bool isReadyToOpenLower = false; // 是否准备好要开始翻开下半页

    private void Update()
    {
        // 修改position可以实现图片移动的效果
        // UpperImage.rectTransform.anchoredPosition += new Vector2(0, 10); 


        // 修改rotation实现打开信封的效果

        // 先打开上半页
        if ((isNewGameClicked == true || isLoadGameClicked) && isReadyToOpenLower == false && frame_count >= 0 && frame_count < 90)
        {
            frame_count++;
            UpperImage.rectTransform.localRotation = Quaternion.Euler(frame_count, 0, 0);
            //LowerImage.rectTransform.localRotation = Quaternion.Euler(frame_count * -1, 0, 0);
            if (frame_count == 90)
            {
                isReadyToOpenLower = true;
                frame_count = 0;

            }
        }

        // 再打开下半页
        if (isReadyToOpenLower && frame_count >= 0 && frame_count < 90)
        {
            frame_count++;
            LowerImage.rectTransform.localRotation = Quaternion.Euler(frame_count * -1, 0, 0);

            if (frame_count == 90)
            {
                isReadyToOpenLower = false;

                SceneManager.LoadScene("Level1");

                Debug.Log("Load Level1 End!");

                //SceneManager.UnloadSceneAsync("BeginGame");
            }
        }


        // 动态调整音量
        MusicManager.GetInstance().ChangeBackgroundValue(BackMusicSlider.value);
        MusicManager.GetInstance().ChangeEnvironmentValue(EnviromentSlider.value);
        MusicManager.GetInstance().ChangeFireValue(FireSlider.value);


    }
    public void NewButtonClicked()
    {
        MusicManager.GetInstance().PlayEnvironmentSound("界面选择音");
        isNewGameClicked = true;


        SelectImage.gameObject.SetActive(false);


    }
    public void LoadButtonClicked()
    {
        isLoadGameClicked = true;
        MusicManager.GetInstance().PlayEnvironmentSound("界面选择音");

        SelectImage.gameObject.SetActive(false);





    }
    public void MusicButtonClicked()
    {
        MusicManager.GetInstance().PlayEnvironmentSound("界面选择音");


        SettingImage.gameObject.SetActive(true);
    }


    public void ExitMusicClicked()
    {
        MusicManager.GetInstance().PlayEnvironmentSound("界面选择音");
        SettingImage.gameObject.SetActive(false);
    }


    public void ExitButtonClicked()
    {
        MusicManager.GetInstance().PlayEnvironmentSound("界面选择音");

        // 点击主界面的退出按钮后立即退出游戏
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
