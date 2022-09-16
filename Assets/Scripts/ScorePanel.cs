using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScorePanel : MonoBehaviour
{
    public TextMeshProUGUI papersValueText;
    public TextMeshProUGUI streakValueText;
    public TextMeshProUGUI coffeeValueText;
    public TextMeshProUGUI totalValueText;


    private void Awake()
    {
        papersValueText.text = GameState.Instance.PapersScore.ToString();
        streakValueText.text = GameState.Instance.StreakScore.ToString();
        coffeeValueText.text = GameState.Instance.CoffeeScore.ToString();
        totalValueText.text = GameState.Instance.TotalScore.ToString();
    }
}
