using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TextParser
{
    List<string> context;
    int cursor = 0;

    public TextParser()
    {
        this.context = new List<string>();
    }

    public void Parse(TextAsset newTextAsset)
    {
        string[] newContext = newTextAsset.text.Split('\n');
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
        if (cursor + 1 >= context.Count)
        {
            cursor = 0;
            return false;
        }
        else
        {
            cursor++;
            return true;
        }
            
    }

    public void ResetCursor()
    {
        cursor = 0;
    }

    public void ClearTextParser()
    {
        context.Clear();
        cursor = 0;
    }
}
