using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{

    public Entity PEntity;
    public ShieldHitpointHandler ShieldHitpointHandler;
    public float MaxRadius = 4f;

    public float RechargeTimeSeconds = 20f;

    public bool Broken = false;
    public float BrokenShrinkRatePerSecond = 3f;
    public float RestartRegenerationPerSecond = 800f;
    public float HPToBreak = 1000f; 


    void Start()
    {
        if (ShieldHitpointHandler == null)
        {
            Debug.LogError("Shield not assigned a ShieldHitpointHandler");
        }

        ShieldHitpointHandler.Shield = this;

        if (PEntity == null)
        {
            Debug.LogError("Shield not assigned a parent Entity");
        }

    }

    public bool Regenerating;

    private void Update()
    {
        if (!Broken)
        {
            float ShieldRadius = (ShieldHitpointHandler.Hitpoints / ShieldHitpointHandler.MaxHitpoints) * MaxRadius;

            Vector3 ShieldSizeVec = new Vector3(ShieldRadius, ShieldRadius, 1);

            transform.localScale = ShieldSizeVec;
        }

        if (ShieldHitpointHandler.Hitpoints < HPToBreak)
            OnBroken();

        if(Broken)
        {
            if(transform.localScale.x < 0f || transform.localScale.y < 0f)
            {
                transform.localScale = new Vector3(0f, 0f, 1f);
            }
            else
            {
                transform.localScale -= new Vector3(BrokenShrinkRatePerSecond * Time.deltaTime, BrokenShrinkRatePerSecond * Time.deltaTime, 0);
            }
        }

        if (Regenerating)
        {
            ShieldHitpointHandler.Hitpoints += RestartRegenerationPerSecond * Time.deltaTime;
        }

        if(ShieldHitpointHandler.Hitpoints > ShieldHitpointHandler.MaxHitpoints)
        {
            Regenerating = false;
            ShieldHitpointHandler.Hitpoints = ShieldHitpointHandler.MaxHitpoints;
        }

    }

    public void OnBroken()
    {
        Broken = true;
        Invoke("OnRegenerate", RechargeTimeSeconds);

    }

    public void OnRegenerate()
    {
        Broken = false;
        Regenerating = true;
    }

}
