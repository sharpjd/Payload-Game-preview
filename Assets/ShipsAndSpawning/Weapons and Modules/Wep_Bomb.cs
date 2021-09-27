using System;
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
                    try
                    {
                        Proj_Bomb projectile = (Proj_Bomb)(Pooler.Instance.GetPooledGameObject(PooledObjectType.Bomb)?.GetComponent<Projectile>() ?? InstantiateProjectileHere(ToInstantiate));

                        PointProjectileTowardsTargetAtAndSetParent(projectile.gameObject, PEntity, target, transform, this);

                        projectile.FuseDistance = Vector2.Distance(projectile.transform.position, projectile.TargetEntity.transform.position);

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
                    } catch (InvalidCastException e)
                    {
                        Debug.LogError("InvalidCastException when trying to cast Projectile to Proj_Bomb in Wep_Bomb; does the given prefab have a Proj_Bomb script?", gameObject);
                    }
                }
            }
        }
    }

}
