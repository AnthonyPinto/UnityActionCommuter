using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;

    public TextMeshProUGUI pausedText;

    private void Start()
    {
        gameOverText.gameObject.SetActive(false);
        pausedText.gameObject.SetActive(false);
    }

    private void Update()
    {
        gameOverText.gameObject.SetActive(GameManager.instance.DidLose);
    }

    public void SetGameOver(bool isGameOver)
    {
        gameOverText.gameObject.SetActive(isGameOver);
    }

    public void SetPaused(bool isPaused)
    {
        pausedText.gameObject.SetActive(isPaused);
    }
}
