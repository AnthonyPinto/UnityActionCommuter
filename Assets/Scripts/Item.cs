using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Poolable))]
public class Item : MonoBehaviour
{
    public int pointValue = 100;
    public AudioClip audioClip;
    AudioSource audioSource;
    SpriteRenderer sr;
    float audioClipLength = 0.5f;

    bool canStillBeMissed = true;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (transform.position.x >= 0)
        {
            canStillBeMissed = true;
        }

        if (canStillBeMissed && transform.position.x <= -10)
        {
            canStillBeMissed = false;
            GameManager.instance.OnPageMissed();
            GameManager.instance.ClearItemStreak();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.ScorePointsEvent(pointValue, transform.position);
            StartCoroutine(WasCollectedSequence());
        }
    }


    private IEnumerator WasCollectedSequence()
    {
        canStillBeMissed = false;
        sr.enabled = false;
        audioSource.PlayOneShot(audioClip);
        GameManager.instance.OnPageCollected();
        yield return new WaitForSeconds(audioClipLength);
        sr.enabled = true;
        GetComponent<Poolable>().pool.Release(gameObject);

    }
}
