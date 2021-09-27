using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolableGameObjectLink))]
public class Proj_Regular : Projectile, IOnPoolAndRetrieve
{

    public PoolableGameObjectLink PoolableGameObjectLink { get; set; }

    private void Start()
    {
        PoolableGameObjectLink = GetComponent<PoolableGameObjectLink>();
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
        for(int i = 0; i < hits.Length; i++)
        {
            OnHit(hits[i].collider.gameObject);
        }
    }

    public override void PostFixedUpdate()
    {
        if(HitPrediction)
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
        Destroy(AllegianceInfo);
        return this;
    }

    public IOnPoolAndRetrieve OnPool()
    {
        DistanceLifeSpan = m_DistanceLifeSpan;
        ExpirationSeconds = m_ExpirationSeconds;
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
