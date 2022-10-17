using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;
    public AudioSource sfxAudioSource;
    public AudioClip coinInsertAudioClip;

    int papersScore = 0;
    int streakScore = 0;
    int coffeeScore = 0;
    int sunglassesScore = 0;
    int distance = 0;

    int coinsUsed;
    int? currentHighScoreIndex;

    List<(string, int)> highScores;

    const int HighScoreListLength = 5;

    const int initialsCharacterLimit = 3;

    public int PapersScore { get => papersScore; set => papersScore = value; }
    public int StreakScore { get => streakScore; set => streakScore = value; }
    public int CoffeeScore { get => coffeeScore; set => coffeeScore = value; }
    public int SunglassesScore { get => sunglassesScore; set => sunglassesScore = value; }
    public int TotalScore { get => papersScore + streakScore + coffeeScore + sunglassesScore; }
    public int Distance { get => distance; set => distance = value; }
    public List<(string, int)> HighScores { get => highScores.GetRange(0, HighScoreListLength); set => highScores = value.GetRange(0, HighScoreListLength); }

    public int? CurrentHighScoreIndex { get => currentHighScoreIndex; set => currentHighScoreIndex = value; }
    public int CoinsUsed { get => coinsUsed; set => coinsUsed = value; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        int loadedCoinsUsed;
        highScores = FileDataHandler.Load(out loadedCoinsUsed);
        CoinsUsed = loadedCoinsUsed;

        if (highScores == null)
        {
            highScores = new List<(string, int)>();
        }

        // if we have fewer than the expected number of entries
        // pad with empty entries
        for (int i = highScores.Count; i < HighScoreListLength; i++)
        {
            highScores.Add(("___", 0));
        }
    }

    public void UseCoin()
    {
        sfxAudioSource.PlayOneShot(coinInsertAudioClip);
        CoinsUsed++;
        FileDataHandler.Save(HighScores, CoinsUsed);
        Debug.Log("TOTAL COINS USED: " + CoinsUsed);
    }

    public void OnWin()
    {
        CurrentHighScoreIndex = GetCurrentHighScoreIndex();
    }

    public void AddHighScoreEntry(string initials)
    {
        if (currentHighScoreIndex.HasValue)
        {
            List<(string, int)> updatedHighScores = new List<(string, int)>(HighScores);
            updatedHighScores.Insert(currentHighScoreIndex.Value, (initials.Substring(0, initialsCharacterLimit), TotalScore));
            HighScores = updatedHighScores;
            FileDataHandler.Save(updatedHighScores, coinsUsed);
        }
    }



    int? GetCurrentHighScoreIndex()
    {
        for (int i = 0; i < HighScoreListLength; i++)
        {
            if (HighScores[i].Item2 < TotalScore)
            {
                return i;
            }
        }

        return null;
    }



    public void ClearScore()
    {
        PapersScore = 0;
        StreakScore = 0;
        CoffeeScore = 0;
    }
}
