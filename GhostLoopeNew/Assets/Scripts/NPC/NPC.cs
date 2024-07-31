using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textMeshPro;
    private bool readyToInteract = false;
    private float detectDistance = 5f;

    // Start is called before the first frame update
    void Start()
    {

    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            readyToInteract = true;
            Debug.Log("NPC Status: " + readyToInteract);

            // main event
            textMeshPro.gameObject.SetActive(true);
            ConversationHUD.GetInstance().LoadConversation(E_ConversationClip.Passage1);
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
            textMeshPro.gameObject.SetActive(false);
            ConversationHUD.GetInstance().UnloadConversation();
            EventCenter.GetInstance().RemoveEventListener(E_Event.Conversation, Conversation);
        }
    }

    public void Conversation(object nothing)
    {
        ConversationHUD.GetInstance().Speak();
        ConversationHUD.GetInstance().NextLine();
    }


    // Update is called once per frame
    void Update()
    {
    }

    private bool CheckRange()
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (player != null) return false;
        
        Vector3 playerPosition = player.transform.position;
        float distance = (playerPosition - transform.position).magnitude;
        
        if (distance < detectDistance) return true;
        else return false;
    }
}
