using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnPoolAndRetrieve
{
    public PoolableGameObjectLink PoolableGameObjectLink { get; set; }
    public IOnPoolAndRetrieve OnRetrieve();
    public IOnPoolAndRetrieve OnPool();

}