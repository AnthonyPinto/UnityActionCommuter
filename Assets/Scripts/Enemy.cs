using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Poolable))]
public class Enemy : MonoBehaviour
{
    public int pointsOnDestroy = 50;
    public float WasHitAnimationDuration = 0.75f;
    public Animator animator;

    bool wasHit = false;
    bool isRoutineRunning = false;
    bool hitPlayer = false;
    GameObject playerObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isRoutineRunning)
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
            isRoutineRunning = true;
            StartCoroutine(WasHitRoutine());

        }
        else if (hitPlayer)
        {
            wasHit = false;
            hitPlayer = false;
            isRoutineRunning = false;
            animator.SetTrigger("Attack");
            Destroy(playerObject);

        }
    }

    IEnumerator WasHitRoutine()
    {

        GameManager.instance.AddPoints(pointsOnDestroy);
        animator.SetTrigger("Hit");
        yield return new WaitForSeconds(WasHitAnimationDuration);


        GetComponent<Poolable>().pool.Release(gameObject);
        isRoutineRunning = false;
    }
}
