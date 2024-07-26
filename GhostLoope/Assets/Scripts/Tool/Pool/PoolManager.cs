using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum E_PoolType
{
    Bullet,
    Enemy
}

public class PoolManager : BaseSingleton<PoolManager>
{
    Dictionary<E_PoolType, Pool> poolManager;
    GameObject poolManagerObject;
    GameObject bulletPrefab;
    // GameObject enemyPrefab;

    // initialize pool manager
    public void Init(GameObject bulletPrefab)
    {
        base.Init();
        poolManager = new Dictionary<E_PoolType, Pool>();
        poolManagerObject = new GameObject("Pool Manager");

        // initialize prefab
        this.bulletPrefab = bulletPrefab;
    }

    public GameObject GetObj(E_PoolType poolType)
    {
        if (!poolManager.ContainsKey(poolType))
        {
            poolManager.Add(poolType, new Pool(poolType.ToString(), poolManagerObject, bulletPrefab));
        }
        return poolManager[poolType].GetObj();
    }

    public void ReturnObj(E_PoolType poolType, GameObject obj)
    {
        poolManager[poolType].ReturnObj(obj);
    }

    public void ClearPool()
    {
        poolManager.Clear();
        poolManagerObject = null;
    }
}
