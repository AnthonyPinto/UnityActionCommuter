using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewHighScorePrompt : MonoBehaviour
{
    public InitialsInput initialsInput;
    public GameObject continuePrompt;

    // Update is called once per frame
    void Update()
    {
        if (initialsInput.IsFilledOut)
        {
            continuePrompt.SetActive(true);
            if (Input.GetKeyDown(KeyCode.C))
            {
                GameState.Instance.AddHighScoreEntry(initialsInput.InputFieldText);
                SceneManager.LoadScene(SceneHelper.TitleSceneIndex);
            }
        }
        else
        {
            continuePrompt.SetActive(false);
        }
    }
}
