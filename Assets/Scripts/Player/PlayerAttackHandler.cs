using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerAttackHandler : MonoBehaviour
    {
        public AudioSource sfxAudioSource;
        public AudioClip attackAudioClip;


        public Animator animator;

        public GameObject attackHitBox;


        float attackDuration = 0.3f;

        float attackCooldown = 0.4f;

        public PlayerController.PlayerState HandleStart(PlayerController.PlayerState state)
        {
            // Hide attack hitbox until we attack
            attackHitBox.SetActive(false);
            return state;
        }

        public PlayerController.PlayerState HandleDoAttack(PlayerController.PlayerState state)
        {
            PlayerController.PlayerState resultState = new PlayerController.PlayerState(state);

            animator.SetTrigger("Attack");

            sfxAudioSource.PlayOneShot(attackAudioClip);

            resultState.actionStartTime = Time.time;
            resultState.currentAction = PlayerController.ActionType.Attack;
            resultState.currentActionDuration = attackDuration;

            return resultState;
        }

        public PlayerController.PlayerState HandleUpdate(PlayerController.PlayerState state)
        {
            if (state.currentAction != PlayerController.ActionType.Attack)
            {
                return state;
            }

            PlayerController.PlayerState resultState = new PlayerController.PlayerState(state);
            // If the attack is over clear the action
            float remainingDuration = resultState.actionStartTime + resultState.currentActionDuration - Time.time;
            float percentComplete = (resultState.currentActionDuration - remainingDuration) / resultState.currentActionDuration;
            if (remainingDuration <= 0)
            {
                attackHitBox.SetActive(false);
                resultState.currentAction = PlayerController.ActionType.Cooldown;
                resultState.actionStartTime = Time.time;
                resultState.currentActionDuration = attackCooldown;
            }
            else
            {
                attackHitBox.SetActive(true);
            }

            return resultState;
        }

        public PlayerController.PlayerState HandleOnHit(PlayerController.PlayerState state)
        {
            attackHitBox.SetActive(false);
            return state;
        }
    }
}
