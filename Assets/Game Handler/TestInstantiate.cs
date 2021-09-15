using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInstantiate : MonoBehaviour
{
    public GameObject testInstantiate;
    public AllegianceInfo facToGive;

    public int instantiationDelayMilliseconds = 50;
    public int testInstantiateCount = 100;
    public Vector2 origin;
    public Vector2 randomOffsetMax;

    public float lastInstantiatedTime = 0;
    public int instantiatedCount = 0;

    void Start()
    {
        if (testInstantiate == null)
        {
            Debug.LogWarning("This test instantiator has nothing assigned, disabling", this);
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (instantiatedCount <= testInstantiateCount && (Time.time - lastInstantiatedTime) > (instantiationDelayMilliseconds * 0.001f))
        {
            InstantiateObject();
            lastInstantiatedTime = Time.time;
            instantiatedCount++;
        }
    }

    GameObject InstantiateObject()
    {

        GameObject obj = Instantiate(testInstantiate);
        obj.SetActive(false);
        if(facToGive != null)
            obj.GetComponent<Entity>()?.gameObject.AddComponent(facToGive.GetType());
        Vector3 randomPos = new Vector3()
        {
            x = Random.Range(-randomOffsetMax.x, randomOffsetMax.x) + origin.x,
            y = Random.Range(-randomOffsetMax.y, randomOffsetMax.y) + origin.y,
            z = obj.transform.position.z
        };
        obj.transform.position = randomPos;
        obj.SetActive(true);
        return obj;
    }
}
