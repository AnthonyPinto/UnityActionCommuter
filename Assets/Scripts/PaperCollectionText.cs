using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PaperCollectionText : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        int countOfTotalPages = GameManager.instance.pagesToCollect;
        int countOfPagesEncountered = GameManager.instance.EncounteredPagesCollected.Count;

        List<string> pageSymbols = new List<string>();

        for (int i = 0; i < countOfTotalPages; i++)
        {
            if (i < countOfPagesEncountered)
            {
                if (GameManager.instance.EncounteredPagesCollected[i])
                {
                    pageSymbols.Add("X");
                }
                else
                {
                    pageSymbols.Add("O");
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
