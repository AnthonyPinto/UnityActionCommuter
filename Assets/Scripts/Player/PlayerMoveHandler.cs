using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerMoveHandler : MonoBehaviour
    {
        public AudioSource sfxAudioSource;
        public AudioClip jumpAudioClip;

        public Animator animator;

        AudioSource audioSource;

        float jumpDuration = 0.5f;
        float characterRailOffset = 0.75f; // How far 'above' the rail position the character should be

        public PlayerController.PlayerState HandleStart(PlayerController.PlayerState state)
        {
            audioSource = GetComponent<AudioSource>();

            TrackManager.TrackSection currentRail = TrackManager.instance.GetTrackSectionByIndex(state.currentRailIndex);
            float currentRailYPosition = currentRail.yPosition;

            // Snap to starting rail
            UpdateYPosition(currentRailYPosition + characterRailOffset);

            SetPlayerLayerFromTrackSectionIndex(state.currentRailIndex);

            return state;
        }

        public PlayerController.PlayerState HandleTryJumpUp(PlayerController.PlayerState state)
        {
            return HandleTryJump(state, true);
        }

        public PlayerController.PlayerState HandleTryJumpDown(PlayerController.PlayerState state)
        {
            return HandleTryJump(state, false);
        }

        private PlayerController.PlayerState HandleTryJump(PlayerController.PlayerState state, bool isUp)
        {
            // Check for the target rail and return if it doesn't exist
            // Skip over 'channel' layers between rails
            int newTargetRailIdx = isUp ? state.currentRailIndex + 2 : state.currentRailIndex - 2;
            if (!TrackManager.instance.isIndexAValidTrackSection(newTargetRailIdx))
            {
                return state;
            }

            PlayerController.PlayerState resultState = new PlayerController.PlayerState(state);

            resultState.currentAction = isUp ? PlayerController.ActionType.Up : PlayerController.ActionType.Down;
            resultState.actionStartTime = Time.time;
            resultState.currentActionDuration = jumpDuration;
            resultState.targetRailIndex = newTargetRailIdx;
            animator.SetTrigger("Jump");

            // pause grind SFX
            audioSource.Pause();

            // play Jump SFX
            sfxAudioSource.PlayOneShot(jumpAudioClip);

            // restart grind SFX
            audioSource.PlayDelayed(resultState.currentActionDuration);

            return resultState;
        }

        public PlayerController.PlayerState HandleUpdate(PlayerController.PlayerState state)
        {
            if (
                state.currentAction != PlayerController.ActionType.Up &&
                state.currentAction != PlayerController.ActionType.Down)
            {
                return state; // if we are not jumping do nothing and return the state unchanged
            }

            PlayerController.PlayerState resultState = new PlayerController.PlayerState(state);


            float remainingDuration = state.actionStartTime + state.currentActionDuration - Time.time;

            TrackManager.TrackSection prevRail = TrackManager.instance.GetTrackSectionByIndex(state.currentRailIndex);
            TrackManager.TrackSection nextRail = TrackManager.instance.GetTrackSectionByIndex(state.targetRailIndex);

            // If the jump is over, finish move and update rail index
            if (remainingDuration <= 0)
            {
                UpdateYPosition(nextRail.yPosition + characterRailOffset);
                resultState.currentRailIndex = resultState.targetRailIndex;
                resultState.currentAction = null;
                SetPlayerLayerFromTrackSectionIndex(resultState.currentRailIndex);

            }
            else
            {
                // reposition based on progress of jump
                float startY = prevRail.yPosition + characterRailOffset;
                float endY = nextRail.yPosition + characterRailOffset;

                float percentComplete = (state.currentActionDuration - remainingDuration) / state.currentActionDuration;

                // Set the current collision layer based on how far into the jump we are
                if (percentComplete < 0.25f)
                {
                    SetPlayerLayerFromTrackSectionIndex(state.currentRailIndex);
                }
                else if (percentComplete < 0.75f)
                {
                    // based on weather we are moving up or down pass through the track section between current and target rails
                    if (state.targetRailIndex > state.currentRailIndex)
                    {
                        SetPlayerLayerFromTrackSectionIndex(state.currentRailIndex + 1);
                    }
                    else
                    {
                        SetPlayerLayerFromTrackSectionIndex(state.currentRailIndex - 1);
                    }
                }
                else
                {
                    SetPlayerLayerFromTrackSectionIndex(state.targetRailIndex);
                }

                UpdateYPosition(Mathf.Lerp(startY, endY, percentComplete));
            }

            return resultState;
        }


        void UpdateYPosition(float newY)
        {
            transform.position = new Vector3(transform.position.x, newY, transform.position.y);
        }

        void SetPlayerLayerFromTrackSectionIndex(int trackSectionIndex)
        {
            TrackManager.TrackSection trackSection = TrackManager.instance.GetTrackSectionByIndex(trackSectionIndex);
            TrackManager.instance.SetObjectLayerToMatchTrackSection(gameObject, trackSection);
        }
    }
}
