using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    public static TrackManager instance;


    public List<GameObject> layerObjects;

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
    }

    public GameObject GetObjectForLayer(Constants.Layer layer)
    {
        return layer switch
        {
            Constants.Layer.RailOne => layerObjects[0],
            Constants.Layer.ChannelOne => layerObjects[1],
            Constants.Layer.RailTwo => layerObjects[2],
            Constants.Layer.ChannelTwo => layerObjects[3],
            Constants.Layer.RailThree => layerObjects[4],
            Constants.Layer.ChannelThree => layerObjects[5],
            Constants.Layer.RailFour => layerObjects[6],
            _ => throw new System.Exception("Unrecognized Layer: " + layer)
        };
    }
}
