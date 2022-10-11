using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinsDisplay : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;

    void Update()
    {
        if (GameState.Instance.CoinsUsed == 0)
        {
            textMeshPro.text = "";
        } else {
            textMeshPro.text = GameState.Instance.CoinsUsed.ToString() + " Coins Used";
        }        
    }
}
