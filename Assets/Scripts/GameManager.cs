using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public AudioClip gameOverAudioClip;
    public AudioSource backgroundMusic;
    public UIManager uiManager;

    int score = 0;
    int distance = 0;
    bool isGameOver = false;

    private void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        StartCoroutine(IncreaseScoreAndDistanceOverTimeRoutine());
    }

    public void AddPoints(int pointsToAdd)
    {
        score += pointsToAdd;
        uiManager.UpdateScore(score);
    }

    public void AddDistance(int distanceToAdd)
    {
        distance += distanceToAdd;
        uiManager.UpdateDistance(distance);
    }

    IEnumerator IncreaseScoreAndDistanceOverTimeRoutine()
    {
        while (!isGameOver)
        {
            AddPoints(1);
            AddDistance(3);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        backgroundMusic.Stop();
        backgroundMusic.clip = gameOverAudioClip;
        backgroundMusic.loop = false;
        backgroundMusic.PlayDelayed(0.5f);
        uiManager.SetGameOver(true);
    }


}
