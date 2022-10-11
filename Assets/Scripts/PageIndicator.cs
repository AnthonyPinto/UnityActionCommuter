using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageIndicator : MonoBehaviour
{
    public Sprite gatheredSprite;
    public Sprite missedSprite;
    public Image overlayImage;

    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetOverlay(bool wasGathered)
    {
        image.color = Color.white;
        overlayImage.sprite = wasGathered ? gatheredSprite : missedSprite;
        overlayImage.color = Color.white;
    }
}
