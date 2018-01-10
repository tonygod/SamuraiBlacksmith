using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour {
    public GameObject quitButton;

    private void Awake()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            quitButton.SetActive(false);
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
