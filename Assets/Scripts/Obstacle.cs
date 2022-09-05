using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Obstacles only effect the layer they are on
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.layer == gameObject.layer)
        {
            GameManager.instance.GameOver();
            Destroy(collision.gameObject);
        }
    }
}
