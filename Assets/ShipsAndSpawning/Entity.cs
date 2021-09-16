using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Entity : MonoBehaviour
{
    // Start is called before the first frame update

    public bool IsDead = false;
    
    [SerializeField]
    private bool selected = false;

    public ShipType ShipType;

    public bool trackInRadar = true;
    public bool makeChildrenSameAllegiance = true;
    [Tooltip("Searches gameObject for component if not assigned")]
    public AIModule MovementAI;
    [Tooltip("Searches gameObject for component if not assigned")]
    public HitpointHandler HitpointHandler;
    public Vector2 DeltaVelocity;

    public bool Selectable = true;

    public bool OnlyTargetAllies = false;

    public bool PlayDestructionAnimationOnDeath = true;
    public GameObject DestructionPrefab;

    public Vector2 pushQueue;
    public AllegianceInfo AllegianceInfo
    {
        get
        {
            /* if (_AllegianceInfo == null)
            {
                Debug.LogError("Entity _allegianceInfo should never be null; it is assigned Fac_Independent by default in." +
                    " Check if it is ever assigned to by something (null assignments to this are also exceptions)", this);
                return _AllegianceInfo;
            } */
            return _AllegianceInfo;
        }
        set
        {
            _AllegianceInfo = value ?? throw new CriticalNullAssignmentException();
        }
    }
    [Tooltip("Critical; defaults to independent if not assigned")]
    [SerializeField]
    private AllegianceInfo _AllegianceInfo;

    public SpriteRenderer SpriteRenderer
    {
        get
        {
            if (_SpriteRenderer == null)
            {
                Debug.LogError("Entity _SpriteRenderer should never be null; it is assigned Fac_Independent by default in Awake()." +
                    " Check if it is ever assigned to by something (null assignments to this are also exceptions)", this);
                return _SpriteRenderer;
            }
            return _SpriteRenderer;
        }
        set
        {
            _SpriteRenderer = value ?? throw new CriticalNullAssignmentException();
        }
    }
    private SpriteRenderer _SpriteRenderer;

    public float SelectionCircleY = 1f;
    public float SelectionCircleX = 1f;

    public void Start()
    {

        if (DestructionPrefab == null)
        {
            DestructionPrefab = GameState.Instance.DefaultDestructionPrefab;
        }

        if (MovementAI == null)
        {
            MovementAI = (AIModule)gameObject.GetComponent(typeof(AIModule));
            if (MovementAI != null)
                Debug.LogWarning("It is reccomended to manually reference an AIModule in " +
                    "order to avoid conflict with other components that require AIModules.", this);
        }

        HitpointHandler ??= (HitpointHandler)gameObject.GetComponent(typeof(HitpointHandler));
        _SpriteRenderer ??= gameObject.GetComponent<SpriteRenderer>();

        _AllegianceInfo ??= (AllegianceInfo)gameObject.GetComponent(typeof(AllegianceInfo));

        if (makeChildrenSameAllegiance && transform.childCount != 0)
        {
            Entity[] childEntities = gameObject.GetComponentsInChildren<Entity>();

            for (int i = 0; i < childEntities.Length; i++)
            {
                //this check is necessary otherwise the parent (this gameobject) will get a duplicate faction script...
                if (childEntities[i] != this)
                    childEntities[i].AllegianceInfo = (AllegianceInfo)childEntities[i].gameObject.AddComponent(AllegianceInfo.GetType());
            }
        }

        if (_AllegianceInfo == null && trackInRadar)
        {
            Debug.LogWarning("Missing or invalid faction despite being set to add as target; setting as independent", this);
            _AllegianceInfo = gameObject.AddComponent<Fac_Independent>();
        }

        if (OnlyTargetAllies) AllegianceInfo.OnlyTargetAllies = true;

        if (trackInRadar)
            Targets.AddAsTarget(this);

        SpriteRenderer.color = AllegianceInfo.FactionColor;

        prevPosition = transform.position;

        PostStart();

    }

    public virtual void PostStart()
    {

    }

    [HideInInspector]
    Vector2 prevPosition;
    private void FixedUpdate()
    {
        DeltaVelocity = ((Vector2)gameObject.transform.position - prevPosition) / Time.fixedDeltaTime;
        prevPosition = gameObject.transform.position;
    }

    public void OnMouseOver()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (AllegianceInfo.Faction != GameState.Instance.PlayerAllegiance.Faction) return;

            if (TargetOverrider.Instance.selectedEntities.Count == 0)
                OnSelectAndTryAddToSelected();

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (!selected)
                {
                    OnSelectAndTryAddToSelected();
                }
                else
                {
                    OnDeselectAndRemoveFromSelected();
                }
            }
        }
    }

    public void OnMouseEnter()
    {
        if (!Selectable) return;

        TargetOverrider.Instance.CurrentMouseOverEntity = this;
        if (!selected)
            OnHighlight();
    }

    public void OnMouseExit()
    {
        if (!Selectable) return;

        TargetOverrider.Instance.CurrentMouseOverEntity = null;
        if(!selected)
            OnDehighlight();
    }

    public GameObject SelectionAnimation;

    public void OnSelectAndTryAddToSelected()
    {
        if (!Selectable) return;
        //if (TargetOverrider.Instance.selectedEntities.Contains(this)) return;

        if (selected) return;

        selected = true;
        OnHighlight();
        TargetOverrider.Instance.selectedEntities.Add(this);
    }

    public void OnDeselectAndRemoveFromSelected()
    {
        if (!Selectable) return;

        selected = false;
        OnDehighlight();
        TargetOverrider.Instance.selectedEntities.Remove(this);
    }

    public void OnHighlight()
    {
        if (!Selectable) return;
        //if (TargetOverrider.Instance.selectedEntities.Contains(this)) return;

        if (SelectionAnimation == null)
        {
            SelectionAnimation = Instantiate(GameState.Instance.SelectionGameObject, transform);
            SelectionAnimation.transform.localScale = new Vector3(SelectionCircleX, SelectionCircleY, SelectionAnimation.transform.localScale.z);
            SelectionAnimation.GetComponent<SelectionCircleAnimation>().SpriteRenderer.color = SpriteRenderer.color;
        }
    }

    public void OnDehighlight()
    {
        if (!Selectable) return;

        Destroy(SelectionAnimation);
    }

    /*
     * this boolean prevents the OnDestruction() function from being called an absurd amount of times in one frame 
     * (e.g. when many Shrapnel projectiles hit it at all at once) and causing immense lag due to many death animations
     * being instantiated at once
     */
    protected bool didDestruct = false;
    public virtual void OnDestruction()
    {

        if (didDestruct) return;

        didDestruct = true;

        if (PlayDestructionAnimationOnDeath)
            PlayDestructionAnimation();

        OnDeselectAndRemoveFromSelected();

        IsDead = true;

        LevelHandler.Instance.CheckForTeamEliminated(AllegianceInfo.Faction);

        Destroy(gameObject);
        Destroy(this);
    }

    public void OnDestroy()
    {
        IsDead = true;
    }


    public virtual void PlayDestructionAnimation()
    {
        GameObject destructionPrefab = Instantiate(DestructionPrefab);
        DestructionAnimation destructionAnimation = destructionPrefab.GetComponent<DestructionAnimation>();
        destructionAnimation.color = AllegianceInfo.FactionColor;
        destructionPrefab.transform.position = transform.position;

    }
}

public enum ShipType
{
    Fighter,
    MissileShip,
    LaserShip,
    ShieldCarrier,
    Carrier,
    Minion,
    Missile,
    Placeholder,
    Bomber,
    Bomb
}