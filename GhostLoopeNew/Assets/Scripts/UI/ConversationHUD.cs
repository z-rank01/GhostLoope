using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public enum E_ConversationClip
{
    Passage1, 
    Passage2,
    Passage3
}

public class ConversationHUD : MonoBehaviour
{
    [SerializeField]
    private TextAsset  currTextAsset;
    [SerializeField]
    private TMPro.TextMeshProUGUI textMeshPro;
    private TextParser textParser;
    private Transform originParent;

    // current information
    string speaker;
    string sentence;

    void OnEnable()
    {
        textParser = new TextParser();
        textMeshPro = GetComponent<TextMeshProUGUI>();
        originParent = this.transform.parent;
        gameObject.SetActive(false);
    }

    // interface
    public void Speak()
    {
        (speaker, sentence) = textParser.GetSpeakerAndSentence();
        GameObject speakerObj = GameObject.Find(speaker);
        if (speakerObj != null)
        {
            textMeshPro.SetText(speaker + ": " + sentence);
            transform.parent = speakerObj.transform;
            transform.localPosition = Vector3.zero;
            transform.LookAt(Camera.main.transform);
            //transform.position = Camera.main.WorldToScreenPoint(speakerObj.transform.position);
            //gameObject.SetActive(true);
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
        GetText(clip);
        if (currTextAsset != null)
        {
            Debug.Log(currTextAsset.text);
            textParser.Parse(currTextAsset);
        }
        else
            Debug.Log("Fail to get text file");
    }

    public void UnloadConversation()
    {
        currTextAsset = null;
        gameObject.SetActive(false);
        textParser.ClearTextParser();
        textMeshPro.SetText("");
    }


    // function
    private bool GetText(E_ConversationClip clip)
    {
        string path = "Passages/" + clip;
        ResourcesManager.GetInstance().LoadResource("Passages/" + clip.ToString(), GetTextResource);
        if (currTextAsset == null) return false;
        return true;
    }

    private void GetTextResource(object textAsset)
    {
        currTextAsset = textAsset as TextAsset;
    }
}
