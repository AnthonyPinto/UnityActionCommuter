using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsSceneManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadGame();
        }
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(SceneHelper.GameSceneIndex);
    }
}
