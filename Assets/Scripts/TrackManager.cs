using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    public static TrackManager instance;


    public List<GameObject> trackSectionGameObjects;

    List<TrackSection> trackSections = new List<TrackSection>();
    public enum TrackSectionKey { RailOne, ChannelOne, RailTwo, ChannelTwo, RailThree, ChannelThree, RailFour }

    static List<TrackSectionKey> TrackSectionKeyList = new List<TrackSectionKey>()
    {
        TrackSectionKey.RailOne,
        TrackSectionKey.ChannelOne,
        TrackSectionKey.RailTwo,
        TrackSectionKey.ChannelTwo,
        TrackSectionKey.RailThree,
        TrackSectionKey.ChannelThree,
        TrackSectionKey.RailFour,
    };

    static Dictionary<TrackSectionKey, string> TrackSectionLayerName = new Dictionary<TrackSectionKey, string>()
    {
        [TrackSectionKey.RailOne] = "RailOne",
        [TrackSectionKey.ChannelOne] = "ChannelOne",
        [TrackSectionKey.RailTwo] = "RailTwo",
        [TrackSectionKey.ChannelTwo] = "ChannelTwo",
        [TrackSectionKey.RailThree] = "RailThree",
        [TrackSectionKey.ChannelThree] = "ChannelThree",
        [TrackSectionKey.RailFour] = "RailFour",
    };

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        // Build the list of TrackSection objects used for game logic
        for (int i = 0; i < trackSectionGameObjects.Count; i++)
        {
            trackSections.Add(new TrackSection(TrackSectionKeyList[i], trackSectionGameObjects[i]));
        }
    }

    public void SetObjectLayerToMatchTrackSection(GameObject objectToSet, TrackSection trackSection)
    {
        objectToSet.SetLayerRecursively(LayerMask.NameToLayer(TrackManager.TrackSectionLayerName[trackSection.key]));
    }

    public bool isIndexAValidTrackSection(int index)
    {
        return index >= 0 && index < trackSections.Count;
    }


    public TrackSection GetTrackSectionByIndex(int index)
    {
        return trackSections[index];
    }

    public TrackSection GetTrackSectionByKey(TrackSectionKey key)
    {
        return trackSections.Find(t => { return t.key == key; });
    }


    public class TrackSection
    {
        public TrackSectionKey key;
        public float yPosition;

        public TrackSection(TrackSectionKey keyArg, GameObject trackSectionObject)
        {
            key = keyArg;
            yPosition = trackSectionObject.transform.position.y;
        }
    }
}