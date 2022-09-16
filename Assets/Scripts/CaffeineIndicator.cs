using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaffeineIndicator : MonoBehaviour
{
    Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = GameManager.instance.GetCaffeinePercentage();
    }
}
