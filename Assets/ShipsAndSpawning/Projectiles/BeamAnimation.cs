using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamAnimation : MonoBehaviour
{

    public PoolableGameObjectLink PoolableGameObjectLink { get; set; }
    public SpriteRenderer SpriteRenderer;
    public float TransparencyReductionPerSecond = 2.3f;

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
            Destroy(gameObject);
        }
    }

}

