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
        if (Instance != null && Instance != this)
        {
            Object.Destroy(gameObject);
        }

        Instance = this;

        director = GetComponent<PlayableDirector>();
    }

    public void StartTimeline()
    {
        director.Play();
    }

}
