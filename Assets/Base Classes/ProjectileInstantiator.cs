using UnityEngine;

public class ProjectileInstantiator : MonoBehaviour
{

    public Weapon PWeapon;
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
        projectileObject.AllegianceInfo = (AllegianceInfo)projectileObject.gameObject.AddComponent(parentEntity.AllegianceInfo.GetType());
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
        projectileObject.AllegianceInfo = (AllegianceInfo)projectileObject.gameObject.AddComponent(parentEntity.AllegianceInfo.GetType());
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
        projectileObject.AllegianceInfo = (AllegianceInfo)projectileObject.gameObject.AddComponent(parentEntity.AllegianceInfo.GetType());
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

}

public class PrefabNoProjectileComponentException : System.Exception
{

}


