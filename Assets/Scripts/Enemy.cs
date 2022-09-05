using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Poolable))]
public class Enemy : MonoBehaviour
{
    public int pointsOnDestroy = 50;
    public Animator animator;
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("Attack");
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("PlayerAttack"))
        {            
            animator.SetTrigger("Attack");
            GameManager.instance.AddPoints(pointsOnDestroy);
            GetComponent<Poolable>().pool.Release(gameObject);
        }
    }
}
