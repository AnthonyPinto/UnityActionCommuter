using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageIndicator : MonoBehaviour
{
    public Sprite gatheredSprite;
    public Sprite missedSprite;
    public Image overlayImage;

    public void SetOverlay(bool wasGathered)
    {
        overlayImage.sprite = wasGathered ? gatheredSprite : missedSprite;
        overlayImage.color = Color.white;
    }
}
