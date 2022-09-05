using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public AudioSource sfxAudioSource;
    public AudioClip jumpAudioClip;
    public AudioClip attackAudioClip;

    public Animator animator;

    public SpriteRenderer attackHitboxRenderer;

    AudioSource audioSource;

    float characterRailOffset = 0.75f; // How far 'above' the rail position the character should be

    int currentRailIndex = 0;
    int targetRailIndex = 0;

    ActionType? queuedAction;
    float actionQueuedTime;

    ActionType? currentAction;
    float actionStartTime;
    float currentActionDuration;

    float attackDuration = 0.1f;
    float jumpDuration = 0.5f;

    float attackCooldown = 0.2f;

    float earlyInputAllowance = 0.25f;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Snap to starting rail
        UpdateYPos(LayerHelper.instance.layerObjects[currentRailIndex].transform.position.y + characterRailOffset);
        SetPlayerLayerFromIndex(currentRailIndex);

        // Hide attack hitbox until we attack
        attackHitboxRenderer.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Get user input and queue it up to either start this frame - or possibly start on a later
        // frame if it becomes possible during the earlyInputAllowance window
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            queuedAction = ActionType.Up;
            actionQueuedTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
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
        if (!currentAction.HasValue && queuedAction.HasValue && Time.time - actionQueuedTime <= earlyInputAllowance)
        {
            switch (queuedAction)
            {
                case ActionType.Up:
                    StartJumpUp();
                    break;
                case ActionType.Down:
                    StartJumpDown();
                    break;
                case ActionType.Attack:
                    StartAttack();
                    break;
                default:
                    throw new System.Exception("unrecognized action type: " + queuedAction);
            };

            queuedAction = null;
        }

        UpdateForCurrentJump();
        UpdateForCurrentAttack();
        UpdateForCurrentCooldown();
    }

    private void StartJumpUp()
    {
        StartJump(true);
    }

    private void StartJumpDown()
    {
        StartJump(false);
    }

    private void StartJump(bool isUp)
    {
        // Check for the target rail and return if it doesn't exist
        // Skip over 'channel' layers between rails
        int newTargetRailIdx = isUp ? currentRailIndex + 2 : currentRailIndex - 2;
        if (newTargetRailIdx < 0 || newTargetRailIdx >= LayerHelper.instance.layerObjects.Count)
        {
            return;
        }

        targetRailIndex = newTargetRailIdx;
        currentAction = isUp ? ActionType.Up : ActionType.Down;
        actionStartTime = Time.time;
        currentActionDuration = jumpDuration;

        animator.SetTrigger("Jump");

        // pause grind SFX
        audioSource.Pause();

        // play Jump SFX
        sfxAudioSource.PlayOneShot(jumpAudioClip);

        // restart grind SFX
        audioSource.PlayDelayed(jumpDuration);
    }

    private void StartAttack()
    {
        animator.SetTrigger("Attack");
        
        sfxAudioSource.PlayOneShot(attackAudioClip);

        currentAction = ActionType.Attack;
        actionStartTime = Time.time;
        currentActionDuration = attackDuration;
    }

    private void UpdateForCurrentJump()
    {
        if (currentAction != ActionType.Up && currentAction != ActionType.Down)
        {
            return;
        }

        float remainingDuration = actionStartTime + currentActionDuration - Time.time;

        GameObject prevRail = LayerHelper.instance.layerObjects[currentRailIndex];
        GameObject nextRail = LayerHelper.instance.layerObjects[targetRailIndex];

        // If the jump is over, finish move and clear current action and update rail index
        if (remainingDuration <= 0)
        {
            currentAction = null;
            UpdateYPos(nextRail.transform.position.y + characterRailOffset);
            currentRailIndex = targetRailIndex;
            SetPlayerLayerFromIndex(currentRailIndex);
        }
        else
        {
            // reposition based on progress of jump
            float startY = prevRail.transform.position.y + characterRailOffset;
            float endY = nextRail.transform.position.y + characterRailOffset;

            float percentComplete = (currentActionDuration - remainingDuration) / currentActionDuration;

            // Set the current collision layer based on how far into the jump we are
            if (percentComplete < 0.25f)
            {
                SetPlayerLayerFromIndex(currentRailIndex);
            }
            else if (percentComplete < 0.75f)
            {
                if (currentAction == ActionType.Up)
                {
                    SetPlayerLayerFromIndex(currentRailIndex + 1);
                }
                else
                {
                    SetPlayerLayerFromIndex(currentRailIndex - 1);
                }
            }
            else
            {
                SetPlayerLayerFromIndex(targetRailIndex);
            }

            UpdateYPos(Mathf.Lerp(startY, endY, percentComplete));
        }
    }

    private void UpdateForCurrentAttack()
    {
        if (currentAction != ActionType.Attack)
        {
            return;
        }

        // If the attack is over clear the action
        float remainingDuration = actionStartTime + currentActionDuration - Time.time;
        float percentComplete = (currentActionDuration - remainingDuration) / currentActionDuration;
        if (remainingDuration <= 0)
        {
            attackHitboxRenderer.gameObject.SetActive(false);
            currentAction = ActionType.Cooldown;
            actionStartTime = Time.time;
            currentActionDuration = attackCooldown;
        }
        else
        {
            attackHitboxRenderer.gameObject.SetActive(true);
            Color prevColor = attackHitboxRenderer.color;
            attackHitboxRenderer.color = new Color(prevColor.r, prevColor.g, prevColor.b, 1 - percentComplete);
        }

        // DO ATTACK HERE
    }

    private void UpdateForCurrentCooldown()
    {
        if (currentAction != ActionType.Cooldown)
        {
            return;
        }

        float remainingDuration = actionStartTime + currentActionDuration - Time.time;
        if (remainingDuration <= 0)
        {
            currentAction = null;
        }
    }


    void UpdateYPos(float newY)
    {
        transform.position = new Vector3(transform.position.x, newY, transform.position.y);
    }

    void SetPlayerLayerFromIndex(int layerIndex)
    {
        int newLayer = LayerMask.NameToLayer(Constants.LayerString[Constants.LayerList[layerIndex]]);
        gameObject.layer = newLayer;
        attackHitboxRenderer.gameObject.layer = newLayer;
    }


    enum ActionType { Up, Down, Attack, Cooldown }
}
