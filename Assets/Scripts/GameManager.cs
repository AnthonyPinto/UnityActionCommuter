using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public AudioClip gameOverAudioClip;
    public AudioClip playerDeathAudioClip;
    public AudioSource backgroundMusic;

    public UIManager uiManager;
    AudioSource audioSource;

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
        uiManager.UpdateScore(score);
    }

    public void AddDistance(int amountToAdd)
    {
        distance += amountToAdd;
        uiManager.UpdateDistance(distance);
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

    public void GameOver()
    {
        backgroundMusic.Stop();
        audioSource.PlayOneShot(playerDeathAudioClip);
        backgroundMusic.clip = gameOverAudioClip;
        backgroundMusic.loop = false;
        backgroundMusic.PlayDelayed(0.5f);
    }


}
