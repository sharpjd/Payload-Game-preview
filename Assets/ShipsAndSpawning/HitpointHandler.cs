using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitpointHandler : MonoBehaviour
{
    public float MaxHitpoints = 100f;
    public float Hitpoints = 100f;

    public bool DoRegeneration = false;
    public float RegenPerSecond = 3f;

    public Entity PEntity;

    public void Start()
    {

        PEntity ??= (Entity)GetComponent(typeof(Entity));

        if(PEntity == null)
        {
            Debug.LogError("HitpointHandler was not assigned and did not find a parent Entity component");
        }

    }

    public virtual void Update()
    {
        if (DoRegeneration)
        {
            if (Hitpoints <= MaxHitpoints)
                Hitpoints += RegenPerSecond * Time.deltaTime;
            if (Hitpoints > MaxHitpoints)
                Hitpoints = MaxHitpoints;
        }
    }


    public virtual void OnHit(float damage)
    {
        Hitpoints -= damage;
        CheckHP();
    }

    public virtual void CheckHP()
    {
        if (Hitpoints <= 0f)
            OnHitpointZero();
    }

    public virtual void OnHitpointZero()
    {
        PEntity.OnDestruction();
    }

}
