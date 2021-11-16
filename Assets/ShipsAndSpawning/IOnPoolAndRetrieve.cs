using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Checklist on pooling of object:
 * - Implement this on a class that extends a base object. I tried implementing pooling by default in the Projectile base class
 * because I thought it was a good idea when half asleep, and ended up spending hours undoing spaghetti code damage
 * - Have you destroyed its AllegianceInfo, if it has one and needs to become neutral?
 * - If it's set to be tracked in radar, have you set isDead to true (and isDead to false on retrieval)?
 * - Did you reset/destroy all members that register themselves in another class or need to be null for it
 * to work like a fresh object?
 * - Did you reset all variables to their previous values (perhaps grab default values in Awake()?)
 * 
 * - You can try calling Start() and/or Awake() in your OnRetrieve() function, but realize that this will cause start() to be
 * called when it is retrieved, not the frame after it starts; also make sure variables are returned to their necessary 
 * initial values
 * 
 * Note: GameObjects to be pooled require a PoolableGameObjectLink to function. 
 */
public interface IOnPoolAndRetrieve
{
    public PoolableGameObjectLink PoolableGameObjectLink { get; set; }
    public IOnPoolAndRetrieve OnRetrieve();
    public IOnPoolAndRetrieve OnPool();

}