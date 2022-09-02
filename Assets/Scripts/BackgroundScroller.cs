using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public SpriteRenderer tier1Sprite;
    public float tier1Speed = 15;
    Vector3 tier1OriginalPos;
    SpriteRenderer tier1SpriteClone;
    float tier1SpriteWidth;


    private void Start()
    {
        tier1SpriteWidth = tier1Sprite.bounds.size.x;
        tier1OriginalPos = tier1Sprite.transform.position;
        // Create a second copy of the sprite and offset it to the right by the width of the sprite we will maintain this offset
        tier1SpriteClone = Instantiate(tier1Sprite, tier1OriginalPos + Vector3.right * tier1SpriteWidth, tier1Sprite.transform.rotation);
    }

    private void Update()
    {
        Vector3 delta = Vector3.left * tier1Speed * Time.deltaTime;
        Vector3 newPos = tier1Sprite.transform.position + delta;

        // If the sprite is about to move past it's full width to the left 'wrap' the sprite back to it's start position
        if (newPos.x < -tier1SpriteWidth)
        {
            newPos.x = newPos.x + tier1SpriteWidth;
        }

        // Move the bg sprite to it's new pos move the clone to the updated pos plus offset
        tier1Sprite.transform.position = newPos;
        tier1SpriteClone.transform.position = newPos + Vector3.right * tier1SpriteWidth;
    }
}
