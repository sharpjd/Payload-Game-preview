using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvailableShipGenerationContainers : MonoBehaviour
{

    public static AvailableShipGenerationContainers Instance;

    public bool AutoGetFromSameGameObject = true;

    public List<ShipGeneratorContainer> ShipGeneratorContainers = new List<ShipGeneratorContainer>();

    public float ChanceSumOfAllContainers = 0f;

    private void Awake()
    {

        Instance = this;

        ShipGeneratorContainer[] shipGeneratorContainers = GetComponents<ShipGeneratorContainer>();

        if (AutoGetFromSameGameObject)
            ShipGeneratorContainers.AddRange(from sgc in  shipGeneratorContainers
                                             where sgc.enabled 
                                             select sgc);

        foreach(ShipGeneratorContainer shipGeneratorContainer in ShipGeneratorContainers)
        {
            shipGeneratorContainer.ChanceFloor = ChanceSumOfAllContainers;
            ChanceSumOfAllContainers += shipGeneratorContainer.GenerationRarityMultiplier;
            shipGeneratorContainer.ChanceCeiling = ChanceSumOfAllContainers;
        }

    }
    public ShipGeneratorContainer GetShipGeneratorContainerByChance(float number)
    {

        if (number < 0 || number > ChanceSumOfAllContainers) 
        {
            Debug.LogError("Can't input number higher than ChanceSumOfAllContainers or less than 0");
        }

        foreach (ShipGeneratorContainer shipGeneratorContainer in ShipGeneratorContainers)
        {
            if (shipGeneratorContainer.ChanceFloor <= number && number < shipGeneratorContainer.ChanceCeiling)
            {
                return shipGeneratorContainer;
            }
        }

        Debug.Log("Hit the end of GetShipGeneratorContainerByChance and returned ShipGeneratorContainers[0]; I guess that happened");
        //theoretically this should be never reached, or I am just bad and need to learn data structures 
        return ShipGeneratorContainers[0];

    }


}
