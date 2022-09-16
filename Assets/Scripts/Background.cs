using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    // To add new sprites that also scroll:
    // - add spriterenderer here with naming tierXSprite
    // - add speed for the tier with naming tierXSpeed
    // - add sprite & speed to backgroundTiers in Start()

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
        // we use two copies of the sprite to make sure it always fills the screen
        SpriteRenderer baseSprite;
        SpriteRenderer clonedSprite;
        float speed;
        Vector3 originalPosition;
        float spriteWidth;

        public BackgroundTier(SpriteRenderer tierSprite, float tierSpeed)
        {
            baseSprite = tierSprite;
            speed = tierSpeed;
            spriteWidth = baseSprite.bounds.size.x;
            originalPosition = baseSprite.transform.position;
            // Create a second copy of the sprite and offset it to the right by
            // the width of the sprite. We will maintain this offset
            clonedSprite = Instantiate(
                baseSprite,
                originalPosition + Vector3.right * spriteWidth,
                baseSprite.transform.rotation
            );
        }

        public void UpdateTier(float deltaTime)
        {
            Vector3 distanceToMove = Vector3.left * (speed + GameManager.instance.GetGameSpeed()) * Time.deltaTime;
            Vector3 newPosition = baseSprite.transform.position + distanceToMove;

            // If the sprite is about to move past it's full width to the left
            // 'wrap' the sprite back to it's start position
            if (newPosition.x < -spriteWidth)
            {
                newPosition.x = newPosition.x + spriteWidth;
            }

            // Move the bg sprite to it's new position
            baseSprite.transform.position = newPosition;

            // Move the clone to the updated position plus offset
            clonedSprite.transform.position = newPosition + Vector3.right * spriteWidth;
        }
    }
}
