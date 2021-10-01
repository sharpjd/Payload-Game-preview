using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    /*
     * NOTE: Weapons should preferably be in separate objects. If they are, you can still sync component script refs between the
     * parent object and child objects. Putting them in separate objects allows them to have separate Radar and Entity scripts.
     */
    public bool OnlyFireWithinRange = true;
    public float Range;
    public float FireRateSecs = 1f;
    public int MagazineSizeMax = 1;
    public float ReloadTimeSecs = 3f;

    public GameObject ToInstantiate
    {
        get
        {
            return _ToInstantiate;
        }
        set
        {
            _ToInstantiate = value ?? throw new CriticalNullAssignmentException();
        }
    }
    [Tooltip("Crtitical; must be assigned")]
    [SerializeField]
    private GameObject _ToInstantiate;

    public Projectile ProjectileScript
    {
        get
        {
            if(_ProjectileScript == null)
            Debug.LogError("Weapon Projectile Script was called but missing. " +
                    "Check if the is orphaned or removed by something else", this);
            return _ProjectileScript;
        }
        set
        {
            _ProjectileScript = value ?? throw new CriticalNullAssignmentException();
        }
    }
    [HideInInspector]
    private Projectile _ProjectileScript;


    public Entity PEntity
    {
        get
        {
            if (_ParentEntity == null)
            {
                Debug.LogError("Weapon parent Entity was called but missing. " +
                    "Check if this script is orphaned (when the parent gameobject w/ Entity destructs, this should too.)", this);
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
    [Tooltip("Critical; searches gameObject for component if not assigned")]
    [SerializeField]
    private Entity _ParentEntity;

    public Radar ARadar
    {
        get
        {
            if (_AttachedRadar == null)
            {
                Debug.LogError("Weapon Radar was called but missing. " +
                    "Check if this script is orphaned (when the parent gameobject w/ Entity destructs, this should too.)", this);
                return null;
            }
            return _AttachedRadar;
        }
        set
        {
            _AttachedRadar = value ?? throw new CriticalNullAssignmentException();
        }
    }
    //DO NOT CALL FROM OTHER THAN AWAKE OR START
    [Tooltip("Critical; searches gameObject for component if not assigned")]
    [SerializeField]
    private Radar _AttachedRadar;

    /*
    public AIModule AI
    {
        get
        {
            if (_AI is null)
            {
                Debug.LogError("Weapon AIModule was called but missing. " +
                    "Check if this script is orphaned (when the parent gameobject w/ Entity destructs, this should too.)");
                return null;
            }
            return _AI;
        }
        set
        {
            _AI = value ?? throw new CriticalNullAssignmentException();
        }
    }
    
    
    //DO NOT CALL FROM OTHER THAN AWAKE OR START
    [Tooltip("Critical; searches gameObject for component if not assigned")]
    [SerializeField]
    private AIModule _AI;
    */

    public virtual void Awake()
    {       

    }

    public void Start()
    {

        if (_ParentEntity == null)
        {
            _ParentEntity = GetComponent<Entity>();
            if(_ParentEntity != null)
                Debug.LogWarning("It is reccomended that Weapons are manually assigned parent Entities to avoid confusion", this);
        }
        if (_AttachedRadar == null)
        {
            _AttachedRadar = GetComponent<Radar>();
            if(_AttachedRadar != null)
                Debug.LogWarning("It is reccomended that Weapons are manually assigned Radars to avoid confusion", this);
        }
        
        _ProjectileScript = (Projectile)ToInstantiate.gameObject.GetComponent(typeof(Projectile));
        if(_ProjectileScript == null)
        {
            throw new PrefabNoProjectileComponentException();
        }

        shotsLeft = MagazineSizeMax;
        Range = ProjectileScript.DistanceLifeSpan;
        PostStart();
    }

    public virtual void PostStart()
    {

    }

    public virtual bool IsInRange(Entity target)
    {
        if (Vector2.Distance(transform.position, target.transform.position) <= Range)
            return true;
        return false;
    }

    public virtual bool IsInRange(GameObject gameObject)
    {
        if (Vector2.Distance(transform.position, gameObject.transform.position) <= Range)
            return true;
        return false;
    }

    public int shotsLeft;
    public bool reloading = false;
    public bool onCoolDown = false;

    //the following three methods are crappy code I copypasted from an older program and modified because I was tired
    public virtual void TryFire(Entity target)
    {

        if (shotsLeft <= 0 && !reloading)
        {
            reloading = true;
            Invoke("reload", ReloadTimeSecs);
        }

        if (!reloading)
        {
            if (!(shotsLeft <= 0))
            {
                if (!onCoolDown)
                {
                    //wow this code is awful
                    if (OnlyFireWithinRange && Vector2.Distance(transform.position, target.transform.position) <= Range)
                    {
                        InstantiateProjectileTowardsTargetWithParent(ToInstantiate, PEntity, target, this);
                        goto EndAndInvokeCooldown;
                    }
                    //this will only be executed if OnlyFireWithinRange is false
                    InstantiateProjectileTowardsTargetWithParent(ToInstantiate, PEntity, target, this);

                    EndAndInvokeCooldown:
                    shotsLeft--;
                    onCoolDown = true;
                    Invoke("coolDown", FireRateSecs);
                }
            }
        }

    }
    void coolDown()
    {
        onCoolDown = false;
    }
    void reload()
    {
        shotsLeft = MagazineSizeMax;
        reloading = false;
    }

    /*
     * Source for the predictive aim function below because I am bad at math:
     * https://gamedevelopment.tutsplus.com/tutorials/unity-solution-for-hitting-moving-targets--cms-29633
     */

    public Vector2 PredictShotTo(Transform targetTransform, Vector2 targetVelocity)
    {
        //Debug.DrawLine(transform.position, targetTransform.position, Color.red, 1f);
        float a = (targetVelocity.x * targetVelocity.x) + (targetVelocity.y * targetVelocity.y) - (ProjectileScript.Velocity * ProjectileScript.Velocity);
        float b = 2 * (targetVelocity.x * (targetTransform.position.x - transform.position.x)
            + targetVelocity.y * (targetTransform.transform.position.y - transform.position.y));
        float c = ((targetTransform.transform.position.x - transform.position.x) * (targetTransform.transform.position.x - transform.position.x)) +
            ((targetTransform.transform.position.y - transform.position.y) * (targetTransform.transform.position.y - transform.position.y));

        float disc = b * b - (4 * a * c);
        if (disc < 0)
        {
            return new Vector2(targetTransform.position.x, targetTransform.position.y);
        } 
        //else 
        //{
            float t1 = (-1 * b + Mathf.Sqrt(disc)) / (2 * a);
            float t2 = (-1 * b - Mathf.Sqrt(disc)) / (2 * a);
            float t = Mathf.Max(t1, t2);// let us take the larger time value 
            float aimX = (targetVelocity.x * t) + targetTransform.transform.position.x;
            float aimY = targetTransform.gameObject.transform.position.y + (targetVelocity.y * t);

            return new Vector2(aimX, aimY);

        //}
    }

    public Projectile InstantiateProjectileHere(GameObject thingToInstantiate)
    {
        Projectile projectileObject = Instantiate(thingToInstantiate).GetComponent<Projectile>() ?? throw new PrefabNoProjectileComponentException(); ;

        projectileObject.transform.position = transform.position;
        projectileObject.prevPosition = transform.position;

        projectileObject.InstantiatedFromInstantiater = true;
        projectileObject.gameObject.SetActive(true);
        return projectileObject;
    }

    public Projectile InstantiateProjectileAtParent(GameObject thingToInstantiate, Entity parentEntity)
    {
        Projectile projectileObject = Instantiate(thingToInstantiate).GetComponent<Projectile>() ?? throw new PrefabNoProjectileComponentException(); ;
        projectileObject.AllegianceInfo = parentEntity.AllegianceInfo.Clone(projectileObject.gameObject);
        projectileObject.PEntity = parentEntity;

        projectileObject.transform.position = parentEntity.transform.position;
        projectileObject.prevPosition = parentEntity.transform.position;

        projectileObject.SpriteRenderer.color = parentEntity.AllegianceInfo.FactionColor;

        projectileObject.InstantiatedFromInstantiater = true;
        projectileObject.gameObject.SetActive(true);
        return projectileObject;
    }

    public Projectile InstantiateProjectileAtParentWithTarget(GameObject thingToInstantiate, Entity parentEntity, Entity targetEntity)
    {
        Projectile projectileObject = Instantiate(thingToInstantiate).GetComponent<Projectile>() ?? throw new PrefabNoProjectileComponentException(); ;
        projectileObject.AllegianceInfo = parentEntity.AllegianceInfo.Clone(projectileObject.gameObject);
        projectileObject.PEntity = parentEntity;

        projectileObject.transform.position = parentEntity.transform.position;
        projectileObject.prevPosition = parentEntity.transform.position;

        projectileObject.TargetEntity = targetEntity;

        projectileObject.SpriteRenderer.color = parentEntity.AllegianceInfo.FactionColor;

        projectileObject.InstantiatedFromInstantiater = true;
        projectileObject.gameObject.SetActive(true);
        return projectileObject;
    }

    public Projectile InstantiateProjectileTowardsTargetWithParent(GameObject thingToInstantiate, Entity parentEntity, Entity targetEntity, Weapon callerForShotPrediction, bool pointToTarget = true, bool predictPosition = true)
    {
        Projectile projectileObject = Instantiate(thingToInstantiate).GetComponent<Projectile>() ?? throw new PrefabNoProjectileComponentException();
        projectileObject.AllegianceInfo = parentEntity.AllegianceInfo.Clone(projectileObject.gameObject);
        projectileObject.PEntity = parentEntity;

        projectileObject.transform.position = parentEntity.transform.position;
        projectileObject.prevPosition = parentEntity.transform.position;

        projectileObject.SpriteRenderer.color = parentEntity.AllegianceInfo.FactionColor;

        projectileObject.TargetEntity = targetEntity;
        if (pointToTarget)
        {
            //There is definitely some funky stuff going on here; try changing rotation manually
            if (predictPosition)
            {
                projectileObject.PointTowardsAndSetRotationXYToZero(callerForShotPrediction.PredictShotTo(targetEntity.transform, targetEntity.DeltaVelocity));
            }
            else
            {
                projectileObject.PointTowardsAndSetRotationXYToZero(targetEntity.transform.position);
            }
        }
        projectileObject.InstantiatedFromInstantiater = true;
        projectileObject.gameObject.SetActive(true);
        return projectileObject;
    }

    public Projectile InstantiateProjectileTowardsTargetAt(GameObject thingToInstantiate, Transform transform, Entity targetEntity, Weapon callerForShotPrediction, bool pointToTarget = true, bool predictPosition = true)
    {
        Projectile projectileObject = Instantiate(thingToInstantiate).GetComponent<Projectile>() ?? throw new PrefabNoProjectileComponentException(); ;

        projectileObject.transform.position = transform.position;
        projectileObject.prevPosition = transform.position;

        projectileObject.TargetEntity = targetEntity;
        if (pointToTarget)
        {
            //There is definitely some funky stuff going on here; try changing rotation manually
            if (predictPosition)
            {
                projectileObject.PointTowardsAndSetRotationXYToZero(callerForShotPrediction.PredictShotTo(targetEntity.transform, targetEntity.DeltaVelocity));
            }
            else
            {
                projectileObject.PointTowardsAndSetRotationXYToZero(targetEntity.transform.position);
            }
        }
        projectileObject.InstantiatedFromInstantiater = true;
        projectileObject.gameObject.SetActive(true);
        return projectileObject;
    }

    public Projectile PointProjectileTowardsTargetAtAndSetParent(GameObject thingToManipulate, Entity parentEntity, Entity targetEntity, Transform transform, Weapon callerForShotPrediction, bool pointToTarget = true, bool predictPosition = true)
    {
        Projectile projectileObject = thingToManipulate.GetComponent<Projectile>() ?? throw new PrefabNoProjectileComponentException();

        projectileObject.AllegianceInfo = parentEntity.AllegianceInfo.Clone(projectileObject.gameObject);
        projectileObject.SpriteRenderer.color = projectileObject.AllegianceInfo.FactionColor;
        projectileObject.PEntity = parentEntity;

        projectileObject.transform.position = transform.position;
        projectileObject.prevPosition = transform.position;

        projectileObject.TargetEntity = targetEntity;
        if (pointToTarget)
        {
            //There is definitely some funky stuff going on here; try changing rotation manually
            if (predictPosition)
            {
                projectileObject.PointTowardsAndSetRotationXYToZero(callerForShotPrediction.PredictShotTo(targetEntity.transform, targetEntity.DeltaVelocity));
            }
            else
            {
                projectileObject.PointTowardsAndSetRotationXYToZero(targetEntity.transform.position);
            }
        }
        projectileObject.InstantiatedFromInstantiater = true;
        projectileObject.gameObject.SetActive(true);
        return projectileObject;
    }

}

public class PrefabNoProjectileComponentException : System.Exception
{

}


public class CannotHitException : Exception
{

}






