using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public UIController uiController;
    public AudioSource audioSource;

    int score = 0;
    int distance = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine(IncreaseScoreAndDistanceOverTimeRoutine());
    }



    public void AddPoints(int points)
    {
        score += points;
        uiController.UpdateScore(score);
    }

    public void AddDistance(int amountToAdd)
    {
        distance += amountToAdd;
        uiController.UpdateDistance(distance);
    }

    IEnumerator IncreaseScoreAndDistanceOverTimeRoutine()
    {
        while (true)
        {
            AddPoints(1);
            AddDistance(3);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void PlaySFX(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }


}
