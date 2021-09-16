using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proj_Laser : Projectile
{

    public GameObject BeamAnimation;

    public float BeamWidth;

    public override void PostStart()
    {
        PredictCollision();
    }

    private void PredictCollision()
    {
        int layermask = 3 << 2;
        layermask = ~layermask;

        RaycastHit2D[] hits = Physics2D.RaycastAll(gameObject.transform.position, TargetEntity.transform.position - transform.position, DistanceLifespan, layermask);
        //Debug.Log(hits.Length);

        float closestDist = float.MaxValue;
        List<Entity> validHitEntities = new List<Entity>();

        for(int i = 0; i < hits.Length; i++)
        {
            Entity entity = hits[i].collider.gameObject.GetComponent<Entity>();

            if (Targets.IsValidTarget(entity))
            {
                validHitEntities.Add(entity);
            }
        }

        Entity closest = null;

        for (int i = 0; i < validHitEntities.Count; i++)
        {
            float dist = Vector2.Distance(validHitEntities[i].transform.position, transform.position);

            if (AllegianceInfo.CanHit(validHitEntities[i].AllegianceInfo.Faction, validHitEntities[i].AllegianceInfo.ID))
            {
                if (dist < closestDist)
                {
                    closest = validHitEntities[i];
                    closestDist = dist;
                }
            }
        }

        if(closest == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 vecToTarget = closest.transform.position - transform.position;

        closest.HitpointHandler?.OnHit(Damage);
        GameObject beam = Instantiate(BeamAnimation);
        beam.transform.localScale = new Vector3(vecToTarget.magnitude, BeamWidth, beam.transform.localScale.z);
        beam.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(vecToTarget.y, vecToTarget.x) * Mathf.Rad2Deg);
        beam.transform.position = (Vector2)transform.position + (vecToTarget.normalized * vecToTarget.magnitude) / 2;
        beam.GetComponent<SpriteRenderer>().color = AllegianceInfo.FactionColor;

        Destroy(gameObject);
    }


}
