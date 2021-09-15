using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrierControlledEntity : Entity
{

    public Entity MothershipEntity;

    public void Update()
    {
        if(MothershipEntity == null || MothershipEntity.gameObject == null)
        {
            OnDestruction();
        }
    }
}
