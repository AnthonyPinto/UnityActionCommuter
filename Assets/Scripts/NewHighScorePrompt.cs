using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class NewHighScorePrompt : MonoBehaviour
{
    public InitialsInput initialsInput;
    public GameObject continuePrompt;
    bool hasTimelineFinished = false;

    // Update is called once per frame
    void Update()
    {
        if (initialsInput.IsFilledOut && hasTimelineFinished)
        {
            continuePrompt.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Return))
            {
                EventSystem.current.SetSelectedGameObject(null);
                GameState.Instance.AddHighScoreEntry(initialsInput.InputFieldText);
                SceneManager.LoadScene(SceneHelper.TitleSceneIndex);
            }
        }
        else
        {
            continuePrompt.SetActive(false);
        }
    }

    public void OnTimelineFinished()
    {
        hasTimelineFinished = true;
    }
}
