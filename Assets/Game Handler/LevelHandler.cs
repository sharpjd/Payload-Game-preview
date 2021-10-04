using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelHandler : MonoBehaviour
{

    public bool DoRandomGeneration = true;

    public LevelState LevelState;

    public float EnemyBank;
    public float FriendlyBank;

    //public List<ShipGeneratorContainer> AvailableToGenerate = new List<ShipGeneratorContainer>();

    [HideInInspector]
    public static LevelHandler Instance;

    public List<TeamShipPlacer> TeamsToPlace = new List<TeamShipPlacer>();

    public Vector2 PositionalVariance = new Vector2();


    void Start()
    {

        Debug.Log("Level handler started");

        Instance = this;

        LevelState = LevelState.Generating;
    }

    bool GenerationComplete;
    bool PreparingComplete;



    void Update()
    {
        if(LevelState == LevelState.Generating)
        {
            foreach(TeamShipPlacer teamShipPlacer in TeamsToPlace)
            {
                teamShipPlacer.StartPlacing(PositionalVariance);
            }
            ChangeLevelState(LevelState.Preparing);
        }

        if(LevelState == LevelState.Preparing)
        {

        }


    }

    public void CheckForTeamEliminated(Factions factions)
    {
        CheckForVictory();
        CheckForDefeat();
    }

    public void CheckForDefeat()
    {

        int count = Targets.GetAllTargetsOfFactionRemoveInvalidsAndNonCounts
            (
            GameState.Instance.PlayerAllegiance.Faction,
            new List<ShipType>(new ShipType[] { ShipType.Missile }
            )
                ).Where(a => a.CountTowardsTeamCount).ToList().Count;

        if (count == 0)
            ChangeLevelState(LevelState.Defeat);

    }


    public void CheckForVictory()
    {

        List<FactionTargetList> factionTargetLists = new List<FactionTargetList>(Targets.GetAllFactionTargetLists());

        //remove the player's faction from this list 
        for (int i = factionTargetLists.Count - 1; i >= 0; i--)
        {
            if (factionTargetLists[i].faction == GameState.Instance.PlayerAllegiance.Faction)
            {
                factionTargetLists.RemoveAt(i);
            }
        }

        //iterate through all available entities; if any valid target is found, return
        for (int i = factionTargetLists.Count - 1; i >= 0; i--)
        {

            int count = Targets.GetAllTargetsOfFactionRemoveInvalidsAndNonCounts
            (
            factionTargetLists[i].faction,
            new List<ShipType>(new ShipType[] { ShipType.Missile }
            )
                ).Where(a => a.CountTowardsTeamCount).ToList().Count;

            if (count > 0) return;
        }

        ChangeLevelState(LevelState.Victory);
    }



    public void ChangeLevelState(LevelState state)
    {
        LevelState = state;
    }

    

}

public enum LevelState
{
    Generating,
    Preparing,
    Started,
    Victory,
    Defeat
}