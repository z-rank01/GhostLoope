using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartingMenu : MonoBehaviour
{
    public Button NewGame;


    public Button Setting;
    public Image SettingImage;
    public Button ExitSetting;

    public Slider BackMusicSlider;
    public Slider EnviromentSlider;
    public Slider FireSlider;



    public Button ExitGame;
    public Image ExitImage;


    public Slider SAN;
    public Slider Resilience;




    // 暂停游戏的按钮，以及接下来出现的4个按钮
    public Button PauseGame;
    public Button ContinueGame;
    public Button OperationGuide;
    public Button MusicSetting;
    public Button ReturnMainMenu;


    // 按下操作指南按钮后出现的两个控件
    public Image GuideImage;
    public Button GuideReturn;



    // Start is called before the first frame update
    void Start()
    {

        SAN.maxValue = GlobalSetting.GetInstance().san;

        Resilience.maxValue = 40;

        NewGame.onClick.AddListener(this.NewGameButtonClicked);
        Setting.onClick.AddListener(this.SettingButtonClicked);
        ExitGame.onClick.AddListener(this.ExitButtonClicked);
        ExitSetting.onClick.AddListener(this.ExitSettingClicked);


        PauseGame.onClick.AddListener(this.PauseGameClicked);
        ContinueGame.onClick.AddListener(this.ContinueGameClicked);
        OperationGuide.onClick.AddListener(this.OperationGuideClicked);
        MusicSetting.onClick.AddListener(this.SettingButtonClicked); // 和主界面的设置按钮一样的效果
        ReturnMainMenu.onClick.AddListener(this.ReturnMainMenuClicked);


        GuideReturn.onClick.AddListener(this.GuideReturnClicked);

        BackMusicSlider.value = 0.5f;
        EnviromentSlider.value = 0.5f;
        FireSlider.value = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {


        SAN.value = Player.GetInstance().GetProperty(E_Property.san);
        Resilience.value = Player.GetInstance().GetProperty(E_Property.resilience);


        MusicManager.GetInstance().ChangeBackgroundValue(BackMusicSlider.value);
        MusicManager.GetInstance().ChangeEnvironmentValue(EnviromentSlider.value);
        MusicManager.GetInstance().ChangeFireValue(FireSlider.value);

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    NewGame.gameObject.SetActive(true);
        //    Setting.gameObject.SetActive(true);
        //    ExitGame.gameObject.SetActive(true);
        //}
       
    }



    // 播放按钮按下的声音
    public void PlayButtonClickedSound()
    {
        MusicManager.GetInstance().PlayEnvironmentSound("界面选择音");
    }

    public void NewGameButtonClicked()
    {
        Time.timeScale = 1.0f; // 正常继续游戏
        Player.GetInstance().enabled = true;



        PlayButtonClickedSound();



        Debug.Log("new GameButtonClicked");
        MusicManager.GetInstance().PlayBackgroundMusic("第一关-配乐");

        NewGame.gameObject.SetActive(false);
        Setting.gameObject.SetActive(false);
        ExitGame.gameObject.SetActive(false);

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

        PauseGame.gameObject.SetActive(false);
        ContinueGame.gameObject.SetActive(false);
        OperationGuide.gameObject.SetActive(false);
        MusicSetting.gameObject.SetActive(false);
        ReturnMainMenu.gameObject.SetActive(false);


        NewGame.gameObject.SetActive(true);
        Setting.gameObject.SetActive(true);
        ExitGame.gameObject.SetActive(true);
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
