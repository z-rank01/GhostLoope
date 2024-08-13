using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum SceneName
{
    BeginGame,
    Level1,
    Level2,
    Level3,
}
public class SceneTravel : MonoBehaviour
{
    public SceneName sceneName;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("SceneTravel OnTriggerEnter:");
        Debug.Log("other.tag: " + other.tag);
        Debug.Log("other: " + other);
        if (other.tag == "Player")
        {
            Debug.Log("SceneName: " + sceneName);
            switch(sceneName)
            {
                case SceneName.Level1:
                    SceneManager.LoadScene("Level2");
                    //SaveManager.GetInstance().loading = true;
                    break;
                case SceneName.Level2:
                    SceneManager.LoadScene("Level3");
                    //SaveManager.GetInstance().loading = true;
                    break;
                case SceneName.Level3:

                    Player.GetInstance().SetIsGameEnd(true);





                    //SceneManager.LoadScene("BeginGame");
                    break;

            };
        }
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log("SceneName: " + sceneName);
    }
}
