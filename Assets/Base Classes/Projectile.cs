using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Projectile : MonoBehaviour
{
    public Entity PEntity;

    public bool HitPrediction = true;
    public float Velocity = 20f;
    public float Damage = 10f;
    public float DistanceLifespan = 30f;
    public bool DoExpire = true;
    public float ExpirationSeconds = 20f;
    public Entity TargetEntity;
    public AllegianceInfo AllegianceInfo;
    public SpriteRenderer SpriteRenderer;
    public bool InstantiatedFromInstantiater;

    public Vector2 DeltaVelocity;

    protected float m_Velocity;
    protected float m_DistanceLifeSpan;
    protected float m_ExpirationSeconds;

    private void Awake()
    {
        m_Velocity = Velocity;
        m_ExpirationSeconds = ExpirationSeconds;
        m_DistanceLifeSpan = DistanceLifespan;
    }

    // Start is called before the first frame update
    private void Start()
    {

        if (SpriteRenderer == null)
        {
            Debug.LogWarning("Assign a SpriteRenderer in the prefab menu for performance",this);
            SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (SpriteRenderer == null)
            {
                Debug.LogError("Projectile spriteRenderer is missing", this);
            }
        }


        if (!InstantiatedFromInstantiater)
        {
            Debug.LogWarning("It is preffered projectiles are instantiated from the ProjectileInstantiater class. " +
                "Set this InstantiatedFromInstantiater to true if this is intentional", this);
        }

        if (DoExpire)
            Invoke("OnExpire", ExpirationSeconds);

        /*
        if (AllegianceInfo == null)
        {
            Debug.LogError("Projectile has no AllegianceInfo", gameObject);
        }
        */

        PostStart();

    }

    public virtual void PostStart()
    {

    }

    public virtual void OnHit(GameObject gameObject)
    {

    }

    [SerializeField]
    public Vector2 prevPosition;
    public void FixedUpdate()
    {
        DeltaVelocity = ((Vector2)gameObject.transform.position - prevPosition) / Time.fixedDeltaTime;
        prevPosition = gameObject.transform.position;
        DistanceLifespan -= DeltaVelocity.magnitude * Time.fixedDeltaTime;
        if (DistanceLifespan <= 0f)
            OnExpire();

        PostFixedUpdate();
    }

    public void PointTowardsAndSetRotationXYToZero(Vector2 pos)
    {
        Vector2 yx = pos - (Vector2)transform.position;

        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(yx.y, yx.x) * Mathf.Rad2Deg, Vector3.forward);
    }

    public virtual void PostFixedUpdate()
    {

    }

    public virtual void OnExpire()
    {
        OnDestruction();
    }

    public virtual void OnDestruction()
    {
        Destroy(gameObject);
    }
}
