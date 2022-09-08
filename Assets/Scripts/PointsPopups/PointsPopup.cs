using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointsPopup : MonoBehaviour
{
    public TextMeshPro text;
    Poolable poolable;

    float animationTime = 1;

    private void Awake()
    {
        poolable = GetComponent<Poolable>();
    }

    private void OnEnable()
    {
        StartCoroutine(OnEnableRoutine());
    }

    IEnumerator OnEnableRoutine()
    {
        yield return new WaitForSeconds(animationTime);
        poolable.Release();
    }

    public void SetPoints(int points)
    {
        text.text = "+" + points.ToString();
    }
}
