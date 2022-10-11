using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameState.Instance.UseCoin();
            LoadIntroCutScene();
        }
    }

    void LoadIntroCutScene()
    {
        SceneManager.LoadScene(SceneHelper.IntroCutSceneIndex);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(SceneHelper.GameSceneIndex);
    }
}
