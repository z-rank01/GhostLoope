using System.Collections.Generic;
using UnityEngine;

public class Tenacity : MonoBehaviour
{
    [Header("Tenacity Setting")]
    public float tenacity = 100.0f;
    public float rotateSpeed = 5.0f;
    private float currTenacity;

    [Header("Spawn Bullet Setting")]
    public E_PoolType spawnBulletType;
    public int bulletNumber = 3;
    public float bulletSpawnDistance = 3.0f;


    private GameObject parentObj;
    private List<Vector3> spawnPosition;
    private int bulletOnScene;
    private bool hasSpawnedBullet = false;


    protected void Update()
    {
        Rotating();
    }

    protected void Rotating()
    {
        transform.RotateAround(parentObj.transform.position, Vector3.up, rotateSpeed);
    }

    

    // event interface
    public void DecreaseBulletNumber()
    {
        bulletOnScene--;
    }

    public void ReceiveDamage(Bullet bullet)
    {
        //Debug.Log("ReceiveDamage");
        EventCenter.GetInstance().EventTrigger<float>(E_Event.TenacityReceiveDamage, bullet.damage);
    }


    // general interface
    public void Init()
    {
        currTenacity = tenacity;
        bulletOnScene = 0;
        spawnPosition = new List<Vector3>();

        EventCenter.GetInstance().AddEventListener(E_Event.TenacityBulletReturn, DecreaseBulletNumber);
    }

    public void DecreaseTenacity(float damage)
    {
        currTenacity -= currTenacity - damage >= 0 ? damage : currTenacity;
    }

    public bool CheckBulletOnScene()
    {
        return bulletOnScene == 0 ? false : true;
    }

    public bool CheckTenacityEqualZero()
    {
        return currTenacity <= 0 ? true : false;
    }

    public void ResetTanacity()
    {
        currTenacity = tenacity;
        bulletOnScene = 0;
        hasSpawnedBullet = false;
    }

    public void SetTenacityParent(GameObject parentObject)
    {
        transform.SetParent(parentObject.transform);
        parentObj = parentObject;

        // initialize bullet spawn position
        for (int i = 0; i < bulletNumber; i++)
        {
            Vector3 direction = Quaternion.AngleAxis(360.0f / (float)i, Vector3.up) * parentObj.transform.forward;
            direction = direction.normalized;
            spawnPosition.Add(parentObj.transform.position + direction * bulletSpawnDistance);
        }
    }

    public void SpawnBullet()
    {
        if (!hasSpawnedBullet)
        {
            bulletOnScene = bulletNumber;
            for (int i = 0; i < bulletNumber; i++)
            {
                GameObject bulletObj = PoolManager.GetInstance().GetObj(spawnBulletType);
                bulletObj.AddComponent<BulletCounter>();
                Bullet bullet = bulletObj.GetComponent<Bullet>();
                bullet.FireOut(spawnPosition[i], Vector3.zero, 0);
            }
            hasSpawnedBullet = true;
        }
    }
}