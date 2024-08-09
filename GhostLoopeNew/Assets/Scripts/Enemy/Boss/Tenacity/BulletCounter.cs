using UnityEngine;

public class BulletCounter : MonoBehaviour
{
    
    void OnDisable()
    {
        EventCenter.GetInstance().EventTrigger(E_Event.TenacityBulletReturn);
        Destroy(this);
    }
}