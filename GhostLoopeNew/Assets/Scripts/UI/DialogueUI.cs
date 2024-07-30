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


    // Start is called before the first frame update
    void Start()
    {
        NewGame.onClick.AddListener(this.Hide);
        Setting.onClick.AddListener(this.SettingButtonClicked);
        ExitGame.onClick.AddListener(this.ExitButtonClicked);
        ExitGameYes.onClick.AddListener(this.ExitButtonYesClicked);
        ExitSetting.onClick.AddListener(this.ExitSettingClicked);
        ExitGameNo.onClick.AddListener(this.ExitButtonNoClicked);

        
    }

    // Update is called once per frame
    void Update()
    {
        MusicManager.GetInstance().ChangeBKValue(MusicSetting.value);
        //Debug.Log("Up Music Key Down" + MusicSetting.value);
    }

    
    public void SettingButtonClicked()
    {
        
        SettingImage.gameObject.SetActive(true);
    }


    public void ExitSettingClicked()
    {
        SettingImage.gameObject.SetActive(false);
    }
    public void ExitButtonClicked()
    {
        ExitImage.gameObject.SetActive(true);
    }
    public void ExitButtonYesClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif

    }
    public void ExitButtonNoClicked()
    {
        ExitImage.gameObject.SetActive(false);
    }







    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
