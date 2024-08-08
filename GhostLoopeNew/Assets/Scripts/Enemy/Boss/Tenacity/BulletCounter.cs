using UnityEngine;

public class BulletCounter : MonoBehaviour
{
    public void OnDisable()
    {
        EventCenter.GetInstance().EventTrigger(E_Event.TenacityBulletReturn);
        Destroy(this);
    }
}