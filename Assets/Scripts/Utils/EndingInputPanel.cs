using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingInputPanel : MonoBehaviour
{
    public GameObject continuePrompt;
    public NewHighScorePrompt newHighScorePrompt;

    private void Start()
    {
        // if there is a new high score
        if (GameState.Instance.CurrentHighScoreIndex.HasValue)
        {
            newHighScorePrompt.gameObject.SetActive(true);
            continuePrompt.SetActive(false);
        }
        else
        {
            newHighScorePrompt.gameObject.SetActive(false);
            continuePrompt.SetActive(true);
        }
    }

    private void Update()
    {

        if (continuePrompt.activeInHierarchy && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(SceneHelper.TitleSceneIndex);
        }
    }
}
