using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{


    public Slider SAN;
    public Slider Strength;
    // Start is called before the first frame update
    void Start()
    {
        SAN.value = Strength.value = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SAN.value -= 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            SAN.value += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Strength.value -= 0.1f;
        }
    }
}
