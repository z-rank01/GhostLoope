using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 'T' will be the singleton class
public class BaseSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    private void Awake()
    {
        instance = this as T;
    }

    public static T GetInstance()
    {
        if (instance == null)
        {
            GameObject obj = new GameObject();
            instance = obj.AddComponent<T>();

            // in case of being cleared when scene changes
            GameObject.DontDestroyOnLoad(obj);

            Debug.Log("CreatingInstance");
        }
        return instance;
    }
}
