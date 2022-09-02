using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public SpriteRenderer tier1Sprite;
    public float tier1Speed = 15;

    public SpriteRenderer tier2Sprite;
    public float tier2Speed = 10;

    List<BackgroundTier> backgroundTiers = new List<BackgroundTier>();


    private void Start()
    {
        backgroundTiers.Add(new BackgroundTier(tier1Sprite, tier1Speed));
        backgroundTiers.Add(new BackgroundTier(tier2Sprite, tier2Speed));
    }

    private void Update()
    {
        foreach (BackgroundTier tier in backgroundTiers)
        {
            tier.UpdateTier(Time.deltaTime);
        }
    }

    class BackgroundTier
    {
        SpriteRenderer baseSprite;
        SpriteRenderer cloneSprite;
        float speed;
        Vector3 originalPos;
        float spriteWidth;

        public BackgroundTier(SpriteRenderer tierSprite, float tierSpeed)
        {
            baseSprite = tierSprite;
            speed = tierSpeed;
            spriteWidth = baseSprite.bounds.size.x;
            originalPos = baseSprite.transform.position;
            // Create a second copy of the sprite and offset it to the right by the width of the sprite we will maintain this offset
            cloneSprite = Instantiate(baseSprite, originalPos + Vector3.right * spriteWidth, baseSprite.transform.rotation);
        }

        public void UpdateTier(float deltaTime)
        {
            Vector3 delta = Vector3.left * speed * Time.deltaTime;
            Vector3 newPos = baseSprite.transform.position + delta;

            // If the sprite is about to move past it's full width to the left 'wrap' the sprite back to it's start position
            if (newPos.x < -spriteWidth)
            {
                newPos.x = newPos.x + spriteWidth;
            }

            // Move the bg sprite to it's new pos move the clone to the updated pos plus offset
            baseSprite.transform.position = newPos;
            cloneSprite.transform.position = newPos + Vector3.right * spriteWidth;
        }
    }
}
