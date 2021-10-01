using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUIShips : MonoBehaviour
{
    public static GlobalUIShips Instance;

    public List<UIShipSpriteContainer> UIShipSpriteContainers = new List<UIShipSpriteContainer>();

    private void Awake()
    {
        Debug.Log("Loaded GlobalUIShips script");

        Instance = this;
    }

}
