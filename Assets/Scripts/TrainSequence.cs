using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Poolable))]
public class TrainSequence : MonoBehaviour
{
    public GameObject trainLightPrefab;
    public GameObject trainPrefab;
    GameObject trainLight;
    GameObject train;

    float trainLightDuration = 1f;
    float trainDuration = 2f;

    float lightYOffset = 0;
    float trainYOffset = -0.1f;

    private void Start()
    {

    }


    private void OnEnable()
    {
        if (trainLight == null)
        {
            trainLight = Instantiate(trainLightPrefab);
            trainLight.SetActive(false);
        }
        if (train == null)
        {
            train = Instantiate(trainPrefab, transform.position, trainPrefab.transform.rotation);
            train.SetActive(false);
        }

        StartCoroutine(OnEnableSequence());
    }

    IEnumerator OnEnableSequence()
    {
        // wait for the OnEnable call to resolve so that we use thisgameObjects position AFTER it is repositioned by the object spawner
        yield return new WaitForSeconds(0);
        trainLight.SetActive(true);
        trainLight.SetLayerRecursively(gameObject.layer);
        trainLight.transform.position = new Vector3(trainLight.transform.position.x, transform.position.y, trainLight.transform.position.z) + Vector3.up * lightYOffset;


        yield return new WaitForSeconds(trainLightDuration);
        trainLight.SetActive(false);
        train.SetActive(true);
        train.SetLayerRecursively(gameObject.layer);
        train.transform.position = transform.position + Vector3.up * trainYOffset;

        yield return new WaitForSeconds(trainDuration);
        train.SetActive(false);
        GetComponent<Poolable>().Release();
    }
}
