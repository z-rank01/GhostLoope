using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ResourcesManager : BaseSingletonMono<ResourcesManager>
{
    // 同步加载
    public void LoadResource<T>(string path) where T : class
    {
        Debug.Log("Loading Resource");

        T resource = default(T);
        if (!string.IsNullOrEmpty(path))
        {
            resource = Resources.Load(path) as T;
            if (resource is GameObject)
                GameObject.Instantiate(resource as GameObject);
        }
        else Debug.Log("Fail to load asset");
    }

    // 异步加载
    public void LoadResourceAsync<T>(string path, UnityAction<T> callback) where T : class
    {
        Debug.Log("Loading Resource Asyc");

        if (!string.IsNullOrEmpty(path))
            GetInstance().StartCoroutine(LoadResourceAsyncCoroutine(path, callback));
        else Debug.Log("Fail to load asset");

    }

    private IEnumerator LoadResourceAsyncCoroutine<T>(string path, UnityAction<T> callback) where T : class
    {
        ResourceRequest rr = Resources.LoadAsync(path);
        yield return rr;

        if (callback != null)
            callback(rr.asset as T);
    }
}
