using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wep_Missile : Weapon
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
                        Projectile projectile = Pooler.Instance.GetPooledGameObject(PooledObjectType.Missile)?.GetComponent<Projectile>() ?? InstantiateProjectileHere(ToInstantiate);
                        PointProjectileTowardsTargetAtAndSetParent(projectile.gameObject, PEntity, target, transform, this);
                        goto EndAndInvokeCooldown;
                    }
                    //this will only be executed if OnlyFireWithinRange is false
                    Projectile projectile_ = Pooler.Instance.GetPooledGameObject(PooledObjectType.Missile)?.GetComponent<Projectile>() ?? InstantiateProjectileHere(ToInstantiate);
                    PointProjectileTowardsTargetAtAndSetParent(projectile_.gameObject, PEntity, target, transform, this);

                    EndAndInvokeCooldown:
                    shotsLeft--;
                    onCoolDown = true;
                    Invoke("coolDown", FireRateSecs);
                }
            }
        }

    }
}
