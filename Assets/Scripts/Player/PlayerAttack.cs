using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public AudioSource sfxAudioSource;
    public AudioClip attackAudioClip;


    public Animator animator;

    public GameObject attackHitBox;

    AudioSource audioSource;

    float lastAttackStartTime;

    float attackDuration = 0.3f;

    float attackCooldown = 0.2f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Hide attack hitbox until we attack
        attackHitBox.SetActive(false);
    }

    private void StartAttack()
    {
        animator.SetTrigger("Attack");

        sfxAudioSource.PlayOneShot(attackAudioClip);

        lastAttackStartTime = Time.time;
    }

    public bool GetIsAttacking()
    {
        return lastAttackStartTime + attackDuration > Time.time;
    }

    private void Update()
    {
        if (!GetIsAttacking())
        {
            return;
        }

        // If the attack is over clear the action
        float remainingDuration = lastAttackStartTime + attackDuration - Time.time;
        float percentComplete = (attackDuration - remainingDuration) / attackDuration;
        if (remainingDuration <= 0)
        {
            attackHitBox.SetActive(false);
            lastAttackStartTime = Time.time;
            attackDuration = attackCooldown;
        }
        else
        {
            attackHitBox.SetActive(true);
        }

    }
}
