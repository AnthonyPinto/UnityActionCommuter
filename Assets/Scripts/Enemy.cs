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
    public OnTriggerEmitter attackTrigger;

    // it gets this from collision
    GameObject playerObject;

    bool isAttacking = false;
    bool wasHitByPlayer = false;
    bool isHitRoutineRunning = false;
    bool hasHitPlayer = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        attackTrigger.SetOnTriggerEnter2DHandler(OnAttackTriggerEnter2D);
    }

    // triggered when player is close enough to be attacked by rat
    private void OnAttackTriggerEnter2D(Collider2D collision)
    {
        // ignore collision if they are on a higher rail
        if (collision.gameObject.CompareTag("Player") && !TrackManager.instance.IsFirstObjectOnATrackAboveTheSecond(collision.gameObject, gameObject))
        {
            isAttacking = true;
            audioSource.PlayOneShot(attackAudioClip);
            animator.SetTrigger("Attack");
        }
    }

    // triggered on collision with rat
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ignore if we are already handling being hit
        // or if the collision is on a higher rail
        if (isHitRoutineRunning || TrackManager.instance.IsFirstObjectOnATrackAboveTheSecond(collision.gameObject, gameObject))
        {
            return;
        }

        // We handle the results of collisions in LateUpdate because
        // OnTriggerEnter2D can be called multiple times in a single frame
        // resulting in both the player and the rat being hit
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            wasHitByPlayer = true;
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            hasHitPlayer = true;
            playerObject = collision.gameObject;
        }
    }

    private void LateUpdate()
    {
        if (wasHitByPlayer)
        {
            StartCoroutine(WasHitRoutine());
            isHitRoutineRunning = true;

            // reset
            wasHitByPlayer = false;
            hasHitPlayer = false;
            isAttacking = false;
        }
        else if (hasHitPlayer && isAttacking)
        {
            playerObject.GetComponent<Player.PlayerController>().OnHit();

            // reset
            isAttacking = false;
            isHitRoutineRunning = false;
            wasHitByPlayer = false;
            hasHitPlayer = false;
        }
    }

    IEnumerator WasHitRoutine()
    {
        animator.SetTrigger("Hit");
        audioSource.PlayOneShot(deathAudioClip);

        yield return new WaitForSeconds(WasHitAnimationDuration);

        GetComponent<Poolable>().pool.Release(gameObject);
        isHitRoutineRunning = false;
    }
}
