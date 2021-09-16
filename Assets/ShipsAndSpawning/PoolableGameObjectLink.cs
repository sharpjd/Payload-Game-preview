using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableGameObjectLink : MonoBehaviour
{
    public PooledObjectType PooledObjectType;
}

public enum PooledObjectType
{
    FighterBullet,
    Shrapnel,
    Bomb,
    Missile,
    Minion,
    MinionProjectile,
    Beam
}
