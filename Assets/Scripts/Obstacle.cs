using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool ignoreNeighboringTrackSections = false; // narrow obstacles like pillars only hit objects on the same layer
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (
            collision.gameObject.CompareTag("Player") &&
            (!ignoreNeighboringTrackSections || collision.gameObject.layer == gameObject.layer)
           )
        {
            GameManager.instance.GameOver();
            collision.gameObject.GetComponent<PlayerController>().OnHit();
        }
    }
}
