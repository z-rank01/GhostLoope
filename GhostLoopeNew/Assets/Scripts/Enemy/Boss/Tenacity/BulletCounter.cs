using UnityEngine;

public class BulletCounter : MonoBehaviour
{
    private void OnDisable()
    {
        EventCenter.GetInstance().EventTrigger<GameObject>(E_Event.TenacityBulletReturn, this.gameObject);
        Destroy(this);
    }
}