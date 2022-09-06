using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Poolable))]
public class Item : MonoBehaviour
{
    public int pointValue = 100;
    public AudioClip audioClip;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.PlaySFX(audioClip);
            GameManager.instance.AddPoints(pointValue);
            GetComponent<Poolable>().pool.Release(gameObject);
        }
    }
}
