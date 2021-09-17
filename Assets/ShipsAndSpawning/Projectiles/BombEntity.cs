using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEntity : Entity
{
    public Proj_Bomb Proj_Bomb;

    public override void OnDestruction()
    {

        if (didDestruct) return;

        didDestruct = true;

        Proj_Bomb.OnDestruction();
        
    }

}
