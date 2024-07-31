using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{


    public Slider SAN;
    public Slider Resilience;
    // Start is called before the first frame update
    void Start()
    {
        SAN.maxValue = Player.GetInstance().GetProperty(E_Property.san);
        Resilience.maxValue = Player.GetInstance().GetProperty(E_Property.resilience);
    }

    // Update is called once per frame
    void Update()
    {
        
        SAN.value = Player.GetInstance().GetProperty(E_Property.san);
        Resilience.value = Player.GetInstance().GetProperty(E_Property.resilience);

    }
}
