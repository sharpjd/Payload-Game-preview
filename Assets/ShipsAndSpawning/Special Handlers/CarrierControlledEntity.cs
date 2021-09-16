using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolableGameObjectLink))]
public class CarrierControlledEntity : Entity, IOnPoolAndRetrieve
{

    public Entity MothershipEntity;
    public PoolableGameObjectLink PoolableGameObjectLink { get; set; }

    public IOnPoolAndRetrieve OnPool()
    {
        return this;
    }

    public IOnPoolAndRetrieve OnRetrieve()
    {
        DeltaVelocity = new Vector2();
        IsDead = true;
        OnDeselectAndRemoveFromSelected();
        OnDehighlight();
        HitpointHandler.Hitpoints = HitpointHandler.MaxHitpoints;
        Destroy(AllegianceInfo);

        return this;
    }

    public void Update()
    {
        if(MothershipEntity == null || MothershipEntity.gameObject == null)
        {
            OnDestruction();
        }
    }
}
