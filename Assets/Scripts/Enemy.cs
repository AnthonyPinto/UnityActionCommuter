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
    public AudioClip attackAudioClip;
    public OnTriggerReporter attackPrepTrigger;

    bool isReadyToAttack = false;
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
            isReadyToAttack = true;
            audioSource.PlayOneShot(attackAudioClip);
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
            isReadyToAttack = false;
            isHitRoutineRunning = true;
            StartCoroutine(WasHitRoutine());

        }
        else if (hitPlayer && isReadyToAttack)
        {
            wasHit = false;
            hitPlayer = false;
            isHitRoutineRunning = false;
            GameManager.instance.GameOver();
            isReadyToAttack = false;
            playerObject.GetComponent<PlayerController>().OnHit();
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
