using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiplierText : MonoBehaviour
{
    TextMeshProUGUI text;
    Animator animator;


    int previousMultiplier = 0;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        int newMultiplier = GameManager.instance.GetScoreMultiplier();
        text.text = "x" + newMultiplier.ToString();
        if (newMultiplier > previousMultiplier)
        {
            animator.SetTrigger("Add");
        }
        else if (newMultiplier < previousMultiplier)
        {
            animator.SetTrigger("Clear");
        }
        previousMultiplier = newMultiplier;
    }
}
