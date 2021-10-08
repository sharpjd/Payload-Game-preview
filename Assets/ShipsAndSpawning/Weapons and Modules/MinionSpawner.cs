using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSpawner : AIModule
{

    public GameObject ThingToSpawn;
    public float TimeBetweenSpawns = 6f;

    public bool DoNotSpawnIfNoTarget = true;

    float LastTimeSpawned = 0f;

    public override void PostStart()
    {
        if(ThingToSpawn == null)
        {
            Debug.LogError("Nothing assigned to ThingToSpawn");
        }
    }

    public void Update()
    {
        if (Time.time - LastTimeSpawned >= TimeBetweenSpawns)
        {
            if (DoNotSpawnIfNoTarget && Target == null) return;

            LastTimeSpawned = Time.time;
            GameObject instantiatedThing = Pooler.Instance.GetPooledGameObject(PooledObjectType.Minion) ?? Instantiate(ThingToSpawn);

            CarrierControlledEntity instantiatedThingEntity = instantiatedThing.GetComponent<CarrierControlledEntity>();
            if (instantiatedThingEntity != null)
            {

                instantiatedThingEntity.AllegianceInfo = (AllegianceInfo)instantiatedThingEntity.gameObject.AddComponent(PEntity.AllegianceInfo.GetType());

                instantiatedThingEntity.SpriteRenderer.color = instantiatedThingEntity.AllegianceInfo.FactionColor;

                instantiatedThingEntity.MothershipEntity = PEntity;

                instantiatedThingEntity.transform.position = transform.position;
                instantiatedThingEntity.transform.rotation = transform.rotation;

                Component[] spawnedAIModules = instantiatedThingEntity.GetComponents(typeof(AIModule));

                for (int i = 0; i < spawnedAIModules.Length; i++)
                {
                    ((AIModule)spawnedAIModules[i]).Target = Target;
                    ((AIModule)spawnedAIModules[i]).TargetingOverriden = true;
                }

                instantiatedThing.SetActive(true);

            }
        }
    }
}
