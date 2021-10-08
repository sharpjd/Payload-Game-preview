using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipGeneratorContainer : MonoBehaviour
{

    public List<GameObject> ToInstantiate = new List<GameObject>();
    public float Cost;

    public float YGapBetweenObjects = -2f;
    public int MaxColumnFormationHeightCount = 3;
    public float XGapBetweenColumns = 2f;

    public float TotalYSpace;

    public float GenerationRarityMultiplier = 1f;

    public float ChanceFloor;
    public float ChanceCeiling;

    private void Awake()
    {
        TotalYSpace = MaxColumnFormationHeightCount * YGapBetweenObjects;
        if(GenerationRarityMultiplier > 1f || GenerationRarityMultiplier < 0f)
        {
            Debug.LogError("GenerationRarityMultiplier must be > 0 to work and 1 < to avoid diluting the spawning chance pool");
        }
    }

    private void Start()
    {
        if(ToInstantiate.Count == 0)
        {
            Debug.LogError("Nothing to instantiate assigned to this ShipGenerationContainer");
        }
    }

    public IEnumerable<GameObject> GetInstantiateShips(AllegianceInfo allegianceInfo, Vector2 origin, Quaternion facing)
    {

        int itemsInColumn = 0;
        float columnCount = 0;

        foreach (GameObject toInstantiate in ToInstantiate)
        {

            if (itemsInColumn >= MaxColumnFormationHeightCount)
            {
                itemsInColumn = 0;
                columnCount++;
            }

            float currentX = MaxColumnFormationHeightCount * columnCount;

            GameObject instantiatedThing = Instantiate(toInstantiate);
            Entity entityScript = instantiatedThing.GetComponent<Entity>();
            entityScript.AllegianceInfo = (AllegianceInfo)entityScript.gameObject.AddComponent(allegianceInfo.GetType());

            instantiatedThing.transform.position = new Vector3(origin.x + (columnCount * XGapBetweenColumns), origin.y + (itemsInColumn * YGapBetweenObjects), instantiatedThing.transform.position.z);
            instantiatedThing.transform.rotation = facing;

            itemsInColumn++;

            yield return instantiatedThing;
        }
    }


}
