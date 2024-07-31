using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textMeshPro;
    private bool readyToInteract = false;

    // Start is called before the first frame update
    void Start()
    {

    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            readyToInteract = true;
            textMeshPro.gameObject.SetActive(true);
            Debug.Log("NPC Status: " + readyToInteract);
        }
        
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            readyToInteract = false;
            textMeshPro.gameObject.SetActive(false);
            Debug.Log("NPC Status: " + readyToInteract);
        }
    }


    //// Update is called once per frame
    //void Update()
    //{
        
    //    Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    //    if (player == null) return;
    //    Vector3 playerPosition = player.transform.position;
    //    float distance = (playerPosition - transform.position).magnitude;
    //    //Debug.Log("distance: " + distance);
    //    if (distance < 2f)
    //    {
    //        textMeshPro.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        textMeshPro.gameObject.SetActive(false);
    //    }
    //}
}
