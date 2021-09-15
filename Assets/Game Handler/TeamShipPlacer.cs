using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamShipPlacer : MonoBehaviour
{

    public int InstantiatedCount = 0;

    [Tooltip("NOTE: will add on to existing ShipGeneratorContainers, disregarding the cost of those already in ToPlace.")]
    public bool RandomlyGenerate = false;

    public List<ShipGeneratorContainer> ToPlace = new List<ShipGeneratorContainer>();

    public AllegianceInfo AllegianceToGive;

    public Vector2 TopLeftCorner;

    public Quaternion facing;

    public float PointsToSpend;

    public bool SurpressHugeInstantiationCountWarning = false;

    private void Awake()
    {
        if(AllegianceToGive == null)
        {
            Debug.LogError("Ship placer wasn't given an AllegianceInfo");
        }

        if(RandomlyGenerate && PointsToSpend == 0)
        {
            Debug.LogWarning("PointsToSpend is zero and this has been told to randomly generate...", this);
        }
    }

    void Start()
    {

        if (RandomlyGenerate)
        {
            GenerateThingsToSpawn();
        }

        int WillInstantiateCount = 0;

        if (!SurpressHugeInstantiationCountWarning)
        {
            foreach (ShipGeneratorContainer shipGeneratorContainer in ToPlace)
            {
                WillInstantiateCount += shipGeneratorContainer.ToInstantiate.Count;
            }

            if (WillInstantiateCount > 300)
            {

                Debug.LogError("Warning: this has been set to instantiate a huge amount of ships " +
                    "and will very definitely cause a lot of lag and the editor to freeze. " +
                    "Check/set SurpressHugeInstantiationCountWarning to true to ignore", this);

                throw new HellaShipsException();
            }
        }

    }

    public void GenerateThingsToSpawn()
    {

        while(PointsToSpend > 0)
        {
            float randomNumber = Random.Range(0f, AvailableShipGenerationContainers.Instance.ChanceSumOfAllContainers);

            ShipGeneratorContainer shipGeneratorContainer = AvailableShipGenerationContainers.Instance.GetShipGeneratorContainerByChance(randomNumber);

            ToPlace.Add(shipGeneratorContainer);
            PointsToSpend -= shipGeneratorContainer.Cost;
            if (PointsToSpend - shipGeneratorContainer.Cost < 0) break;
        }
    }


    public void StartPlacing(Vector2 absoluteVariance)
    {

        int WillInstantiateCount = 0;

        if (!SurpressHugeInstantiationCountWarning)
        {
            foreach (ShipGeneratorContainer shipGeneratorContainer in ToPlace)
            {
                WillInstantiateCount += shipGeneratorContainer.ToInstantiate.Count;
            }

            if (WillInstantiateCount > 300)
            {

                Debug.LogError("Warning: this is about to instantiate a huge amount of ships " +
                    "and will very definitely cause a lot of lag and the editor to freeze. " +
                    "Check/set SurpressHugeInstantiationCountWarning to true to ignore", this);
                throw new HellaShipsException();
            }
        }

        Vector2 currentPos = TopLeftCorner;

        foreach(ShipGeneratorContainer shipGeneration in ToPlace)
        {
            Vector2 randomPos = new Vector2()
            {
                x = Random.Range(-absoluteVariance.x, absoluteVariance.x) + currentPos.x,
                y = Random.Range(-absoluteVariance.y, absoluteVariance.y) + currentPos.y,
            };
 
            foreach (GameObject gameObject in shipGeneration.GetInstantiateShips(AllegianceToGive, TopLeftCorner + currentPos + randomPos, facing))
            {
                InstantiatedCount++;
            }

            currentPos.y += shipGeneration.TotalYSpace;
        }

    }

    class HellaShipsException : System.Exception
    {

    }

}