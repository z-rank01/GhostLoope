using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Property
{
    san, 
    resilience, 
    speed,
    dashSpeed
}

public class PlayerProperty : MonoBehaviour
{
    [SerializeField]
    private float san;
    [SerializeField]
    private float resilience;
    [SerializeField]
    private float speed;

    [SerializeField]
    private float dashSpeed;


    private void Update()
    {
        
    }

    public void SetProperty(E_Property eProperty, object property)
    {
        switch (eProperty)
        {
            case E_Property.san:
                san = (float)property; break;
            case E_Property.resilience:
                resilience = (float)property;break;
            case E_Property.speed:
                speed = (float)property;break;
            case E_Property.dashSpeed:
                dashSpeed = (float)property; break;
        }
    }

    public float GetProperty(E_Property eProperty)
    {
        switch (eProperty)
        {
            case E_Property.san:
                return san;
            case E_Property.resilience:
                return resilience;
            case E_Property.speed:
                return speed;
            case E_Property.dashSpeed:
                return dashSpeed;
            default:
                Debug.Log("Fail to get property.");
                return default(float);
        }
    }
}
