using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Poolable))]
public class Item : MonoBehaviour
{
    public bool isPaper = false;
    public int pointValue = 100;
    public AudioClip audioClip;
    AudioSource audioSource;
    public SpriteRenderer optionalOverrideSpriteRenderer;
    SpriteRenderer sr;
    float audioClipLength = 0.5f;

    bool wasCollected = false;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        sr = optionalOverrideSpriteRenderer != null ? optionalOverrideSpriteRenderer : GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        wasCollected = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.ScorePointsEvent(pointValue, transform.position, isPaper);
            StartCoroutine(WasCollectedSequence());
        }
        else if (collision.gameObject.CompareTag("MissedItemZone") && !wasCollected && isPaper)
        {
            GameManager.instance.OnPaperMissed();
        }
    }


    private IEnumerator WasCollectedSequence()
    {
        wasCollected = true;
        sr.enabled = false;

        if (isPaper)
        {
            audioSource.pitch = 1.1f + GameManager.instance.paperStreak * 0.05f;
        }
        audioSource.PlayOneShot(audioClip);
        yield return new WaitForSeconds(audioClipLength);
        sr.enabled = true;
        GetComponent<Poolable>().pool.Release(gameObject);
    }
}
