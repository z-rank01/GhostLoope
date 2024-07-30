using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textMeshPro;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("In NPC SetText Start");
        textMeshPro.SetText("按Y进行交互");
    }
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("NPC OntriggerEnter");
    }
    // Update is called once per frame
    void Update()
    {
        
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (player == null) return;
        Vector3 playerPosition = player.transform.position;
        float distance = (playerPosition - transform.position).magnitude;
        //Debug.Log("distance: " + distance);
        if (distance < 2f)
        {
            textMeshPro.gameObject.SetActive(true);
        }
        else
        {
            textMeshPro.gameObject.SetActive(false);
        }
    }
}
