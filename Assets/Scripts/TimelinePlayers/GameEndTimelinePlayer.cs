using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameEndTimelinePlayer : MonoBehaviour
{
    public static GameEndTimelinePlayer Instance;

    private PlayableDirector director;
    public GameObject controlPanel;

    void Awake()
    {
        director = GetComponent<PlayableDirector>();
        director.played += PlayDirector;
    }

    private void PlayDirector(PlayableDirector _obj) {
        controlPanel.SetActive(true);
    }

    public void StartTimeline()
    {
        director.Play();
    }

}
