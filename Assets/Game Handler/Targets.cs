using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[DisallowMultipleComponent]
public class Targets : MonoBehaviour
{
    //Do not call this directly
    private static List<FactionTargetList> factionTargetLists = new List<FactionTargetList>();

    public static int FactionListCount { get => factionTargetLists.Count; }

    public float cleanNullIntervalSeconds = 5f;
    //public static float cleanNullIntervalSecondsTimeLeft;

    private void Awake()
    {
        Debug.Log("Global targets handler initialized");
    }

    private void Start()
    {
        Debug.Log("Global targets handler started");
        InvokeRepeating("ClearNullsFromGlobalList", 0, cleanNullIntervalSeconds);
    }

    private void Update()
    {

        

    }

    public static int GetCountOfTargets()
    {
        int targetsTotal = 0;

        foreach(FactionTargetList factionTargetList_ in factionTargetLists)
        {
            targetsTotal += factionTargetList_.targets.Count;
        }

        return targetsTotal;
    }

    public static int GetCountOfValidTargets(int ignoreID)
    {
        int targetsTotal = 0;

        foreach (FactionTargetList factionTargetList_ in factionTargetLists)
        {
            foreach(Entity entity in factionTargetList_.targets)
            {
                if (IsValidTarget(entity) && entity.AllegianceInfo.ID != ignoreID)
                    targetsTotal++;
            }
        }

        return targetsTotal;
    }


    private void ClearNullsFromGlobalList()
    {

        for (int i = 0; i < factionTargetLists.Count; i++)
        {
            for (int j = 0; j < factionTargetLists[i].targets.Count; j++)
            {
                Entity entity = factionTargetLists[i].targets[j];

                if (entity == null || entity.gameObject == null)
                {
                    factionTargetLists[i].targets.RemoveAt(j);
                    j--;
                }
            }
        }

        //Debug.Log("Cleaned " + cleaned + " invalid references from global target list");

        //cleanNullIntervalSecondsTimeLeft = cleanNullIntervalSeconds;
    }


    private static FactionTargetList GetFactionTargetList(Factions faction)
    {
        for (int i = 0; i < factionTargetLists.Count; i++)
        {
            if (factionTargetLists[i] is null)
                Debug.LogError("There is a null FactionTargetList in the global factions list count");

            if (factionTargetLists[i].faction == faction)
            {
                return factionTargetLists[i];
            }
        }
        return null;
    }

    public static void AddAsTarget(Entity self)
    {

        if (!IsValidTarget(self))
            throw new InvalidTargetRegistrant();

        for (int i = 0; i < factionTargetLists.Count; i++)
        {
            if (factionTargetLists[i].faction == self.AllegianceInfo.Faction)
            {
                factionTargetLists[i].targets.Add(self);
                return;
            }
        }

        FactionTargetList newFactionTargetList = new FactionTargetList(self.AllegianceInfo.Faction);
        newFactionTargetList.targets.Add(self);
        factionTargetLists.Add(newFactionTargetList);
    }


    public static Entity GetClosestTarget(Vector2 origin, Entity self, List<ShipType> shipTypesToExclude)
    {
        Entity closestTarget = null;

        for(int i = 0; i < self.AllegianceInfo.TargetFactions.Length; i++)
        { 
            float currentClosestDistance = float.MaxValue;
            List<Entity> targets = GetFactionTargetList(self.AllegianceInfo.TargetFactions[i])?.targets;

            for (int j = 0; j < targets?.Count; j++)
            {
                if (!IsValidTarget(targets[j]))
                    continue;

                float distance = Vector2.Distance(origin, targets[j].gameObject.transform.position);
                if (distance <= currentClosestDistance && targets[j].AllegianceInfo.ID != self.AllegianceInfo.ID && !shipTypesToExclude.Contains(targets[j].ShipType))
                {
                    currentClosestDistance = distance;
                    closestTarget = targets[j];
                }
            }
        }

        return closestTarget;

    }

    public static Entity GetClosestAlly(Vector2 origin, Entity self)
    {
        Entity closestTarget = null;

        float currentClosestDistance = float.MaxValue;
        List<Entity> targets = GetFactionTargetList(self.AllegianceInfo.Faction)?.targets;

        for (int j = 0; j < targets?.Count; j++)
        {
            if (!IsValidTarget(targets[j]))
                continue;

            float distance = Vector2.Distance(origin, targets[j].gameObject.transform.position);
            if (distance <= currentClosestDistance && targets[j].AllegianceInfo.ID != self.AllegianceInfo.ID)
            {
                currentClosestDistance = distance;
                closestTarget = targets[j];
            }
        }

        return closestTarget;

    }


    public static Entity GetClosestTargetInRange(Vector2 origin, Entity self, float rangeRadius, List<ShipType> shipTypesToExclude)
    {
        Entity closestTarget = null;

        for (int i = 0; i < self.AllegianceInfo.TargetFactions.Length; i++)
        {

            float currentClosestDistance = float.MaxValue;
            List<Entity> targets = GetFactionTargetList(self.AllegianceInfo.TargetFactions[i])?.targets;

            for (int j = 0; j < targets?.Count; j++)
            {
                if (!IsValidTarget(targets[j]))
                    continue;

                float distance = Vector2.Distance(origin, targets[j].gameObject.transform.position);
                if (distance <= currentClosestDistance && distance <= rangeRadius && targets[j].AllegianceInfo.ID != self.AllegianceInfo.ID && !shipTypesToExclude.Contains(targets[j].ShipType))
                {
                    currentClosestDistance = distance;
                    closestTarget = targets[j];
                }
            }
        }

        return closestTarget;
    }



    public static List<Entity> GetTargetsWithinRange(Vector2 origin, float rangeRadius, Entity self, List<ShipType> shipTypesToExclude)
    {

        List<Entity> inRangeTargets = new List<Entity>();

        for (int i = 0; i < self.AllegianceInfo.TargetFactions.Length; i++)
        {
            List<Entity> targets = GetFactionTargetList(self.AllegianceInfo.TargetFactions[i])?.targets;

            for (int j = 0; j < targets?.Count; j++)
            {
                if (!IsValidTarget(targets[j]))
                    continue;

                float distance = Vector2.Distance(origin, targets[j].gameObject.transform.position);
                if (distance <= rangeRadius && targets[j].AllegianceInfo.ID != self.AllegianceInfo.ID && !shipTypesToExclude.Contains(targets[j].ShipType))
                {
                    inRangeTargets.Add(targets[j]);
                }
            }
        }

        return inRangeTargets;

    }

    public static List<Entity> GetAllTargetsOfFaction(Factions faction, ShipType shipTypeToExclude)
    {
        List<Entity> entities = GetFactionTargetList(faction).targets;

        for (int i = entities.Count - 1; i >= 0; i--)
        {
            if(entities[i].ShipType == shipTypeToExclude)
            {
                entities.Remove(entities[i]);
            }
        }

        return entities;

    }

    public static List<Entity> GetAllTargetsOfFactionRemoveInvalids(Factions faction, ShipType shipTypeToExclude)
    {
        List<Entity> entities = GetFactionTargetList(faction).targets;

        for (int i = entities.Count - 1; i >= 0; i--)
        {
            if (entities[i].ShipType == shipTypeToExclude || !IsValidTarget(entities[i]))
            {
                entities.Remove(entities[i]);
            }
        }

        return entities;

    }


    public static bool IsValidTarget(Entity entity)
    {
        if (entity == null || entity.gameObject == null || entity.AllegianceInfo == null || entity.IsDead) return false;
        return true;
    }

    public static FactionTargetList[] GetAllFactionTargetLists()
    {
        return factionTargetLists.ToArray();
    }

    public class FactionTargetList
    {
        public readonly Factions faction;
        [SerializeField]
        public List<Entity> targets = new List<Entity>();

        public FactionTargetList(Factions faction)
        {
            this.faction = faction;
        }
    }

    public class InvalidTargetRegistrant : Exception
    {
        public InvalidTargetRegistrant()
        {

        }
    }

}


