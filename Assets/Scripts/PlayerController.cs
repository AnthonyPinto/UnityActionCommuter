using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public AudioClip jumpAudioClip;

    public Animator animator;

    AudioSource audioSource;

    float defaultAudioVolume;

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

    float earlyInputAllowance = 0.25f;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        defaultAudioVolume = audioSource.volume;

        // Snap to starting rail
        UpdateYPos(LayerHelper.instance.layerObjects[currentRailIndex].transform.position.y + characterRailOffset);
        SetLayerFromIndex(currentRailIndex);
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

        // play jump SFX
        audioSource.Pause();
        audioSource.volume = 0.8f;
        audioSource.PlayOneShot(jumpAudioClip);

        // reset audio source
        audioSource.volume = defaultAudioVolume;
        audioSource.PlayDelayed(jumpDuration);
    }

    private void StartAttack()
    {
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
            SetLayerFromIndex(currentRailIndex);
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
                SetLayerFromIndex(currentRailIndex);
            }
            else if (percentComplete < 0.75f)
            {
                if (currentAction == ActionType.Up)
                {
                    SetLayerFromIndex(currentRailIndex + 1);
                }
                else
                {
                    SetLayerFromIndex(currentRailIndex - 1);
                }
            }
            else
            {
                SetLayerFromIndex(targetRailIndex);
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
        if (remainingDuration <= 0)
        {
            currentAction = null;
        }

        // DO ATTACK HERE
    }


    void UpdateYPos(float newY)
    {
        transform.position = new Vector3(transform.position.x, newY, transform.position.y);
    }

    void SetLayerFromIndex(int layerIndex)
    {
        gameObject.layer = LayerMask.NameToLayer(Constants.LayerString[Constants.LayerList[layerIndex]]);
    }


    enum ActionType { Up, Down, Attack }
}
