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
        didDestruct = false;

        //Targets.RemoveEntityFromTargets(this);

        return this;
    }

    public IOnPoolAndRetrieve OnRetrieve()
    {
        //FrameBasedExecutor.Instance.ExecuteNextFrame(new System.Action(Start));
        return this;
    }

    public override void OnDestruction()
    {
        if (didDestruct) return;

        didDestruct = true;

        if (PlayDestructionAnimationOnDeath)
            PlayDestructionAnimation();

        OnDeselectAndRemoveFromSelected();

        IsDead = true;

        LevelHandler.Instance.CheckForTeamEliminated(AllegianceInfo.Faction);

        Pooler.Instance.PoolGameObject(PoolableGameObjectLink);
    }

}

