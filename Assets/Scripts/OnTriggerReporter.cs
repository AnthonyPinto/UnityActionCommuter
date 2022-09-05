using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerReporter : MonoBehaviour
{
    public delegate void OnTrigger2DHandler(Collider2D collision);

    OnTrigger2DHandler OnTriggerEnter2DHandler;


    public void SetOnTriggerEnter2DHandler(OnTrigger2DHandler method)
    {
        OnTriggerEnter2DHandler = method;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("FOO");
        OnTriggerEnter2DHandler(collision);
    }
}
