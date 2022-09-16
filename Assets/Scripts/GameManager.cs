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

    int totalPapers = 4; // temporary UI supports up to 20 keeping it low for testing;
    List<bool> encounteredPapersCollected = new List<bool>();
    int paperStreak = 0;


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

        if (!isGameOver && encounteredPapersCollected.Count >= totalPapers)
        {
            GameOver();
        }
    }

    private void Start()
    {
        StartCoroutine(IncreaseDistanceOverTimeRoutine());
    }

    public int GetTotalPapers()
    {
        return totalPapers;
    }

    public List<bool> GetEncounteredPapersCollected()
    {
        return encounteredPapersCollected;
    }

    public float GetGameSpeed()
    {
        return gameSpeed;
    }

    public bool GetIsGameOver()
    {
        return isGameOver;
    }

    public int GetPaperStreak()
    {
        return paperStreak;
    }

    public void ScorePointsEvent(int eventBaseValue, Vector3 positionOfEvent, bool isPaper)
    {
        int pointsToAdd = eventBaseValue;
        if (isPaper)
        {
            OnPaperCollected();
            pointsToAdd *= paperStreak;
        }

        AddPoints(pointsToAdd);
        pointsPopupsManager.RunPointsPopupAtPosition(pointsToAdd, positionOfEvent);
    }

    void OnPaperCollected()
    {
        encounteredPapersCollected.Add(true);
        paperStreak++;
    }

    public void OnPaperMissed()
    {
        encounteredPapersCollected.Add(false);
        OnStreakBroken();
    }

    public void OnStreakBroken()
    {
        paperStreak = 0;
    }


    void AddPoints(int pointsToAdd)
    {
        score += pointsToAdd;
        uiManager.UpdateScore(score);
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

    IEnumerator IncreaseDistanceOverTimeRoutine()
    {
        while (!isGameOver)
        {
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
        gameSpeed = 0;
    }


}
