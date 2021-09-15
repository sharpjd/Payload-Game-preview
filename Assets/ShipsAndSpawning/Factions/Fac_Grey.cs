using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fac_Grey : AllegianceInfo
{
    public override Factions Faction { get => Factions.Grey; set => throw new System.NotImplementedException(); }
    public override Factions[] TargetFactions { get => OnlyTargetAllies ? new Factions[] { Faction } : new Factions[] { Factions.Red, Factions.Independent }; set => throw new System.NotImplementedException(); } 
    public override int ID { get; set; }
    public override Color FactionColor { get => new Color(1, 1, 1); set => throw new NotImplementedException(); }
}

