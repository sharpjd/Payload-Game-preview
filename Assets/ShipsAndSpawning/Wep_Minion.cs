using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wep_Minion : Weapon
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
                        Projectile projectile = Pooler.Instance.GetPooledGameObject(PooledObjectType.MinionProjectile)?.GetComponent<Projectile>() ?? InstantiateProjectileHere(ToInstantiate);
                        PointProjectileTowardsTargetAt(projectile.gameObject, transform, target, this);
                        goto EndAndInvokeCooldown;
                    }
                    //this will only be executed if OnlyFireWithinRange is false
                    Projectile projectile_ = Pooler.Instance.GetPooledGameObject(PooledObjectType.MinionProjectile)?.GetComponent<Projectile>() ?? InstantiateProjectileHere(ToInstantiate);
                    PointProjectileTowardsTargetAt(projectile_.gameObject, transform, target, this);

                    EndAndInvokeCooldown:
                    shotsLeft--;
                    onCoolDown = true;
                    Invoke("coolDown", FireRateSecs);
                }
            }
        }

    }

}
