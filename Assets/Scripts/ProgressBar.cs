using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {

    public Image timer;
    public float rate;

    private bool running = false;

    public void StartProgressBar()
    {
        running = true;
    }

    public void ResetProgressBar()
    {
        running = false;
        timer.fillAmount = 1f;
    }

    void Update()
    {
        if (running)
            timer.fillAmount -= (Time.deltaTime / rate);
    }
}
