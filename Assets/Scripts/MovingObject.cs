using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public int speed = 15;

    private void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
    }
}
