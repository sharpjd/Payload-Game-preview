using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEntity : Entity, IOnPoolAndRetrieve
{
    public Proj_Bomb Proj_Bomb;

    public PoolableGameObjectLink PoolableGameObjectLink { get; set; }

    public override void OnDestruction()
    {

        if (didOnDestruction) return;

        didOnDestruction = true;

        Proj_Bomb.OnDestruction();

    }

    public IOnPoolAndRetrieve OnPool()
    {
        DeltaVelocity = new Vector2();
        IsDead = true;
        OnDeselectAndRemoveFromSelected();
        OnDehighlight();
        HitpointHandler.Hitpoints = HitpointHandler.MaxHitpoints;
        Destroy(AllegianceInfo);
        didOnDestruction = false;

        //Targets.RemoveEntityFromTargets(this);

        return this;
    }

    public IOnPoolAndRetrieve OnRetrieve()
    {
        //FrameBasedExecutor.Instance.ExecuteNextFrame(new System.Action(Start));

        //TODO: Bomb Entities potentially need to register themselves as targets

        IsDead = false;
        return this;
    }
}
