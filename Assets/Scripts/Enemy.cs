using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Poolable))]
public class Enemy : MonoBehaviour
{
    AudioSource audioSource;

    public int pointsOnDestroy = 50;
    public float WasHitAnimationDuration = 0.75f;
    public Animator animator;
    public AudioClip deathAudioClip;
    public OnTriggerReporter attackPrepTrigger;

    bool wasHit = false;
    bool isHitRoutineRunning = false;
    bool hitPlayer = false;
    GameObject playerObject;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        attackPrepTrigger.SetOnTriggerEnter2DHandler(OnAttackPrepTriggerEnter2D);
    }

    private void OnAttackPrepTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("FOO");
            animator.SetTrigger("Attack");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isHitRoutineRunning)
        {
            return;
        }

        // We handle the results of collisions in LateUpdate because
        // OnTriggerEnter2D can be called multiple times in a single frame
        // resulting in both the player and the rat being hit
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            wasHit = true;
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            hitPlayer = true;
            playerObject = collision.gameObject;
        }
    }

    private void LateUpdate()
    {
        if (wasHit)
        {
            wasHit = false;
            hitPlayer = false;
            isHitRoutineRunning = true;
            StartCoroutine(WasHitRoutine());

        }
        else if (hitPlayer)
        {
            wasHit = false;
            hitPlayer = false;
            isHitRoutineRunning = false;
            Destroy(playerObject);

        }
    }

    IEnumerator WasHitRoutine()
    {

        GameManager.instance.AddPoints(pointsOnDestroy);
        animator.SetTrigger("Hit");
        audioSource.PlayOneShot(deathAudioClip);
        yield return new WaitForSeconds(WasHitAnimationDuration);


        GetComponent<Poolable>().pool.Release(gameObject);
        isHitRoutineRunning = false;
    }
}
