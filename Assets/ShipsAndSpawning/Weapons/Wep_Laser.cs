using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wep_Laser : Weapon
{
    public override void TryFire(Entity target)
    {
        if (shotsLeft <= 0 && !reloading)
        {
            reloading = true;
            Invoke("reload", ReloadTimeSecs);
        }

        if (!reloading)
        {
            if (!(shotsLeft <= 0))
            {
                if (!onCoolDown)
                {
                    //wow this code is awful
                    if (OnlyFireWithinRange && Vector2.Distance(transform.position, target.transform.position) <= Range)
                    {
                        projectileInstantiator.InstantiateProjectileAtParentWithTarget(ToInstantiate, PEntity, target);
                        goto EndAndInvokeCooldown;
                    }
                    //this will only be executed if OnlyFireWithinRange is false
                    projectileInstantiator.InstantiateProjectileAtParentWithTarget(ToInstantiate, PEntity, target);

                    EndAndInvokeCooldown:
                    shotsLeft--;
                    onCoolDown = true;
                    Invoke("coolDown", FireRateSecs);
                }
            }
        }
    }

}
