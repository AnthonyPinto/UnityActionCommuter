using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreEntry : MonoBehaviour
{
    public TextMeshProUGUI initialsText;
    public TextMeshProUGUI scoreText;

    public int entryIndex;

    // Start is called before the first frame update
    void Start()
    {
        (string, int) entryValues = GameState.Instance.HighScores[entryIndex];
        initialsText.text = entryValues.Item1;
        scoreText.text = entryValues.Item2.ToString().PadLeft(5, '0');
        

        if (entryIndex == GameState.Instance.CurrentHighScoreIndex)
        {
            initialsText.color = Color.cyan;
            scoreText.color = Color.cyan;
        }
    }
}
