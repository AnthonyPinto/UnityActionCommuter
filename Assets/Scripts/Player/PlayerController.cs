using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {

        AudioSource audioSource;
        BoxCollider2D boxCollider;

        public SpriteRenderer spriteRenderer;
        public Animator playerAnimator;
        public AudioClip playerDeathAudioClip;

        public PlayerMoveHandler moveHandler;
        public PlayerAttackHandler attackHandler;
        public PlayerCooldownHandler cooldownHandler;

        ActionType? queuedAction;
        float actionQueuedTime;

        float onHitAnimationDuration = 0.5f;
        float earlyInputAllowance = 0.25f;
        int startingRailIndex = 0;

        float invincibilityDuration = 1;

        PlayerState playerState;

        public class PlayerState
        {
            public ActionType? currentAction;
            public float actionStartTime;
            public float currentActionDuration;

            public bool isInvincible = false;
            public float invincibilityStartTime;
            public float currentInvincibilityDuration;

            public int currentRailIndex = 0;
            public int targetRailIndex = 0;

            public bool hasSunglasses = false;

            public PlayerState(int initialRailIndex)
            {
                currentRailIndex = initialRailIndex;
                targetRailIndex = initialRailIndex;
            }

            public PlayerState(PlayerState stateToClone)
            {
                currentAction = stateToClone.currentAction;
                actionStartTime = stateToClone.actionStartTime;
                currentActionDuration = stateToClone.currentActionDuration;
                currentRailIndex = stateToClone.currentRailIndex;
                targetRailIndex = stateToClone.targetRailIndex;
                hasSunglasses = stateToClone.hasSunglasses;
                isInvincible = stateToClone.isInvincible;
                invincibilityStartTime = stateToClone.invincibilityStartTime;
                currentInvincibilityDuration = stateToClone.currentInvincibilityDuration;
            }
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            boxCollider = GetComponent<BoxCollider2D>();
            playerState = new PlayerState(startingRailIndex);
            playerState = moveHandler.HandleStart(playerState);
            playerState = attackHandler.HandleStart(playerState);
        }

        private void Update()
        {
            // Get user input and queue it up to either start this frame - or possibly start on a later
            // frame if it becomes possible during the earlyInputAllowance window
            if (
                Input.GetKeyDown(KeyCode.UpArrow) ||
                Input.GetKeyDown(KeyCode.W))
            {
                queuedAction = ActionType.Up;
                actionQueuedTime = Time.time;
            }
            else if (
                Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.S))
            {
                queuedAction = ActionType.Down;
                actionQueuedTime = Time.time;
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                queuedAction = ActionType.Attack;
                actionQueuedTime = Time.time;
            }

            // If we aren't in the middle of an action, and an action is queued and the input for that action was
            // within the input allowance, start the new action
            if (
                !playerState.currentAction.HasValue &&
                queuedAction.HasValue &&
                Time.time - actionQueuedTime <= earlyInputAllowance
            )
            {
                switch (queuedAction)
                {
                    case ActionType.Up:
                        playerState = moveHandler.HandleTryJumpUp(playerState);
                        break;
                    case ActionType.Down:
                        playerState = moveHandler.HandleTryJumpDown(playerState);
                        break;
                    case ActionType.Attack:
                        playerState = attackHandler.HandleDoAttack(playerState);
                        break;
                    default:
                        throw new System.Exception("unrecognized action type: " + queuedAction);
                };

                queuedAction = null;
            }

            playerState = moveHandler.HandleUpdate(playerState);
            playerState = attackHandler.HandleUpdate(playerState);
            playerState = cooldownHandler.HandleUpdate(playerState);
            playerState = HandleInvincibility(playerState);
        }

        public PlayerState HandleInvincibility(PlayerState state)
        {
            if (!state.isInvincible)
            {
                spriteRenderer.color = Color.white;
                return state;
            }

            float remainingDuration = state.invincibilityStartTime + state.currentInvincibilityDuration - Time.time;

            if (remainingDuration <= 0)
            {
                state.isInvincible = false;
            }
            else
            {
                int tenthsOfSeconds = Mathf.RoundToInt(remainingDuration * 10);
                if (tenthsOfSeconds % 3 == 0)
                {
                    spriteRenderer.color = Color.clear;
                }
                else
                {
                    spriteRenderer.color = Color.white;
                }
            }

            return state;
        }

        public void OnHit()
        {
            if (playerState.isInvincible)
            {

            }
            else if (playerState.hasSunglasses)
            {
                playerState = attackHandler.HandleOnHit(playerState);
                playerState.isInvincible = true;
                playerState.invincibilityStartTime = Time.time;
                playerState.currentInvincibilityDuration = invincibilityDuration;

                RemoveSunglasses();
                GameManager.instance.ClearItemStreak();
            }
            else
            {
                playerState.currentAction = ActionType.Death;
                playerState.currentActionDuration = float.PositiveInfinity;
                playerState.actionStartTime = Time.time;
                boxCollider.enabled = false;
                StartCoroutine(OnDeathRoutine());
                GameManager.instance.ClearItemStreak();
                GameManager.instance.GameOver();
            }
        }

        public void OnSunglasses()
        {
            AddSunglasses();
        }

        void AddSunglasses()
        {
            playerState.hasSunglasses = true;
            playerAnimator.SetFloat("HasSunglasses", 1);
            GameManager.instance.SetPlayerHasSunglasses(true);
        }

        void RemoveSunglasses()
        {
            playerState.hasSunglasses = false;
            playerAnimator.SetFloat("HasSunglasses", 0);
            GameManager.instance.SetPlayerHasSunglasses(false);
        }

        IEnumerator OnDeathRoutine()
        {
            playerAnimator.SetTrigger("Hit");
            audioSource.PlayOneShot(playerDeathAudioClip);
            yield return new WaitForSeconds(onHitAnimationDuration);
            Destroy(gameObject);
        }

        public enum ActionType { Up, Down, Attack, Cooldown, Death }
    }
}
