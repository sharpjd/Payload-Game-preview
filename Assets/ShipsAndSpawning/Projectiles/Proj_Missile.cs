using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolableGameObjectLink))]
public class Proj_Missile : Projectile, IOnPoolAndRetrieve
{

    public PoolableGameObjectLink PoolableGameObjectLink { get; set; }

    public float CurrentVelocity = 1f;
    public float AccelerationPerSecond = 20f;
    public float TurnRate = 120f;
    public float AutoAcquireDelaySecs = 2f;

    float m_InitialVelocity;

    protected override void Start()
    {
        PoolableGameObjectLink = GetComponent<PoolableGameObjectLink>();
        base.Start();
    }
    protected override void Awake()
    {

        m_InitialVelocity = CurrentVelocity;
        base.Awake();

    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        OnHit(collision.gameObject);
    }

    private void PredictCollision()
    {
        int layermask = 1 << 2;
        layermask = ~layermask;

        RaycastHit2D[] hits = Physics2D.RaycastAll(gameObject.transform.position, DeltaVelocity.normalized, DeltaVelocity.magnitude * Time.fixedDeltaTime, layermask);
        //Debug.Log(hits.Length);
        for (int i = 0; i < hits.Length; i++)
        {
            OnHit(hits[i].collider.gameObject);
        }
    }

    public override void PostFixedUpdate()
    {
        if (CurrentVelocity <= Velocity)
            CurrentVelocity += AccelerationPerSecond * Time.fixedDeltaTime;

        if (HitPrediction)
            PredictCollision();
        transform.position += transform.right * Velocity * Time.fixedDeltaTime;

        if (Targets.IsValidTarget(TargetEntity, PEntity.AllegianceInfo))
        {
            TurnToVec(TargetEntity.transform.position);
        }
    }

    public void TurnToVec(Vector2 vec)
    {

        Vector2 direction = vec - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float posAngle = Vector2.Angle(direction, transform.right);

        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, TurnRate * Time.deltaTime / posAngle);
    }


    public override void OnHit(GameObject gameObject)
    {
        Entity enemyEntityScript = (Entity)gameObject.GetComponent(typeof(Entity));
        if (enemyEntityScript == null) return;

        if (AllegianceInfo.CanHit(enemyEntityScript.AllegianceInfo.Faction, enemyEntityScript.AllegianceInfo.ID))
        {
            enemyEntityScript.HitpointHandler?.OnHit(Damage);
            OnDestruction();
        }
    }

    

    public override void  PostStart()
    {

        if(PEntity == null)
        {
            Debug.LogError("Missile needs an Entity component");
        }
        //PEntity.AllegianceInfo = (AllegianceInfo)gameObject.AddComponent(AllegianceInfo.GetType());
    }

    bool DoAutoAcquireTargets = true;

    private float LastUpdateTime = 0f;

    bool checkedForTargetBeforeRefreshTime = false;
    public void Update()
    {

        if (!DoAutoAcquireTargets)
            return;

        if (TargetEntity != null)
        {
            checkedForTargetBeforeRefreshTime = false;
        }

        if (TargetEntity == null && !checkedForTargetBeforeRefreshTime)
        {
            checkedForTargetBeforeRefreshTime = true;
            LastUpdateTime = Time.time;
            TargetEntity = Targets.GetClosestTarget(transform.position, PEntity, new List<ShipType>());
        }

        if (TargetEntity == null && Time.time - LastUpdateTime > AutoAcquireDelaySecs)
        {
            checkedForTargetBeforeRefreshTime = false;
            LastUpdateTime = Time.time;
            TargetEntity = Targets.GetClosestTarget(transform.position, PEntity, new List<ShipType>());
        }
    }

    public IOnPoolAndRetrieve OnRetrieve()
    {
        Destroy(AllegianceInfo);
        return this;
    }

    public IOnPoolAndRetrieve OnPool()
    {
        DistanceLifeSpan = m_DistanceLifeSpan;
        ExpirationSeconds = m_ExpirationSeconds;
        CurrentVelocity = m_InitialVelocity;
        Destroy(AllegianceInfo);
        DeltaVelocity = new Vector2();

        gameObject.SetActive(false);
        return this;
    }

    public override void OnDestruction()
    {
        Pooler.Instance.PoolGameObject(PoolableGameObjectLink);
    }
}
