using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameState : MonoBehaviour
{

    public GameObject SelectionGameObject;

    public GameObject TargetGameObject;

    public static GameState Instance;

    public AllegianceInfo PlayerAllegiance;

    public GameObject DefaultDestructionPrefab;

    private void Awake()
    {
        Instance = this;

        Debug.Log("Started game state instance");
        if(PlayerAllegiance == null)
        {
            Debug.LogError("PlayerFaction is missing");
        }

        if (SelectionGameObject == null)
        {
            Debug.LogError("Selection highlight prefab is missing");
        }

        if (TargetGameObject == null)
        {
            Debug.LogError("Target highlight prefab is missing");
        }

        if (DefaultDestructionPrefab == null)
        {
            Debug.LogError("Default destruction animation prefab is missing");
        }


    }


}
