using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public Button NewGame;


    public Button Setting;
    public Image SettingImage;
    public Button ExitSetting;

    public Slider MusicSetting;


    public Button ExitGame;
    public Image ExitImage;
    public Button ExitGameYes;
    public Button ExitGameNo;


    public Slider SAN;
    public Slider Resilience;

    // Start is called before the first frame update
    void Start()
    {

        SAN.maxValue = GlobalSetting.GetInstance().san;

        Resilience.maxValue = 40;

        NewGame.onClick.AddListener(this.NewGameButtonClicked);
        Setting.onClick.AddListener(this.SettingButtonClicked);
        ExitGame.onClick.AddListener(this.ExitButtonClicked);
        ExitGameYes.onClick.AddListener(this.ExitButtonYesClicked);
        ExitSetting.onClick.AddListener(this.ExitSettingClicked);
        ExitGameNo.onClick.AddListener(this.ExitButtonNoClicked);

        
    }

    // Update is called once per frame
    void Update()
    {


        //Debug.Log("In DialogueUI Update: " + SAN.value);
        SAN.value = Player.GetInstance().GetProperty(E_Property.san);
        Resilience.value = Player.GetInstance().GetProperty(E_Property.resilience);


        MusicManager.GetInstance().ChangeBKValue(MusicSetting.value);



        //SAN.value = Player.GetInstance().GetProperty(E_Property.san);
        //Resilience.value = Player.GetInstance().GetProperty(E_Property.resilience);
        //Debug.Log("Up Music Key Down" + MusicSetting.value);
    }

    
    public void SettingButtonClicked()
    {
        MusicManager.GetInstance().PlaySound("界面选择音");
        SettingImage.gameObject.SetActive(true);
    }


    public void ExitSettingClicked()
    {
        MusicManager.GetInstance().PlaySound("界面选择音");
        SettingImage.gameObject.SetActive(false);
    }
    public void ExitButtonClicked()
    {
        MusicManager.GetInstance().PlaySound("界面选择音");
        ExitImage.gameObject.SetActive(true);
    }
    public void ExitButtonYesClicked()
    {
        MusicManager.GetInstance().PlaySound("界面选择音");

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif

    }
    public void ExitButtonNoClicked()
    {
        MusicManager.GetInstance().PlaySound("界面选择音");
        ExitImage.gameObject.SetActive(false);
    }







    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void NewGameButtonClicked()
    {
        MusicManager.GetInstance().PlaySound("界面选择音");
        Debug.Log("new GameButtonClicked");
        MusicManager.GetInstance().PlayBkMusic("第一关-配乐");
        gameObject.SetActive(false);
    }
}
