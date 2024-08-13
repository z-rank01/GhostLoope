using System.Collections.Generic;
using UnityEngine;

public class Tenacity : MonoBehaviour
{
    [Header("Tenacity Setting")]
    public float tenacity = 100.0f;
    public float extraDamage = 100.0f;
    public float rotateSpeed = 5.0f;
    private float currTenacity;

    [Header("Spawn Bullet Setting")]
    public E_PoolType spawnBulletType;
    public int bulletNumber = 3;
    public float bulletSpawnDistance = 3.0f;
    public int brokenSeconds = 5;

    private GameObject parentObj;
    private List<GameObject> spawnedBullets;
    private int bulletOnScene;
    private bool hasSpawnedBullet = false;

    private float currTotalTime;
    private int currSeconds;

    protected void Update()
    {
        Rotating();

        // update counter
        currTotalTime += Time.deltaTime;
        currSeconds = (int)(currTotalTime % 60);
    }

    protected void Rotating()
    {
        transform.RotateAround(parentObj.transform.position, Vector3.up, rotateSpeed);
    }

    

    // event interface
    public void DecreaseBulletNumber(GameObject spawnBullet)
    {
        bulletOnScene--;
        spawnedBullets.Remove(spawnBullet);
    }

    public void ReceiveDamage(Bullet bullet)
    {
        //Debug.Log("ReceiveDamage");
        EventCenter.GetInstance().EventTrigger<float>(E_Event.TenacityReceiveDamage, extraDamage);
    }


    // general interface
    public void Init()
    {
        currTenacity = tenacity;
        bulletOnScene = 0;
        spawnedBullets = new List<GameObject>();

        EventCenter.GetInstance().AddEventListener<GameObject>(E_Event.TenacityBulletReturn, DecreaseBulletNumber);
    }

    public float GetCurrentTenacity()
    {
        return currTenacity;
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

    public bool CheckCounterFinish()
    {
        return currSeconds < brokenSeconds;
    }


    public void DisableTenacity()
    {
        for (int i = 0; i < spawnedBullets.Count; i++)
        {
            PoolManager.GetInstance().ReturnObj(spawnBulletType, spawnedBullets[i]);
        }
        spawnedBullets.Clear();
        gameObject.SetActive(false);
    }

    public void ResetTanacity()
    {
        currTenacity = tenacity;
        bulletOnScene = 0;
        hasSpawnedBullet = false;
        currTotalTime = 0.0f;
        currSeconds = 0;
    }

    public void SetTenacityParent(GameObject parentObject)
    {
        transform.SetParent(parentObject.transform);
        parentObj = parentObject;
    }

    public void SpawnBullet()
    {
        if (!hasSpawnedBullet)
        {
            bulletOnScene = bulletNumber;
            

            for (int i = 1; i <= bulletNumber; i++)
            {
                GameObject bulletObj = PoolManager.GetInstance().GetObj(spawnBulletType);
                bulletObj.AddComponent<BulletCounter>();
                spawnedBullets.Add(bulletObj);

                Bullet bullet = bulletObj.GetComponent<Bullet>();
                float angle = Mathf.Lerp(0, 360.0f, (float)i / (float)bulletNumber);
                Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * parentObj.transform.forward;
                bullet.FireOut(parentObj.transform.position + direction * bulletSpawnDistance, 
                               Vector3.zero, 
                               0);
            }

            hasSpawnedBullet = true;
        }
    }
}