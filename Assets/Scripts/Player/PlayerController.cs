using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {

        AudioSource audioSource;
        BoxCollider2D boxCollider;

        public Animator animator;
        public AudioClip playerDeathAudioClip;

        public PlayerMoveHandler moveHandler;
        public PlayerAttackHandler attackHandler;
        public PlayerCooldownHandler cooldownHandler;

        ActionType? queuedAction;
        float actionQueuedTime;

        float onHitAnimationDuration = 0.6f;
        float earlyInputAllowance = 0.25f;
        int startingRailIndex = 0;

        PlayerState playerState;

        public class PlayerState
        {
            public ActionType? currentAction;
            public float actionStartTime;
            public float currentActionDuration;

            public int currentRailIndex = 0;
            public int targetRailIndex = 0;

            public bool isDead = false;

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
                isDead = stateToClone.isDead;
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
        }

        public void OnHit()
        {
            if (!playerState.isDead)
            {
                playerState.isDead = true;
                playerState = attackHandler.HandleOnHit(playerState);
                boxCollider.enabled = false;
                StartCoroutine(OnHitRoutine());
            }
        }

        IEnumerator OnHitRoutine()
        {
            animator.SetTrigger("Hit");
            audioSource.PlayOneShot(playerDeathAudioClip);
            yield return new WaitForSeconds(onHitAnimationDuration);
            Destroy(gameObject);
        }

        public enum ActionType { Up, Down, Attack, Cooldown }
    }
}
