using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Radar))]
public class AIModule : MonoBehaviour
{
    public bool TargetingOverriden = false;
    public bool PreventTargetOverride = false;

    public List<ShipType> ShipTypesToIgnore = new List<ShipType>(); 

    public Entity Target
    {
        get
        {
            if(_target == null)
            {
                TargetingOverriden = false;
            }
            return _target;
        }
        set
        {
            if(!TargetingOverriden && !PreventTargetOverride)
                _target = value;
        }
    }
    [SerializeField]
    private Entity _target;

    public Entity PEntity
    {
        get
        {
            if (_ParentEntity == null)
            {
                Debug.LogError("AIModule parent Entity was called but missing. " +
                    "Check if this script is orphaned (when the parent gameobject w/ Entity destructs, this should too.)");
                return null;
            }
            return _ParentEntity;
        }
        set
        {
            _ParentEntity = value ?? throw new CriticalNullAssignmentException();
        }
    }
    //DO NOT CALL FROM OTHER THAN AWAKE OR START
    [SerializeField]
    private Entity _ParentEntity;

    public Radar PRadar
    {
        get
        {
            if (_ParentRadar == null)
            {
                Debug.LogError("AIModule parent entity Radarwas called but missing. " +
                    "Check if this script is orphaned (when the parent gameobject w/ Entity destructs, this should too.)");
                return null;
            }
            return _ParentRadar;
        }
        set
        {
            _ParentRadar = value ?? throw new CriticalNullAssignmentException();
        }
    }
    //DO NOT CALL FROM OTHER THAN AWAKE OR START
    [SerializeField]
    private Radar _ParentRadar;

    public virtual void Awake()
    {
        
    }

    public void Start()
    {
        if (_ParentEntity == null)
        {
            _ParentEntity = GetComponent<Entity>();
            if (_ParentEntity == null)
            {
                Debug.LogError("This AIModule was not assigned a parent Entity", this);
                //goto CheckAI;
                goto CheckRadar;
            }
            Debug.LogWarning("It is reccomended that AIModules are manually assigned parent Entities to avoid confusion", this);
        }
        CheckRadar:
        if (_ParentRadar == null)
        {
            _ParentRadar = GetComponent<Radar>();
            if (_ParentRadar == null)
            {
                Debug.LogError("This AIModule was not assigned a Radar", this);
            }
            Debug.LogWarning("It is reccomended that AIModules are manually assigned Radars to avoid confusion", this);
        }

        PostStart();

    }
    public virtual void PostStart()
    {

    }

}
