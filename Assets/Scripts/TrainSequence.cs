using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Poolable))]
public class TrainSequence : MonoBehaviour
{
    public AudioClip trainPassingClip;
    public AudioClip trainHornClip;

    public GameObject trainLightPrefab;
    public GameObject trainPrefab;

    AudioSource trainAudioSource;
    GameObject train;

    float trainLightDuration = 1f;
    float trainDuration = 2f;

    float trainYOffset = -0.4f;

    private void Awake()
    {
        if (train == null)
        {
            train = Instantiate(trainPrefab, transform.position, trainPrefab.transform.rotation);
            train.SetActive(false);
        }
        trainAudioSource = train.GetComponent<AudioSource>();
    }


    private void OnEnable()
    {
        StartCoroutine(OnEnableSequence());
    }

    IEnumerator OnEnableSequence()
    {
        // wait for the OnEnable call to resolve so that we use thisgameObjects position AFTER it is repositioned by the object spawner
        yield return new WaitForSeconds(0);
        train.SetActive(true);
        trainAudioSource.PlayOneShot(trainHornClip);
        train.GetComponent<MovingObject>().enabled = false;
        train.SetLayerRecursively(gameObject.layer);
        train.transform.position = transform.position + Vector3.up * trainYOffset + Vector3.left * 10;

        yield return new WaitForSeconds(trainLightDuration);
        trainAudioSource.PlayOneShot(trainPassingClip);
        train.GetComponent<MovingObject>().enabled = true;

        yield return new WaitForSeconds(trainDuration);
        train.SetActive(false);
        GetComponent<Poolable>().Release();
    }
}
