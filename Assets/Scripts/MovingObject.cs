using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Poolable))]
public class MovingObject : MonoBehaviour
{
    public int speed = 15;

    private void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x < -60)
        {
            GetComponent<Poolable>().Release();
        }
    }
}
