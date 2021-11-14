using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamLineupContainer : MonoBehaviour
{

    public AllegianceInfo AllegianceInfo;

    //private List<GameObject> Lineup = new List<GameObject>();

    public List<UIShipSpriteContainer> LineupUIShipSpriteContainers = new List<UIShipSpriteContainer>();

    private void Start()
    {
        {
            LineupUIShipSpriteContainers.AddRange(GlobalUIShips.Instance.UIShipSpriteContainers.ToArray());
            //if (uIShipSpriteContainer == null) Debug.LogError("UISpriteContainer GameObject doesn't contain the relevant script", uIShipSpriteContainer);
            //if(uIShipSpriteContainer != null)  
        }
    }

    public void UpdateLineupCounts()
    {

        List<Entity> teamEntities = Targets.GetAllTargetsOfFaction(AllegianceInfo.Faction);

        foreach (UIShipSpriteContainer uIShipSprite in LineupUIShipSpriteContainers)
        {
            int count = 0;
            for (int i = 0; i < teamEntities.Count; i++)
            {
                if(teamEntities[i].ShipType == uIShipSprite.ShipType)
                {
                    count++;
                }
            }

            uIShipSprite.Count = count;
        }
    }
}
