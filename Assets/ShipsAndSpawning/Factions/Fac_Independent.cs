using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fac_Independent : AllegianceInfo
{
    public override Factions Faction { get => Factions.Independent; set => throw new System.NotImplementedException(); }
    public override Factions[] TargetFactions { get => OnlyTargetAllies ? new Factions[] { Faction } :  new Factions[] { Factions.Grey, Factions.Independent, Factions.Red }; set => throw new System.NotImplementedException(); }
    public override int ID { get; set; }

    public override Color FactionColor { get => new Color(1, 1, 0); set => throw new NotImplementedException(); }
}