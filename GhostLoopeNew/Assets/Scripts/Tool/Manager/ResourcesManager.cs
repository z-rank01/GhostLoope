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
    public void LoadResource(string path, UnityAction<object> callback)
    {
        Debug.Log("Loading Resource");

        if (!string.IsNullOrEmpty(path))
        {
            object resource = Resources.Load(path);
            if (resource is GameObject)
                GameObject.Instantiate(resource as GameObject);
            else if (callback != null)
                callback(resource);
        }
        else Debug.Log("Fail to load asset");
    }

    // 异步加载
    public void LoadResourceAsync(string path, UnityAction<object> callback)
    {
        Debug.Log("Loading Resource Asyc");

        if (!string.IsNullOrEmpty(path))
            GetInstance().StartCoroutine(LoadResourceAsyncCoroutine(path, callback));
        else Debug.Log("Fail to load asset");

    }

    private IEnumerator LoadResourceAsyncCoroutine(string path, UnityAction<object> callback)
    {
        ResourceRequest rr = Resources.LoadAsync(path);
        yield return rr;

        if (callback != null)
            callback(rr.asset);
    }
}
