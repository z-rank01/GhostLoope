using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveStone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            Debug.Log(bullet.bulletType + " Hit GraveStone!");
            Destroy(gameObject);
            PoolManager.GetInstance().ReturnObj(bullet.bulletType, bullet.gameObject);


            // 增加属性上限
            GlobalSetting.GetInstance().san += GlobalSetting.GetInstance().treasureSan;
            GlobalSetting.GetInstance().resilience += GlobalSetting.GetInstance().treasureRes;
        }
    }
}
