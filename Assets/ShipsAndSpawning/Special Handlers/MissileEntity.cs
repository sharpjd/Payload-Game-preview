using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolableGameObjectLink))]
public class MissileEntity : Entity, IOnPoolAndRetrieve
{
    public PoolableGameObjectLink PoolableGameObjectLink { get; set; }

    public override void PostStart()
    {
        PoolableGameObjectLink = GetComponent<PoolableGameObjectLink>();
    }

    public IOnPoolAndRetrieve OnPool()
    {

        DeltaVelocity = new Vector2();
        IsDead = true;
        OnDeselectAndRemoveFromSelected();
        OnDehighlight();
        HitpointHandler.Hitpoints = HitpointHandler.MaxHitpoints;
        Destroy(AllegianceInfo);

        return this;
    }

    public IOnPoolAndRetrieve OnRetrieve()
    {
        return this;
    }
}

