using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public enum E_ConversationClip
{
    Passage1, 
    Passage2,
    Passage3
}

public class ConversationHUD : BaseSingletonMono<ConversationHUD>
{
    [SerializeField]
    TextAsset  currTextAsset;
    TMPro.TextMeshPro textMeshPro;
    TextParser textParser;

    // current information
    string speaker;
    string sentence;

    // Start is called before the first frame update
    void Awake()
    {
        base.Init();
        currTextAsset = new TextAsset();
        textParser = new TextParser();
        //textMeshPro = new TMPro.TextMeshPro();
    }

    // interface
    public void Speak()
    {
        (speaker, sentence) = textParser.GetSpeakerAndSentence();
        GameObject speakerObj = GameObject.Find(speaker);
        if (speakerObj != null)
        {
            textMeshPro.SetText(sentence);
            textMeshPro.transform.parent = speakerObj.transform;
            textMeshPro.transform.localPosition = Vector3.one;
            textMeshPro.gameObject.SetActive(true);
        }
        else
            Debug.LogError("Fail to find speaker! Please Check the text");
    }

    public void NextLine()
    {
        textParser.NextLine();
    }

    public void LoadConversation(E_ConversationClip clip)
    {
        if (GetText(clip))
            textParser.Parse(currTextAsset);
        else
            Debug.Log("Fail to get text file");
    }

    public void UnloadConversation()
    {
        currTextAsset = null;
        textMeshPro.transform.parent = transform;
        textMeshPro.gameObject.SetActive(false);
        textParser.ClearTextParser();
    }


    // function
    private bool GetText(E_ConversationClip clip)
    {
        string path = "Passages/" + clip;
        ResourcesManager.GetInstance().LoadResourceAsync("Passages/" + clip.ToString(), GetTextResource);
        if (currTextAsset == null) return false;
        return true;
    }

    private void GetTextResource(object textAsset)
    {
        currTextAsset = textAsset as TextAsset;
    }
}
