using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingInputPanel : MonoBehaviour
{
    public GameObject continuePrompt;
    public NewHighScorePrompt newHighScorePrompt;
    bool hasTimelineFinished = false;

    private void Update()
    {
        if (hasTimelineFinished)
        {
            // if there is a new high score, show high score screen and hide prompt
            if (GameState.Instance.CurrentHighScoreIndex.HasValue) {
                newHighScorePrompt.gameObject.SetActive(true);
                continuePrompt.SetActive(false);
            // if no new high score and animations in timeline are completed, show prompt to continue
            } else {
                continuePrompt.SetActive(true);
            }
        }

        if (continuePrompt.activeInHierarchy && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(SceneHelper.TitleSceneIndex);
        }
    }

    public void OnTimelineFinished()
    {
        hasTimelineFinished = true;
    }
}
