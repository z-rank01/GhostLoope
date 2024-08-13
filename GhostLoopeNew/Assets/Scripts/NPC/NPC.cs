using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public List<TextAsset> textAssets;

    private NPCConversation conversation;
    private int cursorOfPassage;
    private bool readyToInteract = false;
    private float detectDistance = 5f;
    

    void Awake()
    {
        
    }

    void OnEnable()
    {
        conversation = GetComponent<NPCConversation>();
        conversation.Init();

        foreach (var item in textAssets)
        {
            conversation.LoadPassages(item);
        }
    }

    void OnDisable()
    {
        
    }

    void Update()
    {
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            readyToInteract = true;
            // Debug.Log("NPC Status: " + readyToInteract);
            // main event
            conversation.ParsePassage(cursorOfPassage);
            conversation.ShowHint();
            EventCenter.GetInstance().AddEventListener(E_Event.Conversation, Conversation);
        }
        
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            readyToInteract = false;
            // Debug.Log("NPC Status: " + readyToInteract);
            // main event
            conversation.UnloadPassage();
            EventCenter.GetInstance().RemoveEventListener(E_Event.Conversation, Conversation);
        }
    }

    public void Conversation()
    {
        if (readyToInteract)
        {
            conversation.Speak();
            if (!conversation.NextLine())
            {
                conversation.UnloadPassage();
                readyToInteract = false;
            }
        }
    }

}
