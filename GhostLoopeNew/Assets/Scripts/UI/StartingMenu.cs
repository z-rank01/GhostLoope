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
    public Button ExitGameYes;
    public Button ExitGameNo;


    public Slider SAN;
    public Slider Resilience;




    // ��ͣ��Ϸ�İ�ť���Լ����������ֵ�4����ť
    public Button PauseGame;
    public Button ContinueGame;
    public Button OperationGuide;
    public Button MusicSetting;
    public Button ReturnMainMenu;


    // Start is called before the first frame update
    void Start()
    {

        SAN.maxValue = GlobalSetting.GetInstance().san;

        Resilience.maxValue = 40;

        NewGame.onClick.AddListener(this.NewGameButtonClicked);
        Setting.onClick.AddListener(this.SettingButtonClicked);
        ExitGame.onClick.AddListener(this.ExitButtonClicked);
        ExitSetting.onClick.AddListener(this.ExitSettingClicked);


        //ExitGameYes.onClick.AddListener(this.ExitButtonYesClicked);
        //ExitGameNo.onClick.AddListener(this.ExitButtonNoClicked);



        PauseGame.onClick.AddListener(this.PauseGameClicked);
        ContinueGame.onClick.AddListener(this.ContinueGameClicked);
        OperationGuide.onClick.AddListener(this.OperationGuideClicked);
        MusicSetting.onClick.AddListener(this.SettingButtonClicked); // ������������ð�ťһ����Ч��
        ReturnMainMenu.onClick.AddListener(this.ReturnMainMenuClicked);




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



    // ���Ű�ť���µ�����
    public void PlayButtonClickedSound()
    {
        MusicManager.GetInstance().PlayEnvironmentSound("����ѡ����");
    }

    public void NewGameButtonClicked()
    {
        Time.timeScale = 1.0f; // ����������Ϸ


        PlayButtonClickedSound();



        Debug.Log("new GameButtonClicked");
        MusicManager.GetInstance().PlayBackgroundMusic("��һ��-����");

        NewGame.gameObject.SetActive(false);
        Setting.gameObject.SetActive(false);
        ExitGame.gameObject.SetActive(false);

        PauseGame.gameObject.SetActive(true);
    }
    
    public void SettingButtonClicked()
    {
        PlayButtonClickedSound();
        SettingImage.gameObject.SetActive(true);
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
        Time.timeScale = 0.0f; // ��ͣ��Ϸ

        Player.GetInstance().enabled = false;







        PlayButtonClickedSound();

        ContinueGame.gameObject.SetActive(true);
        OperationGuide.gameObject.SetActive(true);
        MusicSetting.gameObject.SetActive(true);
        ReturnMainMenu.gameObject.SetActive(true);
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
    }
    public void OperationGuideClicked()
    {
        PlayButtonClickedSound();
    }
    //public void PauseGameClicked()
    //{

    //}
    public void ReturnMainMenuClicked()
    {
        PlayButtonClickedSound();


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

        MusicManager.GetInstance().PlayEnvironmentSound("����ѡ����");
        SettingImage.gameObject.SetActive(false);
    }
    //public void ExitButtonYesClicked()
    //{
    //    MusicManager.GetInstance().PlayEnvironmentSound("����ѡ����");

    //    #if UNITY_EDITOR
    //    UnityEditor.EditorApplication.isPlaying = false;
    //    #else
    //        Application.Quit();
    //    #endif

    //}
    //public void ExitButtonNoClicked()
    //{
    //    MusicManager.GetInstance().PlayEnvironmentSound("����ѡ����");
    //    ExitImage.gameObject.SetActive(false);
    //}







    //public void Show()
    //{
    //    gameObject.SetActive(true);
    //}

}
