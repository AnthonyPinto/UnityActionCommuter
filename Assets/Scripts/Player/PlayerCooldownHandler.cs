using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCooldownHandler : MonoBehaviour
{
    public PlayerController.PlayerState HandleUpdate(PlayerController.PlayerState state)
    {
        if (state.currentAction != PlayerController.ActionType.Cooldown)
        {
            return state;
        }

        PlayerController.PlayerState resultState = new PlayerController.PlayerState(state);

        float remainingDuration = resultState.actionStartTime + resultState.currentActionDuration - Time.time;
        if (remainingDuration <= 0)
        {
            resultState.currentAction = null;
        }

        return resultState;
    }
}
