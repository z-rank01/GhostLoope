using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class NPCConversation : MonoBehaviour
{
    public GameObject textMeshProObject;
    public Vector3 Offset;
    
    private List<TextAsset> passages;
    private TextParser textParser;
    private TMPro.TextMeshProUGUI textMeshProUGUI;

    private bool showText = false;
    private GameObject speakerObj;
    private string speakContent;

    private void Update()
    {
        if (showText)
        {
            ShowText(speakContent, speakerObj);
        }
    }

    // interface
    public void Init()
    {
        passages = new List<TextAsset>();
        textParser = new TextParser();
        textMeshProUGUI = textMeshProObject.GetComponent<TMPro.TextMeshProUGUI>();
        if (textMeshProUGUI == null)
        {
            textMeshProUGUI = GameObject.FindGameObjectWithTag("ConversationHUD").GetComponent<TMPro.TextMeshProUGUI>();
        }
    }

    public void LoadPassages(string pathToPassage)
    {
        ResourcesManager.GetInstance().LoadResourceAsync(pathToPassage, GetTextResource);
    }

    public void LoadPassages(TextAsset asset)
    {
        passages.Add(asset);
    }

    public void ShowHint()
    {
        showText = true;
        speakerObj = this.gameObject;
        speakContent = "E";
    }

    public void ParsePassage(int idx)
    {
        // parse text
        textParser.Parse(passages[idx]);
    }

    private void ShowText(string textContent, GameObject speakerObj)
    {
        textMeshProUGUI.SetText(textContent);
        textMeshProObject.transform.position = Camera.main.WorldToScreenPoint(speakerObj.transform.position + Offset);
        textMeshProObject.SetActive(true);
    }

    public void Speak()
    {
        // show text on target textMeshPro
        (string speaker, string sentence) = textParser.GetSpeakerAndSentence();
        speakerObj = GameObject.Find(speaker);
        speakContent = speaker + ": " + sentence;
        showText = true;
    }

    public void UnloadPassage()
    {
        showText = false;
        textParser.ResetCursor();
        textMeshProObject.transform.localPosition = Vector3.zero;
        textMeshProObject.SetActive(false);
    }

    public bool NextLine()
    {
        return textParser.NextLine();
    }

    public int CheckPassageCount()
    {
        return passages.Count;
    }


    // private function
    private void GetTextResource(object textAsset)
    {
        passages.Add(textAsset as TextAsset);
    }

}