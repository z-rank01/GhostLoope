using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScenesManager : BaseSingletonMono<ScenesManager>
{
    // 同步加载
    public void LoadScene(string SceneName, UnityAction callback)
    {
        Debug.Log("Loading Scene");
        SceneManager.LoadScene(SceneName);
        if (callback != null) callback();
    }

    // 异步加载
    public void LoadSceneAsync(string SceneName, UnityAction callback)
    {
        //Debug.Log("In LoadSceneAsnc");
        //IEnumerator coroutine = LoadSceneAsycCoroutine(SceneName, fun);
        //MonoMgr.GetInstance().StartCoroutine(LoadSceneAsycCoroutine(SceneName, fun));

        Debug.Log("Loading Scene Async");
        GetInstance().StartCoroutine(LoadSceneAsyncCoroutine(SceneName, callback));
    }

    private IEnumerator LoadSceneAsyncCoroutine(string SceneName, UnityAction callback)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(SceneName);

        while (!ao.isDone)
        {
            // if needed to show progress bar, can use this event trigger
            EventCenter.GetInstance().EventTrigger(E_Event.LoadScene, ao.progress);
            yield return ao.progress;
        }
        yield return ao;

        if (callback != null) callback();
    }
}
