using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargAI_FocusDown : AITargetAcquisitor
{

    //will not find another target if not null1
    public Entity ApeThisTarget;

    public bool DoAutoAcquireTargets = true;
    public float AutoAcquireDelaySecs = 1f;

    private float LastUpdateTime = 0f;

    public override void PostStart()
    {
        ApeThisTarget = Targets.GetClosestTarget(PRadar.transform.position, PEntity, ShipTypesToIgnore);
        SetAllTargets(ApeThisTarget);
    }
    public void Update()
    {
        /*
        Debug.Log("autoaq: " + DoAutoAcquireTargets);
        Debug.Log("validtarget:" + !Targets.IsValidTarget(ApeThisTarget));
        Debug.Log("validtime:" + (Time.time - LastUpdateTime > AutoAcquireDelaySecs));
        */
        
        //will not find another target if not null
        if (DoAutoAcquireTargets && !Targets.IsValidTarget(ApeThisTarget) && Time.time - LastUpdateTime > AutoAcquireDelaySecs)
        {
            LastUpdateTime = Time.time;
            ApeThisTarget = Targets.GetClosestTarget(PEntity.transform.position, PEntity, ShipTypesToIgnore);
            SetAllTargets(ApeThisTarget);
        }
    }
}
