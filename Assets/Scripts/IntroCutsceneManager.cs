using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCutsceneManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadControlsScene();
        }
    }

    public void LoadControlsScene()
    {
        SceneManager.LoadScene(SceneHelper.ControlsSceneIndex);
    }

    public void OnEndIntroCutscene()
    {
        LoadControlsScene();
    }


}
