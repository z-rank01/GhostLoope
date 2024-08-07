using UnityEngine;

public class Tenacity : MonoBehaviour
{
    public float tenacity = 100.0f;
    public float currTenacity;
    public GameObject parentObj;

    protected void Start()
    {
        parentObj = this.gameObject.transform.parent.gameObject;
        currTenacity = tenacity;
    }

    protected void Update()
    {
        Rotating();
    }

    protected void Rotating()
    {
        gameObject.transform.RotateAround(parentObj.transform.position, Vector3.up, Time.time);
    }

    protected void ReceiveDamage(Bullet bullet)
    {
        currTenacity -= bullet.damage;
        EventCenter.GetInstance().EventTrigger(E_Event.TenacityReceiveDamage);
    }

    // interface
    public bool CheckTenacityEqualZero()
    {
        return currTenacity <= 0 ? true : false;
    }

    public void RefreshTenacityValue()
    {
        currTenacity = tenacity;
    }
}