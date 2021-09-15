using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAnimation : MonoBehaviour
{

    public SpriteRenderer SpriteRenderer;
    public float StartScale = 30f;
    public float ShrinkConstantPerSecond = 90f;
    public float Transparency = 0.5f;

    float animationStartTime;

    void Start()
    {
        animationStartTime = Time.time;
        if (SpriteRenderer == null)
        {
            Debug.LogError("Missing sprite renderer");
        }

        transform.localScale = new Vector3(StartScale, StartScale, 1);
        SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, Transparency);

    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale -= new Vector3(ShrinkConstantPerSecond, ShrinkConstantPerSecond, 0) * Time.deltaTime;

        if(transform.localScale.x < 0 || transform.localScale.y < 0)
        {
            Destroy(gameObject);
        }

    }
}
