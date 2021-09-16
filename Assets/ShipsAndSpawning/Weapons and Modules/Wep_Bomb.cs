using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wep_Bomb : Weapon
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

                    Projectile projectile = Pooler.Instance.GetPooledGameObject(PooledObjectType.Bomb)?.GetComponent<Projectile>() ?? InstantiateProjectileHere(ToInstantiate);

                    PointProjectileTowardsTargetAt(projectile.gameObject, transform, target, this);

                    //wow this code is awful
                    if (OnlyFireWithinRange && Vector2.Distance(transform.position, target.transform.position) <= Range)
                    {
                        projectile.PEntity.AllegianceInfo = (AllegianceInfo)projectile.gameObject.AddComponent(PEntity.AllegianceInfo.GetType()); 
                        goto EndAndInvokeCooldown;
                    }
                    //this will only be executed if OnlyFireWithinRange is false
                    projectile.PEntity.AllegianceInfo = (AllegianceInfo)projectile.gameObject.AddComponent(PEntity.AllegianceInfo.GetType());

                    EndAndInvokeCooldown:
                    shotsLeft--;
                    onCoolDown = true;
                    Invoke("coolDown", FireRateSecs);
                }
            }
        }
    }

}
