using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PaperCollectionIndicator : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        int countOfTotalPages = GameManager.instance.GetTotalPapers();
        int countOfPagesEncountered = GameManager.instance.GetEncounteredPapersCollected().Count;

        List<string> pageSymbols = new List<string>();

        for (int i = 0; i < countOfTotalPages; i++)
        {
            if (i < countOfPagesEncountered)
            {
                if (GameManager.instance.GetEncounteredPapersCollected()[i])
                {
                    pageSymbols.Add("<sprite=1>");
                }
                else
                {
                    pageSymbols.Add("X");
                }
            }
            else
            {
                pageSymbols.Add("-");
            }

        }

        text.text = string.Join(" ", pageSymbols);
    }
}
