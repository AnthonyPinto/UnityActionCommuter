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


    float baseGameSpeed = 11;
    float caffeinePercentage = 1;

    int totalPapers = 4; // temporary UI supports up to 20 keeping it low for testing;
    List<bool> encounteredPapersCollected = new List<bool>();
    int paperStreak = 0;


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

    private void Start()
    {
        GameState.Instance.ClearScore();
        StartCoroutine(IncreaseDistanceOverTimeRoutine());
        StartCoroutine(DecreaseCaffeineOverTimeRoutine());
    }

    private void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.C))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (!isGameOver && encounteredPapersCollected.Count >= totalPapers)
        {
            SceneManager.LoadScene(SceneHelper.EndingSceneIndex);
        }
    }

    public float GetCaffeinePercentage()
    {
        return caffeinePercentage;
    }

    public void AddCaffeine()
    {
        caffeinePercentage = Mathf.Min(caffeinePercentage + 0.25f, 1);
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
        if (caffeinePercentage >= 0.5f)
        {
            return baseGameSpeed;
        }
        else
        {
            return baseGameSpeed * 2 * Mathf.Max(caffeinePercentage, 0.25f);
        }
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
        int totalPoints = eventBaseValue;
        if (isPaper)
        {
            encounteredPapersCollected.Add(true);
            paperStreak++;
            totalPoints *= paperStreak;
            GameState.Instance.PapersScore += eventBaseValue;
            GameState.Instance.StreakScore += totalPoints - eventBaseValue;

        }
        else // if it isn't paper it is coffee
        {
            GameState.Instance.CoffeeScore += totalPoints;
            AddCaffeine();
        }

        pointsPopupsManager.RunPointsPopupAtPosition(totalPoints, positionOfEvent);
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

    public void SetPlayerHasSunglasses(bool hasSunglasses)
    {
        playerHasSunglasses = hasSunglasses;
    }

    IEnumerator IncreaseDistanceOverTimeRoutine()
    {
        while (!isGameOver)
        {
            GameState.Instance.Distance += 3;
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
        baseGameSpeed = 0;
    }


}
