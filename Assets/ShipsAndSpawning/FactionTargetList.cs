using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionTargetList : MonoBehaviour
{
    public Factions faction;
    [SerializeField]
    public List<Entity> targets = new List<Entity>();
}