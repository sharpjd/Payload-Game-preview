using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionAnimation : MonoBehaviour
{
    public Color color;
    public ParticleSystem AttachedParticleSystem;
    public float DestructionTime = 1f;

    private void Start()
    {
        var settings = AttachedParticleSystem.main;
        settings.startColor = color;
        Invoke("RemoveGameObject", DestructionTime);
    }

    private void RemoveGameObject()
    {
        Destroy(gameObject);
    }

}
