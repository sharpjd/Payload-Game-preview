using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Radar : MonoBehaviour
{
    public Entity ParentEntity
    {
        get
        {
            if (_parentEntity is null)
            {
                Debug.LogError("Radar parent entity was called but missing. " +
                    "Check if this script is orphaned");
                return null;
            }
            return _parentEntity;
        }
        set
        {
            _parentEntity = value ?? throw new CriticalNullAssignmentException();
        }
    }
    [Tooltip("Critical; searches gameObject for component if not assigned")]
    private Entity _parentEntity;

    public void Awake()
    {
        _parentEntity ??= GetComponent<Entity>();
        PostAwake();
    }

    public virtual void PostAwake()
    {

    }

    public void Start()
    {
        if (_parentEntity is null)
        {
            Debug.LogError("This radar lacks a parent entity — it failed to find one, and/or was not assigned one. Disabling");
            enabled = false;
            return;
        }

        //DEBUG 
        //InvokeRepeating("FindNewTargetIfIdle", 3, radarRefreshTime);
        PostStart();
    }

    public virtual void PostStart()
    {

    }

    public virtual void Update()
    {
        /* DEBUG
         * if (target == null)
            FindNewTarget();
         */
    }
    /* DEBUG
    public virtual void FindNewTarget()
    {
        target = Targets.GetClosestTarget(ParentEntity.transform.position, ParentEntity);
        if (target == null)
        {
            Debug.LogWarning("There was no/null target found after calling FindNewTarget(); " +
                "there are currently " + Targets.GetCountOfTargets() +
                " global targets and " + Targets.GetCountOfValidTargets(ParentEntity.AllegianceInfo.ID)
                + " valid targets, as well as " + Targets.FactionListCount + " registered factions");
        }
    }
    */
}

