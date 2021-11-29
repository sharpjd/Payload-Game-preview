using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{

    public Entity PEntity;
    public ShieldHitpointHandler ShieldHitpointHandler;

    public float MaxRadius = 4f;

    public float RechargeTimeSeconds = 20f;

    public bool Broken = false;
    public float BrokenShrinkRatePerSecond = 3f;
    public float RestartRegenerationPerSecond = 800f;
    public float RestartHPInjection = 1500f;
    [SerializeField]
    private float RemainingInjection = 0f;

    public float PercentageToBreak = 0.1f;

    public float InvulnerabilitySeconds = 5f;
    public float InvulnerabilityAnimationSpeed = 3.5f;

    private SpriteRenderer SpriteRenderer;

    private float AnimationDenominator
    {
        get
        {
            return 1 / InvulnerabilityAnimationSpeed;
        }
    }

    void Start()
    {

        SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        if (ShieldHitpointHandler == null)
        {
            Debug.LogError("Shield not assigned a ShieldHitpointHandler");
        }

        ShieldHitpointHandler.Shield = this;

        if (PEntity == null)
        {
            Debug.LogError("Shield not assigned a parent Entity");
        }

    }

    public void OnRegenerate()
    {
        ShieldHitpointHandler.Hitpoints = 0f;
        RegenerationTimeStart = Time.time;
        RemainingInjection = RestartHPInjection;
        Regenerating = true;
        Broken = false;
        ShieldHitpointHandler.Invulnerable = true;
    }

    
    public bool Regenerating;
    public float RegenerationTimeStart;

    private void Update()
    {
        if (!Broken)
        {
            float ShieldRadius = (ShieldHitpointHandler.Hitpoints / ShieldHitpointHandler.MaxHitpoints) * MaxRadius;

            Vector3 ShieldSizeVec = new Vector3(ShieldRadius, ShieldRadius, 1);

            transform.localScale = ShieldSizeVec;

            
            if (!Regenerating && ShieldHitpointHandler.Hitpoints / ShieldHitpointHandler.MaxHitpoints < PercentageToBreak)
            {
                OnBroken();
                Broken = true;
            }
        }
        

        if(Broken)
        {
            if(transform.localScale.x <= 0f || transform.localScale.y <= 0f)
            {
                transform.localScale = new Vector3(0f, 0f, 1f);
            }
            else
            {
                transform.localScale -= new Vector3(BrokenShrinkRatePerSecond * Time.deltaTime, BrokenShrinkRatePerSecond * Time.deltaTime, 0);
            }
        }

        if (Regenerating)
        {
            ShieldHitpointHandler.Hitpoints += RestartRegenerationPerSecond * Time.deltaTime;
            RemainingInjection -= RestartRegenerationPerSecond * Time.deltaTime;

            float a = 0.5f * Mathf.Cos((Time.time - RegenerationTimeStart) / AnimationDenominator) + 0.5f;

            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, a);

            if ((ShieldHitpointHandler.Hitpoints > ShieldHitpointHandler.MaxHitpoints || RemainingInjection <= 0) && Time.time - RegenerationTimeStart > InvulnerabilitySeconds)
            {
                OnRegenerationFinish();
                Regenerating = false;
            }
        }

    }

    void OnRegenerationFinish()
    {
        SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, 1f);
        ShieldHitpointHandler.Invulnerable = false;
    }

    public void OnBroken()
    {
        Invoke("OnRegenerate", RechargeTimeSeconds);
    }

    

}
