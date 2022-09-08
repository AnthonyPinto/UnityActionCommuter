using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool ignoreNeighboringTrackSections = false; // narrow obstacles like pillars only hit objects on the same layer
    public AudioClip hitAudioClip;

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (
            collision.gameObject.CompareTag("Player") &&
            (!ignoreNeighboringTrackSections || collision.gameObject.layer == gameObject.layer)
           )
        {
            audioSource.PlayOneShot(hitAudioClip);
            collision.gameObject.GetComponent<Player.PlayerController>().OnHit();
        }
    }
}
