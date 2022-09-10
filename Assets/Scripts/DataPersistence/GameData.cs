using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    int numberOfScores = 10;
    List<(string, int)> highScores;

    // Constructer sets the defaults before there is any data to load
    public GameData()
    {
        highScores = new List<(string, int)>();
        for (int i = 0; i < numberOfScores; i++)
        {
            highScores.Add(("", 0));
        }
    }
}
