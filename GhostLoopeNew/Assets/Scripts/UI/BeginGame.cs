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


    public Image UpperImage; // ��������ϰ벿��
    public Image LowerImage; // ��������°벿��

    public Image SettingImage; // ��Ч����


    public Button ExitSetting; // �˳���Ч���� 

    public Slider BackMusicSlider;
    public Slider EnviromentSlider;
    public Slider FireSlider;

    public Image SelectImage; // �ŷ����ֽ���

    // ʹ�þ�̬��������StartingMenu���з��ʸñ���
    public static bool isNewGameClicked = false; //����Ϸ�����س�ʼ����
    public static bool isLoadGameClicked = false; // ������Ϸ�����س�ʼ��������Ҫ�������Json�ļ�



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

    bool isReadyToOpenLower = false; // �Ƿ�׼����Ҫ��ʼ�����°�ҳ

    
    private void Update()
    {
        // �޸�position����ʵ��ͼƬ�ƶ���Ч��
        // UpperImage.rectTransform.anchoredPosition += new Vector2(0, 10); 


        // �޸�rotationʵ�ִ��ŷ��Ч��

        // �ȴ��ϰ�ҳ
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

        // �ٴ��°�ҳ
        if (isReadyToOpenLower && frame_count >= 0 && frame_count < 90)
        {
            frame_count++;
            LowerImage.rectTransform.localRotation = Quaternion.Euler(frame_count * -1, 0, 0);

            if (frame_count == 90)
            {
                isReadyToOpenLower = false;

                // ����Ϸ��ֻ���س������ɣ�����ڳ�ʼλ��
                if (isNewGameClicked)
                {
                    SaveManager.GetInstance().san = 300;
                    SaveManager.GetInstance().res = 30;

                    SceneManager.LoadScene("Level1");
                }
                // ������Ϸ�����س����󣬼����ѱ����ļ�����ҵ�λ�ú���Ϣ
                else if (isLoadGameClicked)
                {
                    SaveManager.GetInstance().LoadGame();
                }
                Debug.Log("Load Level1 End!");

                //SceneManager.UnloadSceneAsync("BeginGame");
            }
        }


        // ��̬��������
        MusicManager.GetInstance().ChangeBackgroundValue(BackMusicSlider.value);
        MusicManager.GetInstance().ChangeEnvironmentValue(EnviromentSlider.value);
        MusicManager.GetInstance().ChangeFireValue(FireSlider.value);


    }
    public void NewButtonClicked()
    {
        MusicManager.GetInstance().PlayFireSound("����ѡ����");


        SelectImage.gameObject.SetActive(false);
        isNewGameClicked = true;


    }
    public void LoadButtonClicked()
    {
        MusicManager.GetInstance().PlayFireSound("����ѡ����");

        SelectImage.gameObject.SetActive(false);

        isLoadGameClicked = true;




    }
    public void MusicButtonClicked()
    {
        MusicManager.GetInstance().PlayFireSound("����ѡ����");


        SettingImage.gameObject.SetActive(true);
    }


    public void ExitMusicClicked()
    {
        MusicManager.GetInstance().PlayFireSound("����ѡ����");
        SettingImage.gameObject.SetActive(false);
    }


    public void ExitButtonClicked()
    {
        MusicManager.GetInstance().PlayFireSound("����ѡ����");

        // �����������˳���ť�������˳���Ϸ
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
