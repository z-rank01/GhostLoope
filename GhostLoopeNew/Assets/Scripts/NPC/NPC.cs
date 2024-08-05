using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private bool readyToInteract = false;
    private float detectDistance = 5f;
    public ConversationHUD conversationHUD;

    private void Awake()
    {
        
    }

    void OnEnable()
    {
        // load .txt file
        conversationHUD.LoadConversation(E_ConversationClip.Passage1);
    }

    private void OnDisable()
    {
        // unload .txt file
        conversationHUD.UnloadConversation();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            readyToInteract = true;
            Debug.Log("NPC Status: " + readyToInteract);
            // main event
            conversationHUD.gameObject.SetActive(true);
            EventCenter.GetInstance().AddEventListener(E_Event.Conversation, Conversation);
        }
        
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            readyToInteract = false;
            Debug.Log("NPC Status: " + readyToInteract);
            // main event
            conversationHUD.gameObject.SetActive(false);
            EventCenter.GetInstance().RemoveEventListener(E_Event.Conversation, Conversation);
        }
    }

    public void Conversation()
    {
        conversationHUD.Speak();
        conversationHUD.NextLine();
    }


    // Update is called once per frame
    void Update()
    {
    }

}
