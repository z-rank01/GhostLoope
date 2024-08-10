using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Property
{
    san, 
    resilience, 
    speed,
    dashSpeed,
    slowSpeed,
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

    [SerializeField]
    private float slowSpeed;



    private void Update()
    {
        
    }

    public void SetProperty(E_Property eProperty, object property)
    {
        switch (eProperty)
        {
            case E_Property.san:
                san = Mathf.Clamp((float)property, 0, GlobalSetting.GetInstance().san); // 防止越界
                break;
            case E_Property.resilience:
                resilience = Mathf.Clamp((float)property, 0, GlobalSetting.GetInstance().resilience); // 防止越界
                break;
            case E_Property.speed:
                speed = (float)property;break;
            case E_Property.dashSpeed:
                dashSpeed = (float)property; break;
            case E_Property.slowSpeed:
                slowSpeed = (float)property; break;

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
            case E_Property.slowSpeed:
                return slowSpeed;
            default:
                Debug.Log("Fail to get property.");
                return default(float);
        }
    }
}
