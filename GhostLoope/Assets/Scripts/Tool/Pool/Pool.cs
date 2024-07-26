using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// template constrain
public class Pool
{
    private GameObject parentObj;
    private List<GameObject> pool;
    private GameObject poolItemPrefab;

    // Initialization
    public Pool(string parentObjectName, GameObject poolManagerObject, GameObject prefab)
    {
        parentObj = new GameObject(parentObjectName);
        parentObj.transform.parent = poolManagerObject.transform;
        poolItemPrefab = prefab;
        pool = new List<GameObject>();
    }

    public GameObject GetObj()
    {
        GameObject obj = null;
        if (pool != null && pool.Count > 0)
        {
            obj = pool[0];
            pool.RemoveAt(0);
        }
        else
        {
            // If there is no target object, create one
            obj = GameObject.Instantiate(poolItemPrefab);
        }

        // update object status
        obj.SetActive(true);
        obj.transform.parent = parentObj.transform;
        return obj;
    }


    public void ReturnObj(GameObject obj)
    {
        // Add in object
        if (pool != null)
        {
            pool.Add(obj);
        }
        else
        {
            pool = new List<GameObject>() { obj };
        }

        // update status of object
        obj.SetActive(false);
        obj.transform.parent = null;
    }

    public int GetCount()
    {
        return pool.Count;
    }
}
