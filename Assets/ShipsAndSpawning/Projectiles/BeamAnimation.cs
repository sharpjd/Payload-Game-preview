using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolableGameObjectLink))]
public class BeamAnimation : MonoBehaviour, IOnPoolAndRetrieve
{

    public PoolableGameObjectLink PoolableGameObjectLink { get; set; }
    public SpriteRenderer SpriteRenderer;
    public float TransparencyReductionPerSecond = 2.3f;

    

    public IOnPoolAndRetrieve OnPool()
    {
        SpriteRenderer.color = new Color(0, 0, 0, 1);
        gameObject.SetActive(false);
        return this;
    }

    public IOnPoolAndRetrieve OnRetrieve()
    {
        return this;
    }

    void Start()
    {
        if (SpriteRenderer == null)
        {
            Debug.LogError("Beam missing sprite renderer");
        }

        PoolableGameObjectLink = GetComponent<PoolableGameObjectLink>();

    }

    // Update is called once per frame
    void Update()
    {
        SpriteRenderer.color -= new Color(0, 0, 0, TransparencyReductionPerSecond * Time.deltaTime);
        if(SpriteRenderer.color.a <= 0f)
        {
            Pool();
        }
    }

    void Pool()
    {
        Pooler.Instance.PoolGameObject(PoolableGameObjectLink);
    }

}

