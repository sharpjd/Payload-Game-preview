using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolableGameObjectLink))]
public class CarrierControlledEntity : Entity, IOnPoolAndRetrieve
{

    public Entity MothershipEntity;
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

        Debug.Log(Targets.RemoveEntityFromTargets(this));
        

        return this;
    }

    public IOnPoolAndRetrieve OnRetrieve()
    {
        IsDead = false;
        didOnDestruction = false;
        FrameBasedExecutor.Instance.ExecuteNextFrame(new System.Action(addAsTarget)); 
        DeltaVelocity = new Vector2();
        return this;

        void addAsTarget()
        {
            Targets.AddAsTarget(this);
            Debug.Log(Targets.GetAllTargetsOfFaction(AllegianceInfo.Faction).Contains(this));

        };
    }

    public void Update()
    {
        if (MothershipEntity == null || MothershipEntity.gameObject == null)
        {
            OnDestruction();
        }
    }

    public override void OnDestruction()
    {
        if (didOnDestruction) return;

        didOnDestruction = true;

        if (PlayDestructionAnimationOnDeath)
            PlayDestructionAnimation();

        OnDeselectAndRemoveFromSelected();

        IsDead = true;

        LevelHandler.Instance.CheckForTeamEliminated(AllegianceInfo.Faction);

        Pooler.Instance.PoolGameObject(PoolableGameObjectLink);
    }
}
