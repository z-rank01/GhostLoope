using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    private float bulletSpeed;
    private Vector3 fireDirection;

    // Update is called once per frame
    void Update()
    {
        Flying();
    }


    // interface
    public void FireOut(Vector3 position, Vector3 fireDirection, float bulletSpeed)
    {
        transform.position = position;
        this.fireDirection = fireDirection;
        this.bulletSpeed = bulletSpeed;
    }

    public void Flying()
    {
        transform.position += fireDirection * bulletSpeed;
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Boooommm!!");
        PoolManager.GetInstance().ReturnObj(E_PoolType.SimpleBullet, gameObject);
    }
}
