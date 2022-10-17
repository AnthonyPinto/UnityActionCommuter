using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsSceneManager : MonoBehaviour
{
    public GameObject fadeoutObject;
    bool isLoadingNextScene = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !isLoadingNextScene)
        {
            StartCoroutine(LoadGameSceneRoutine());
            isLoadingNextScene = true;
        }
    }

    public IEnumerator LoadGameSceneRoutine()
    {
        fadeoutObject.SetActive(true);
        yield return new WaitForSeconds(0.11f);
        SceneManager.LoadScene(SceneHelper.GameSceneIndex);
    }
}
