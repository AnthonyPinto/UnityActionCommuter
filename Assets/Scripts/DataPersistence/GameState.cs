using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    int papersScore = 0;
    int streakScore = 0;
    int coffeeScore = 0;
    int distance = 0;

    public int PapersScore { get => papersScore; set => papersScore = value; }
    public int StreakScore { get => streakScore; set => streakScore = value; }
    public int CoffeeScore { get => coffeeScore; set => coffeeScore = value; }
    public int TotalScore { get => papersScore + streakScore + coffeeScore; }
    public int Distance { get => distance; set => distance = value; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }



    public void ClearScore()
    {
        PapersScore = 0;
        StreakScore = 0;
        CoffeeScore = 0;
    }

}
