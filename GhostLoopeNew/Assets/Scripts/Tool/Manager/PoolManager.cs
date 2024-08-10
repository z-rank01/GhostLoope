using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum E_PoolType
{
    SimpleBullet,

    FireBullet,
    ThunderBullet,
    ExplodeBullet,
    BurnBullet,
    IceBullet,
    PoisonBullet,
    SpiritPoisonBullet,

    BossPoisonBomb, 


    spawnEnemy,
}

public class PoolManager : BaseSingleton<PoolManager>
{
    Dictionary<E_PoolType, Pool> poolManager;
    GameObject poolManagerObject;

    // initialize pool manager
    public new void Init()
    {
        base.Init();
        poolManager = new Dictionary<E_PoolType, Pool>();
        poolManagerObject = new GameObject("Pool Manager");
    }

    public void AddPool(E_PoolType poolType, GameObject prefab)
    {
        poolManager.Add(poolType, new Pool(poolType.ToString(), poolManagerObject, prefab));
    }

    public GameObject GetObj(E_PoolType poolType)
    {
        return poolManager[poolType].GetObj();
    }

    public void ReturnObj(E_PoolType poolType, GameObject obj)
    {
        poolManager[poolType].ReturnObj(obj);
    }

    public void ClearPool()
    {
        poolManager.Clear();
    }
}
