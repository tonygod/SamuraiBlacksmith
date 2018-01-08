using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {

    public Image timer;
    public float rate;

    void Update()
    {
        timer.fillAmount -= (Time.deltaTime / rate);
    }
}
