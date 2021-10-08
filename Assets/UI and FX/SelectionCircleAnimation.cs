using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCircleAnimation : MonoBehaviour
{

    public SpriteRenderer SpriteRenderer;
    public float FrequencyCoefficient = 1f;

    float animationStartTime;

    void Start()
    {
        animationStartTime = Time.time;
        if(SpriteRenderer == null)
        {
            Debug.LogError("Missing sprite renderer");
        }
    }

    // Update is called once per frame
    void Update()
    {
        SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, 0.5f * Mathf.Sin(FrequencyCoefficient * (Time.time - animationStartTime + 1.5f)) + 0.5f);
    }
}
