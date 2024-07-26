using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 'T' will be the singleton class
public class BaseSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public void Init()
    {
        instance = this as T;
    }

    public static T GetInstance()
    {
        if (instance == null)
        {
            UnityEngine.GameObject obj = new UnityEngine.GameObject();
            instance = obj.AddComponent<T>();

            // in case of being cleared when scene changes
            UnityEngine.GameObject.DontDestroyOnLoad(obj);

            Debug.Log("CreatingInstance");
        }
        return instance;
    }
}
