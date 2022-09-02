using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailHelper : MonoBehaviour
{
    public static RailHelper instance;


    public List<GameObject> rails;

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
