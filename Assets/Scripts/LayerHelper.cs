using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerHelper : MonoBehaviour
{
    public static LayerHelper instance;


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
}
