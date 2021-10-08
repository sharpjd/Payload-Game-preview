using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHitpointHandler : HitpointHandler
{

    public Shield Shield;
    public override void Update()
    {
        if (!Shield.Broken)
        {
            if (DoRegeneration)
            {
                if (Hitpoints <= MaxHitpoints)
                    Hitpoints += RegenPerSecond * Time.deltaTime;
                if (Hitpoints > MaxHitpoints)
                    Hitpoints = MaxHitpoints;
            }
        }
    }

    public override void OnHitpointZero()
    {
        Shield.OnBroken();
    }
}
