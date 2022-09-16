using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    public TextMeshProUGUI distanceText;

    public TextMeshProUGUI gameOverText;

    private void Start()
    {
        gameOverText.gameObject.SetActive(false);
    }

    private void Update()
    {
        scoreText.text = GameState.Instance.TotalScore.ToString();
        distanceText.text = GameState.Instance.Distance.ToString() + " ft";
        gameOverText.gameObject.SetActive(GameManager.instance.GetIsGameOver());
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void UpdateDistance(int distance)
    {
        distanceText.text = distance.ToString() + " ft";
    }

    public void SetGameOver(bool isGameOver)
    {
        gameOverText.gameObject.SetActive(isGameOver);
    }
}
