using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public AudioClip gameOverAudioClip;
    public AudioSource backgroundMusic;
    public UIManager uiManager;
    public PointsPopupsManager pointsPopupsManager;


    float gameSpeed = 11;

    float caffeinePercentage = 1;

    int score = 0;
    int distance = 0;
    bool isGameOver = false;
    public bool playerHasSunglasses = false;

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

    private void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.C))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void Start()
    {
        StartCoroutine(IncreaseScoreAndDistanceOverTimeRoutine());
        StartCoroutine(DecreaseCaffeineOverTimeRoutine());
    }

    public float GetCaffeinePercentage()
    {
        return caffeinePercentage;
    }

    public float GetGameSpeed()
    {
        if (caffeinePercentage >= 0.5f)
        {
            return gameSpeed;
        }
        else
        {
            return gameSpeed * 2 * Mathf.Max(caffeinePercentage, 0.25f);
        }

    }

    public bool GetIsGameOver()
    {
        return isGameOver;
    }

    public void ScorePointsEvent(int pointsToAdd, Vector3 positionOfEvent)
    {
        AddPoints(pointsToAdd);
        pointsPopupsManager.RunPointsPopupAtPosition(pointsToAdd, positionOfEvent);
    }

    void AddPoints(int pointsToAdd)
    {
        score += pointsToAdd;
        uiManager.UpdateScore(score);
    }

    public void AddCaffeine()
    {
        caffeinePercentage = Mathf.Min(caffeinePercentage + 0.25f, 1);
    }

    public void AddDistance(int distanceToAdd)
    {
        distance += distanceToAdd;
        uiManager.UpdateDistance(distance);
    }

    public void SetPlayerHasSunglasses(bool hasSunglasses)
    {
        playerHasSunglasses = hasSunglasses;
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

    IEnumerator DecreaseCaffeineOverTimeRoutine()
    {
        while (!isGameOver)
        {
            caffeinePercentage -= 0.01f;
            caffeinePercentage = Mathf.Max(caffeinePercentage, 0);
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
        gameSpeed = 0;
    }


}
