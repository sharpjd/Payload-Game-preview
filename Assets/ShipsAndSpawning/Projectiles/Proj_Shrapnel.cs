using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolableGameObjectLink))]
public class Proj_Shrapnel : Projectile, IOnPoolAndRetrieve
{

    public PoolableGameObjectLink PoolableGameObjectLink { get; set; }

    public float VelocityFloor = 3f;

    private void Awake()
    {
        PoolableGameObjectLink = GetComponent<PoolableGameObjectLink>();
        m_Velocity = Velocity;
        m_DistanceLifeSpan = DistanceLifeSpan;
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
        Velocity = m_Velocity * (DistanceLifeSpan / m_DistanceLifeSpan) + VelocityFloor;

        if (HitPrediction)
            PredictCollision();
        transform.position += transform.right * Velocity * Time.fixedDeltaTime;
    }

    public override void OnHit(GameObject gameObject)
    {
        Entity enemyEntityScript = (Entity)gameObject.GetComponent(typeof(Entity));

        if (enemyEntityScript == null) return;

        //Debug.Log(AllegianceInfo.CanHit(enemyEntityScript.AllegianceInfo.Faction, AllegianceInfo.ID));

        /* Debug.Log("enemy: " + enemyEntityScript.AllegianceInfo.Faction + ", " 
            + enemyEntityScript.AllegianceInfo.ID 
            + "; this: " + AllegianceInfo.Faction + ", " + AllegianceInfo.ID); */
        if (AllegianceInfo.CanHit(enemyEntityScript.AllegianceInfo.Faction, enemyEntityScript.AllegianceInfo.ID))
        {
            enemyEntityScript.HitpointHandler?.OnHit(Damage);
            OnDestruction();
        }
    }

    public IOnPoolAndRetrieve OnRetrieve()
    {
        gameObject.SetActive(true);
        return this;
    }

    public IOnPoolAndRetrieve OnPool()
    {
        DistanceLifeSpan = m_DistanceLifeSpan;
        ExpirationSeconds = m_ExpirationSeconds;
        Velocity = m_Velocity;
        m_Velocity = Velocity;
        m_DistanceLifeSpan = DistanceLifeSpan;
        Destroy(AllegianceInfo);
        DeltaVelocity = new Vector2();

        return this;
    }

    public override void OnDestruction()
    {
        Pooler.Instance.PoolGameObject(PoolableGameObjectLink);
    }

}
