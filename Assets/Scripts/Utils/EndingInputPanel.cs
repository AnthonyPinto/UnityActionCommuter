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

        if (continuePrompt.activeInHierarchy && Input.GetKeyDown(KeyCode.C))
        {
            SceneManager.LoadScene(SceneHelper.TitleSceneIndex);
        }
    }
}
