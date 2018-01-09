using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : MonoBehaviour {

    public static SceneMgr instance;

    public bool autoLoadScene = false;
    public string autoLoadSceneName;
    public float autoLoadTime = 3.0f;

    private void Awake()
    {
        if ( instance != null )
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        if (autoLoadScene)
        {
            StartCoroutine(LoadSceneDelayed(autoLoadTime));
        }
        DynamicGI.UpdateEnvironment();
    }


    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    IEnumerator LoadSceneDelayed(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        LoadScene(autoLoadSceneName);
    }
}
