using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class StartingMenu : MonoBehaviour
{


    public Button Setting;
    public Image SettingImage;
    public Button ExitSetting;

    public Slider BackMusicSlider;
    public Slider EnviromentSlider;
    public Slider FireSlider;

    // ��ҵ�Ѫ����������
    public Slider San;
    public Slider Resilience;

    // Boss��Ѫ����������
    public Slider Enemy_San;
    public Slider Enemy_Res;

    private Vector2 OriginalPosSan;
    private Vector2 OriginalPosRes;

    private Vector2 NotFightingPosSan;
    private Vector2 NotFightingPosRes;
    private Vector2 Offset = new Vector2(0,1000000);

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

    public Image GameEndImage; // ��Ϸ������Ĳ���ͼƬ

    int FadeAlpha = 10; // ��¶���������ٶȣ�FadeAlpha ֡��FadeImage������ʧ
    public Image FadeImage; // ��ʼ��Ϸ���ɺ�ɫ�𽥱�Ϊ͸������¶������

    // ��TextMeshProUGUI������ȷ��ֵ
    public TextMeshProUGUI updatePropertyText; // ������ʾ�ı�

    // Start is called before the first frame update
    void Start()
    {
        //updatePropertyText.SetText("WWWW");


        San.maxValue = GlobalSetting.GetInstance().san;
        Resilience.maxValue = GlobalSetting.GetInstance().resilience;

        OriginalPosSan = Enemy_San.GetComponent<RectTransform>().anchoredPosition;
        OriginalPosRes = Enemy_Res.GetComponent<RectTransform>().anchoredPosition;

        // ��ʼʱBossѪ������ʾ
        NotFightingPosSan = Enemy_San.GetComponent<RectTransform>().anchoredPosition + Offset;
        NotFightingPosRes = Enemy_Res.GetComponent<RectTransform>().anchoredPosition + Offset;


        Setting.onClick.AddListener(this.SettingButtonClicked);
        ExitSetting.onClick.AddListener(this.ExitSettingClicked);


        PauseGame.onClick.AddListener(this.PauseGameClicked);
        ContinueGame.onClick.AddListener(this.ContinueGameClicked);
        OperationGuide.onClick.AddListener(this.OperationGuideClicked);
        MusicSetting.onClick.AddListener(this.SettingButtonClicked); // ������������ð�ťһ����Ч��
        ReturnMainMenu.onClick.AddListener(this.ReturnMainMenuClicked);


        GuideReturn.onClick.AddListener(this.GuideReturnClicked);


        if (BeginGame.isLoadGameClicked)
        {
            BeginGame.isLoadGameClicked = false; // ��Ҫ�û�false����Ȼÿ�λص������涼���Զ����������Ϸ
            // �����ڳ����л�ǰ��BeiginGame�е���
            // ֻ�����³���������ɺ�Ķ����е��ã�
            // SaveManager.GetInstance().LoadGame();
        }
        
        if (BeginGame.isNewGameClicked)
        {

            MusicManager.GetInstance().PlayFireSound("��һ��-����");


            MusicManager.GetInstance().PlayBackgroundMusic("��һ��-����");
            MusicManager.GetInstance().PlayEnvironmentSound("��һ��-������-΢��");
            BeginGame.isNewGameClicked = false;
            BeginGame.isLoadGameClicked = false;
        }


        FadeImage.gameObject.SetActive(true);
    }

    void UpdateSanAndRes(Slider san, Slider res, float maxSan, float maxRes)
    {
        



        // ����Ѫ���ĳ���
        float width_San = san.GetComponent<RectTransform>().sizeDelta.x;

        // �����������ĳ���
        float width_Res = res.GetComponent<RectTransform>().sizeDelta.x;

        // Ѫ����������
        float left_San = width_San / 2 - san.value / maxSan * 0.5f * width_San;

        // ��������������
        float left_Res = width_Res / 2 - res.value / maxRes * 0.5f * width_Res;

        // �̶�ê��
        san.fillRect.anchorMin = new Vector2(0, 0);
        san.fillRect.anchorMax = new Vector2(1, 1);
        res.fillRect.anchorMin = new Vector2(0, 0);
        res.fillRect.anchorMax = new Vector2(1, 1);

        // �޸�left, bottom
        san.fillRect.offsetMin = new Vector2(left_San, 0);
        res.fillRect.offsetMin = new Vector2(left_Res, 0);

        // �޸�right, top
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

    // ��Ϸ������ص�������Ķ����ٶ�
    private int endAlpha = 100;
    void UpdateGameEndImage()
    {
        if (endAlpha > 0)
        {
            endAlpha--;
            GameEndImage.GetComponent<Image>().color = new Color(0, 0, 0, 1.0f -(endAlpha * 1.0f / 100));
            if (endAlpha == 0)
            {
                //GameObject.Destroy(GameEndImage);
                SceneManager.LoadScene("BeginGame");
            }
        }
    }

    IEnumerator ShowUpdatePropertyText()
    {
        updatePropertyText.gameObject.SetActive(true);
        updatePropertyText.SetText(Player.GetInstance().showText);
        for (int i = 0; i <= 20; i++)
        {
            updatePropertyText.color = new Color(1, 1, 1, 1 - i * 1.0f / 20);
            yield return new WaitForSeconds(0.1f);
        }

        updatePropertyText.gameObject.SetActive(false);

    }
    // Update is called once per frame
    void Update()
    {
        // ��Ϸ�������Ŷ���

        if (Player.GetInstance().GetIsGameEnd())
        {
            GameEndImage.gameObject.SetActive(true);
            UpdateGameEndImage();
        }
        // ��ʾ�����ı�
        if (Player.GetInstance().isNeedToShowText)
        {
            Player.GetInstance().isNeedToShowText = false;
            StartCoroutine(ShowUpdatePropertyText());
        }

        // ��̬�������ֵ
        San.maxValue = GlobalSetting.GetInstance().san;
        Resilience.maxValue = GlobalSetting.GetInstance().resilience;

        float height = San.GetComponent<RectTransform>().sizeDelta.y;

        // ���ȸ�����ҵ����԰��������ӣ� �߶ȱ��ֲ��� (Ŀǰ�ı���Ϊ4��10����100������ֵ��Ӧ400����λ�����������ȡ�40������ֵ��Ӧ400����λ�����������ȣ�
        San.GetComponent<RectTransform>().sizeDelta = new Vector2(San.maxValue * 4, height);
        Resilience.GetComponent<RectTransform>().sizeDelta = new Vector2(Resilience.maxValue * 10, height);



        San.value = Player.GetInstance().GetProperty(E_Property.san);
        Resilience.value = Player.GetInstance().GetProperty(E_Property.resilience);

        // ������ҵ�Ѫ����������
        UpdateSanAndRes(San, Resilience, San.maxValue, Resilience.maxValue);

        // ����Bossս״̬����ʾ�͸���Boss��Ѫ����������
        if (Player.GetInstance().GetIsFightingBoss())
        {
            //Debug.Log("Enemy_San.transform.position: " + Enemy_San.GetComponent<RectTransform>());


            Enemy_San.GetComponent<RectTransform>().anchoredPosition = OriginalPosSan;
            Enemy_Res.GetComponent<RectTransform>().anchoredPosition = OriginalPosRes;

            //UpdateSanAndRes(Enemy_San, Enemy_Res, 100, 40);
        }
        else
        {
            Enemy_San.GetComponent<RectTransform>().anchoredPosition = NotFightingPosSan;
            Enemy_Res.GetComponent<RectTransform>().anchoredPosition = NotFightingPosRes;
        }

        // ��̬�޸�������С
        UpdateMusicVolume();

        // ���ų������ֶ���
        UpdateFadeImage();
    }


    // ���Ű�ť���µ�����
    public void PlayButtonClickedSound()
    {
        MusicManager.GetInstance().PlayFireSound("����ѡ����");
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
