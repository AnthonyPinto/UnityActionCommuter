using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StreakIndicator : MonoBehaviour
{
    TextMeshProUGUI text;
    //Animator animator;


    //int previousMultiplier = 0;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        //animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        int newMultiplier = GameManager.instance.GetPaperStreak();
        text.text = newMultiplier > 1 ? "Streak x" + newMultiplier.ToString() : "";

        // NOTE we can use this for animation if we stick with this indicator style

        //if (newMultiplier > previousMultiplier)
        //{
        //    animator.SetTrigger("Add");
        //}
        //else if (newMultiplier < previousMultiplier)
        //{
        //    animator.SetTrigger("Clear");
        //}
        //previousMultiplier = newMultiplier;
    }
}
