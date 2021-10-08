using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fac_Red : AllegianceInfo
{
    public override int ID { get; set; }
    public override Factions Faction { get => Factions.Red; set => throw new System.NotImplementedException(); }
    public override Factions[] TargetFactions
    {
        get => OnlyTargetAllies ? new Factions[] { Faction } : new Factions[] { Factions.Independent, Factions.Grey }; set => throw new System.NotImplementedException();
    }

    public override Color FactionColor { get => new Color(1, 0, 0); set => throw new NotImplementedException(); }
}
