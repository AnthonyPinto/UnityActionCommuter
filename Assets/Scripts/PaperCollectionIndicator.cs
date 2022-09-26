using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PaperCollectionIndicator : MonoBehaviour
{
    public PageIndicator pagePrefab;

    List<PageIndicator> listOfPages;

    private void Start()
    {
        for (int i = 0; i < GameManager.instance.GetTotalPapers(); i++)
        {
            PageIndicator page = Instantiate(pagePrefab, transform.position, Quaternion.identity);
            page.transform.SetParent(transform);
            listOfPages.Add(page);
        }
    }

    private void Update()
    {
        int countOfTotalPages = GameManager.instance.GetTotalPapers();
        int countOfPagesEncountered = GameManager.instance.GetEncounteredPapersCollected().Count;

        for (int i = 0; i < countOfTotalPages; i++)
        {
            if (i < countOfPagesEncountered)
            {
                SpriteRenderer pageRenderer = listOfPages[i].GetComponent<SpriteRenderer>();
                if (!pageRenderer)
                {
                    continue;
                }

                if (GameManager.instance.GetEncounteredPapersCollected()[i])
                {
                    pageRenderer.sprite = listOfPages[i].gatheredSprite;
                }
                else
                {
                    pageRenderer.sprite = listOfPages[i].missedSprite;
                }
            }

        }
    }
}
