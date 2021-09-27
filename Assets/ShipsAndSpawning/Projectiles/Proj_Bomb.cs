using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolableGameObjectLink))]
public class Proj_Bomb : Projectile, IOnPoolAndRetrieve
{

    public PoolableGameObjectLink PoolableGameObjectLink { get; set; }

    public float FuseDistance;

    public float FuseDistanceOffset;

    public Weapon SubProjectileWeapon;

    public int SubProjectiles = 10;

    public float SubProjectileSpreadArcDegrees = 180f;

    public bool MakeSubProjectilesNeutral = true;
    [SerializeField]
    protected float m_FuseDistance;
    private void Awake()
    {
        m_Velocity = Velocity;
        m_ExpirationSeconds = ExpirationSeconds;
        m_DistanceLifeSpan = DistanceLifeSpan;
        m_FuseDistance = FuseDistance;
    }

    private void Start()
    {
        PoolableGameObjectLink ??= GetComponent<PoolableGameObjectLink>();
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
        if (HitPrediction)
            PredictCollision();

        FuseDistance -= (transform.right * Velocity).magnitude * Time.fixedDeltaTime;
        transform.position += transform.right * Velocity * Time.fixedDeltaTime;

        if(FuseDistance - FuseDistanceOffset <= 0f)
        {
            OnDestruction();
        }

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
    public override void PostStart()
    {

        FuseDistance = Vector2.Distance(transform.position, TargetEntity.transform.position) - FuseDistanceOffset;

        if (SubProjectileWeapon == null)
        {
            Debug.LogError("SubWeapon Prefab should contain a Weapon script");
        }

        AllegianceInfo = (AllegianceInfo)gameObject.AddComponent(PEntity.AllegianceInfo.GetType());

    }

    bool didDestruct = false;

    public override void OnDestruction()
    {
        //jank fix for didDestruct screwing things up
        if (didDestruct) return;

        didDestruct = true;

        gameObject.SetActive(false);

        for(int i = 0; i < SubProjectiles; i++) 
        {

            Projectile projectile = Pooler.Instance.GetPooledGameObject(PooledObjectType.Shrapnel)?.GetComponent<Projectile>() ?? SubProjectileWeapon.InstantiateProjectileHere(SubProjectileWeapon.ToInstantiate);

            Quaternion dir = Quaternion.Euler(0f, 0f, Random.Range(-SubProjectileSpreadArcDegrees, SubProjectileSpreadArcDegrees) + transform.rotation.eulerAngles.z);

            if (MakeSubProjectilesNeutral)
            {
                projectile.transform.rotation = dir;
                projectile.AllegianceInfo = projectile.gameObject.AddComponent<Fac_Independent>();
                projectile.SpriteRenderer.color = projectile.AllegianceInfo.FactionColor;
                projectile.transform.position = transform.position;
                projectile.prevPosition = transform.position;
            } 
            else
            {
                projectile.AllegianceInfo = projectile.gameObject.AddComponent<Fac_Independent>();
                projectile.SpriteRenderer.color = projectile.AllegianceInfo.FactionColor;
                projectile.transform.position = transform.position;
                projectile.prevPosition = transform.position;
            }

            //projectile.gameObject.SetActive(true);

        }

        Pooler.Instance.PoolGameObject(PoolableGameObjectLink);

    }

    public IOnPoolAndRetrieve OnRetrieve()
    {
        didDestruct = false;
        Destroy(AllegianceInfo);
        return this;
    }

    public IOnPoolAndRetrieve OnPool()
    {
        DistanceLifeSpan = m_DistanceLifeSpan;
        ExpirationSeconds = m_ExpirationSeconds;
        FuseDistance = m_FuseDistance;

        Component[] allegianceInfos = GetComponents(typeof(AllegianceInfo));

        for (int i = 0; i < allegianceInfos.Length; i++)
            Destroy(allegianceInfos[i]);

        DeltaVelocity = new Vector2();

        gameObject.SetActive(false);

        return this;
    }
}
