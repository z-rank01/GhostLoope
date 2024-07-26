using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Test : MonoBehaviour
{
    private PoolManager poolManager;

    [SerializeField]
    private GameObject bulletPrefab;

    private List<GameObject> bullets = new List<GameObject> ();


    // Start is called before the first frame update
    void Start()
    {
        poolManager = gameObject.AddComponent<PoolManager>();
        poolManager.Init(bulletPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject newBullet = poolManager.GetObj(E_PoolType.Bullet);
            bullets.Add(newBullet);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (bullets.Count > 0)
            {
                poolManager.ReturnObj(E_PoolType.Bullet, bullets[0]);
                bullets.RemoveAt(0);
            }   
        }
    }
}
