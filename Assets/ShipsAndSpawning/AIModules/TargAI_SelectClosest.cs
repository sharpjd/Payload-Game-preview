using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargAI_SelectClosest : AITargetAcquisitor
{

    bool DoAutoAcquireTargets = true;
    public float AutoAcquireDelaySecs = 3f;

    private float LastUpdateTime = 0f;

    bool checkedForTargetBeforeRefreshTime = false;
    public override void PostStart()
    {
        SetAllTargets(Targets.GetClosestTarget(PRadar.transform.position, PRadar.ParentEntity, ShipTypesToIgnore));
    }

    public void Update()
    {
        //Debug.Log((Target == null) + ", " + checkedForTargetBeforeRefreshTime);

        if(Target != null)
        {
            checkedForTargetBeforeRefreshTime = false;
        }

        if(Target == null && !checkedForTargetBeforeRefreshTime)
        {
            checkedForTargetBeforeRefreshTime = true;
            LastUpdateTime = Time.time;
            SetAllTargets(Targets.GetClosestTarget(PRadar.transform.position, PRadar.ParentEntity, ShipTypesToIgnore));
        }

        if (DoAutoAcquireTargets /*&& Target == null*/ && Time.time - LastUpdateTime > AutoAcquireDelaySecs)
        {
            checkedForTargetBeforeRefreshTime = false;
            LastUpdateTime = Time.time;
            SetAllTargets(Targets.GetClosestTarget(PRadar.transform.position, PRadar.ParentEntity, ShipTypesToIgnore));
        }
    }
}
