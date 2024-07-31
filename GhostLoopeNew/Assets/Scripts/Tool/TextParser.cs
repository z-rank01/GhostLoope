using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextParser
{
    TextAsset textAsset;
    List<string> context;
    int cursor = 0;

    public TextParser()
    {
        this.textAsset = new TextAsset();
        this.context = new List<string>();
    }

    public void Parse(TextAsset textAsset)
    {
        this.textAsset = textAsset;
        string[] newContext = textAsset.text.Split('n');
        Debug.Log(textAsset.text);
        foreach (string ctx in newContext)
        {
            context.Add(ctx);
        }
        Debug.Log(context.Count);
    }

    public (string, string) GetSpeakerAndSentence()
    {
        // speaker
        string[] currLine = context[cursor].Split(':');
        
        return (currLine[0], currLine[1]);
    }

    public bool NextLine()
    {
        if (cursor >= context.Count) 
            return false;
        else
            cursor++;
            return true;
    }

    public void ClearTextParser()
    {
        textAsset = null;
        context = null;
        cursor = 0;
    }
}
