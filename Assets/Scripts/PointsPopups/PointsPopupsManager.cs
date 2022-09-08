using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PointsPopupsManager : MonoBehaviour
{
    public GameObject pointsPopupPrefab;
    ObjectPool<GameObject> objectPool;

    private void Awake()
    {
        objectPool = ObjectPoolFactory.CreatePooler(pointsPopupPrefab);
    }

    public void RunPointsPopupAtPosition(int points, Vector3 position)
    {
        GameObject popup = objectPool.Get();
        popup.GetComponent<Poolable>().pool = objectPool;
        popup.transform.position = position;
        popup.GetComponent<PointsPopup>().SetPoints(points);
    }
}
