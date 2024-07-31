using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    private float bulletSpeed;
    private Vector3 fireDirection;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Flying();
        if (!CheckWithinScreen())
        {
            Debug.Log("OUT OF SCREEN");
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnvironmentObject"))
        {
            Debug.Log("Collider Boooommm!!");
            PoolManager.GetInstance().ReturnObj(E_PoolType.SimpleBullet, gameObject);
        }

        if (other.gameObject.CompareTag("Boundary"))
        {
            Debug.Log("OUT OF BOUNDARY");
            PoolManager.GetInstance().ReturnObj(E_PoolType.SimpleBullet, gameObject);
        }
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


    public bool CheckWithinScreen()
    {
        Vector3 viewPortPosition = Camera.main.WorldToViewportPoint(transform.position);

        if (viewPortPosition.x >= 0 && viewPortPosition.x <= 1
            &&
            viewPortPosition.y >= 0 && viewPortPosition.y <= 1
            &&
            viewPortPosition.z > 0)
            return true;
        else return false;
    }
}
