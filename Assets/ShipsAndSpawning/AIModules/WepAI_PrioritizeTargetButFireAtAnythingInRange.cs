using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WepAI_PrioritizeTargetButFireAtAnythingInRange : AIWeaponController
{

    public Entity AlternativeTarget
    {
        get
        {
            if (!Targets.IsValidTarget(_alternativeTarget))
            {
                TargetingOverriden = false;
                return null;
            }
            return _alternativeTarget;
        }
        set
        {
            if (!TargetingOverriden && !PreventTargetOverride)
                _alternativeTarget = value;
        }
    }
    [SerializeField]
    private Entity _alternativeTarget;



    [Tooltip("Getting a target is expensive. This is how often it can check for alternative targets")]
    float SecondsBeforeCheckingAlternativeTarget = 1f;

    public void Update()
    {

        if(Target != null && Weapon.IsInRange(Target))
        {
            Weapon.TryFire(Target);
        } else
        {
            if (Targets.IsValidTarget(AlternativeTarget) && Weapon.IsInRange(AlternativeTarget))
            {
                Weapon.TryFire(AlternativeTarget);
            }
            else
            {
                TryCheckForAlternativeTarget();
                if (Targets.IsValidTarget(AlternativeTarget))
                {
                    Weapon.TryFire(AlternativeTarget);
                }
            }  
        }
    }

    public override void PostStart()
    {
        LastTimeChecked = Time.time;
    }

    float LastTimeChecked;
    public void TryCheckForAlternativeTarget()
    {
        if(Time.time - LastTimeChecked > SecondsBeforeCheckingAlternativeTarget)
        {
            LastTimeChecked = Time.time;
            AlternativeTarget = Targets.GetClosestTargetInRange(gameObject.transform.position, PEntity, Weapon.Range, ShipTypesToIgnore);
        }
    }
}
