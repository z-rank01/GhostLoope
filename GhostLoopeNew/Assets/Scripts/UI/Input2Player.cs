using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Input2Player : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {
    }



    void OnTriggerEnter(Collider other)
    {
        Debug.Log("In OnTriggerEnter Player");
    }



    // Update is called once per frame
    void Update()
    {
        //Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();



        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(horizontal, 0, vertical);
        //Debug.Log("dir: " + dir);
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
            transform.Translate(Vector3.forward * 2 * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {

            //transform.position += new Vector3()
            //Debug.Log(transform.position);
            //Debug.Log("W");
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {

        }
        else if (Input.GetKeyDown(KeyCode.A))
        {

        }
        else if (Input.GetKeyDown(KeyCode.D))
        {

        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("LeftShift键按下");
            //m_Player.SetSpeed(m_Player.m_ShiftSpeed);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Debug.Log("LeftShift键抬起");
            //m_Player.SetSpeed(m_Player.m_NormalSpeed);
        }
        if (Input.GetMouseButton(0))
        {
            Debug.Log("左键按下");
        }


        if (Input.GetMouseButton(1))
        {
            Debug.Log("右键按下");
        }

    }
}
