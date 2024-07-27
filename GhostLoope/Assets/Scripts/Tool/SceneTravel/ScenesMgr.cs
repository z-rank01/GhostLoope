using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScenesMgr : BaseSingleton<ScenesMgr>
{
    public void Start()
    {
        //Debug.Log("In SceneManager Start");
        //LoadScene("Scene/Scene Travel", null);

        //LoadSceneAsnc("Scene/Scene Travel", null);
    }
    // 同步加载
    public void LoadScene(string SceneName, UnityAction fun)
    {
        //Debug.Log("In LoadScene");
        SceneManager.LoadScene(SceneName);
        if (fun != null) fun();
    }

    // 异步下载
    public void LoadSceneAsnc(string SceneName, UnityAction fun)
    {
        //Debug.Log("In LoadSceneAsnc");
        //IEnumerator coroutine = LoadSceneAsycCoroutine(SceneName, fun);

        GetInstance().StartCoroutine(LoadSceneAsycCoroutine(SceneName, fun));
        //MonoMgr.GetInstance().StartCoroutine(LoadSceneAsycCoroutine(SceneName, fun));
    }

    private IEnumerator LoadSceneAsycCoroutine(string SceneName, UnityAction fun)
    {

        AsyncOperation ao = SceneManager.LoadSceneAsync(SceneName);

        while (!ao.isDone)
        {
            EventCenter.GetInstance().EventTrigger("Loading", ao.progress);
            yield return ao.progress;
        }


        yield return ao;
        if (fun != null) fun();
    }
}
