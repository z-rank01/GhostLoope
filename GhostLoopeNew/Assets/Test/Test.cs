using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;

public class Test : MonoBehaviour
{
    private PoolManager poolManager;

    [SerializeField]
    private GameObject bulletPrefab;

    private List<GameObject> bullets = new List<GameObject> ();
    private int cnt = 0;

    // Start is called before the first frame update
    void Start()
    {
        poolManager = gameObject.AddComponent<PoolManager>();
        poolManager.Init(bulletPrefab);
        EventCenter.GetInstance().AddEventListener(E_Event.LoadScene, LoadSceneProgress);
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
        else if (Input.GetKeyDown(KeyCode.A))
        {
            ScenesManager.GetInstance().LoadSceneAsync("TestScene", LoadScene);
        }


        if (cnt++ == 0)
            ResourcesManager.GetInstance().LoadResourceAsync<GameObject>("Bullet", LoadResources);
    }

    private void LoadResources(GameObject resource)
    {
        // load resource
        GameObject.Instantiate(resource);
        resource.transform.position = new Vector3(-3, -3, 0);
    }

    private void LoadScene()
    {
        Debug.Log("Scene loaded.");
    }

    private void LoadSceneProgress(object progress)
    {
        if ((float)progress < 0.9f)
            Debug.Log($"Loading scene progress: {(float)progress * 100}%");
        else
            Debug.Log($"Loading scene progress: 100%");
    }
}
