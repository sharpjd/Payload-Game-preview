using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class AllegianceInfo : MonoBehaviour
{
    public static int globalIDCount = 0;

    private void Awake()
    {
        ID = globalIDCount;
        globalIDCount++;
        //Debug.Log("Global ID count is now " + globalIDCount);
    }

    public abstract int ID { get; set; }
    public abstract Factions Faction { get; set; }
    public abstract Factions[] TargetFactions { get; set; }
    public abstract Color FactionColor { get; set; }

    public bool OnlyTargetAllies = false;

    public bool CanHit(Factions faction, int targetID)
    {

        for(int i = 0; i < TargetFactions.Length; i++)
        {
            //Debug.Log((faction == TargetFactions[i]) + ", " + (ID == targetID));
            if (TargetFactions[i] == faction && ID != targetID)
                return true;
        }
        return false;
    }

    public bool CanHitIgnoresID(Factions faction)
    {

        for (int i = 0; i < TargetFactions.Length; i++)
        {
            //Debug.Log((faction == TargetFactions[i]) + ", " + (ID == targetID));
            if (TargetFactions[i] == faction)
                return true;
        }
        return false;
    }

    public AllegianceInfo Clone(GameObject self)
    {
        return (AllegianceInfo)self.AddComponent(this.GetType());
    }

}

public enum Factions
{
    Grey,
    Red,
    Independent
}