using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public AudioSource sfxAudioSource;
    public AudioClip jumpAudioClip;

    public Animator animator;

    AudioSource audioSource;

    float characterRailOffset = 0.75f; // How far 'above' the rail position the character should be

    int currentRailIndex = 0;
    int targetRailIndex = 0;

    float lastJumpStartTime;
    bool isLastJumpUp;
    float jumpDuration = 0.5f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        TrackManager.TrackSection currentRail = TrackManager.instance.GetTrackSectionByIndex(currentRailIndex);
        float currentRailYPosition = currentRail.yPosition;

        // Snap to starting rail
        UpdateYPosition(currentRailYPosition + characterRailOffset);

        SetPlayerLayerFromTrackSectionIndex(currentRailIndex);
    }

    public bool GetIsJumping()
    {
        return lastJumpStartTime + jumpDuration > Time.time;
    }

    public void TryJumpUp()
    {
        TryStartJump(true);
    }

    public void TryJumpDown()
    {
        TryStartJump(false);
    }

    private void TryStartJump(bool isUp)
    {
        // Check for the target rail and return if it doesn't exist
        // Skip over 'channel' layers between rails
        int newTargetRailIdx = isUp ? currentRailIndex + 2 : currentRailIndex - 2;
        if (!TrackManager.instance.isIndexAValidTrackSection(newTargetRailIdx))
        {
            return;
        }

        targetRailIndex = newTargetRailIdx;
        isLastJumpUp = isUp;
        lastJumpStartTime = Time.time;
        animator.SetTrigger("Jump");

        // pause grind SFX
        audioSource.Pause();

        // play Jump SFX
        sfxAudioSource.PlayOneShot(jumpAudioClip);

        // restart grind SFX
        audioSource.PlayDelayed(jumpDuration);
    }

    private void Update()
    {
        if (!GetIsJumping())
        {
            return;
        }

        float remainingDuration = lastJumpStartTime + jumpDuration - Time.time;

        TrackManager.TrackSection prevRail = TrackManager.instance.GetTrackSectionByIndex(currentRailIndex);
        TrackManager.TrackSection nextRail = TrackManager.instance.GetTrackSectionByIndex(targetRailIndex);

        // If the jump is over, finish move and update rail index
        if (remainingDuration <= 0)
        {
            UpdateYPosition(nextRail.yPosition + characterRailOffset);
            currentRailIndex = targetRailIndex;
            SetPlayerLayerFromTrackSectionIndex(currentRailIndex);
        }
        else
        {
            // reposition based on progress of jump
            float startY = prevRail.yPosition + characterRailOffset;
            float endY = nextRail.yPosition + characterRailOffset;

            float percentComplete = (jumpDuration - remainingDuration) / jumpDuration;

            // Set the current collision layer based on how far into the jump we are
            if (percentComplete < 0.25f)
            {
                SetPlayerLayerFromTrackSectionIndex(currentRailIndex);
            }
            else if (percentComplete < 0.75f)
            {
                if (isLastJumpUp)
                {
                    SetPlayerLayerFromTrackSectionIndex(currentRailIndex + 1);
                }
                else
                {
                    SetPlayerLayerFromTrackSectionIndex(currentRailIndex - 1);
                }
            }
            else
            {
                SetPlayerLayerFromTrackSectionIndex(targetRailIndex);
            }

            UpdateYPosition(Mathf.Lerp(startY, endY, percentComplete));
        }
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
