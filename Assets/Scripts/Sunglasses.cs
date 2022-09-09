using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sunglasses : MonoBehaviour
{
    public int pointValue = 100;
    public AudioClip audioClip;
    AudioSource audioSource;
    public SpriteRenderer spriteRenderer;
    float audioClipLength = 0.5f;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player.PlayerController>().OnSunglasses();
            StartCoroutine(WasCollectedSequence());
        }
    }


    private IEnumerator WasCollectedSequence()
    {
        spriteRenderer.enabled = false;
        audioSource.PlayOneShot(audioClip);
        yield return new WaitForSeconds(audioClipLength);
        spriteRenderer.enabled = true;
        GetComponent<Poolable>().pool.Release(gameObject);
    }
}
