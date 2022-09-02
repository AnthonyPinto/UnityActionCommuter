using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
        // Snap to starting rail
        UpdateYPos(RailHelper.instance.rails[targetRailIndex].transform.position.y + characterRailOffset);
        currentRailIndex = targetRailIndex;
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
        int newTargetRailIdx = isUp ? currentRailIndex + 1 : currentRailIndex - 1;
        if (newTargetRailIdx < 0 || newTargetRailIdx >= RailHelper.instance.rails.Count)
        {
            return;
        }

        targetRailIndex = newTargetRailIdx;
        currentAction = isUp ? ActionType.Up : ActionType.Down;
        actionStartTime = Time.time;
        currentActionDuration = jumpDuration;
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

        GameObject prevRail = RailHelper.instance.rails[currentRailIndex];
        GameObject nextRail = RailHelper.instance.rails[targetRailIndex];

        // If the jump is over, finish move and clear current action and update rail index
        if (remainingDuration <= 0)
        {
            currentAction = null;
            UpdateYPos(nextRail.transform.position.y + characterRailOffset);
            currentRailIndex = targetRailIndex;
        }
        else
        {
            // reposition based on progress of jump
            float startY = prevRail.transform.position.y + characterRailOffset;
            float endY = nextRail.transform.position.y + characterRailOffset;

            UpdateYPos(Mathf.Lerp(startY, endY, (currentActionDuration - remainingDuration) / currentActionDuration));
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


    enum ActionType { Up, Down, Attack }
}
