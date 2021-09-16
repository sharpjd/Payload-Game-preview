using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnPoolAndRetrieve
{
    public IOnPoolAndRetrieve OnRetrieve();
    public IOnPoolAndRetrieve OnPool();

}