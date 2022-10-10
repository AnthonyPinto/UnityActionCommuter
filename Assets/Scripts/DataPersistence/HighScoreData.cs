using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HighScoreData
{

    public List<HighScoreEntry> highScores;
    public int coinsUsed;

    public HighScoreData(List<(string, int)> rawHighScores, int coinsUsed)
    {
        this.coinsUsed = coinsUsed;

        highScores = new List<HighScoreEntry>();

        foreach ((string, int) rawEntry in rawHighScores)
        {
            highScores.Add(new HighScoreEntry(rawEntry.Item1, rawEntry.Item2));
        }
    }

    public List<(string, int)> GetRawHighScores()
    {
        List<(string, int)> rawHighScores = new List<(string, int)>();

        foreach (HighScoreEntry entry in highScores)
        {
            rawHighScores.Add((entry.initials, entry.score));
        }

        return rawHighScores;
    }




    [System.Serializable]
    public class HighScoreEntry
    {
        public string initials;
        public int score;

        public HighScoreEntry(string initials, int score)
        {
            this.initials = initials;
            this.score = score;
        }
    }

}
