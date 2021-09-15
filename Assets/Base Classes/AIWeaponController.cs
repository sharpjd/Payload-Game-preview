using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWeaponController : AIModule
{
    
    public Weapon Weapon;

    public override void PostStart()
    {
        if(Weapon == null)
        {
            Weapon = (Weapon)GetComponent(typeof(Weapon));
            if(Weapon == null)
            {
                Debug.LogWarning("Warning: this WeaponAI was not assigned and did not find a Weapon component", this);
                goto EndAndPostStart;
            }
            Debug.LogWarning("Warning: it is reccomended to assign WeaponAI a Weapon manually to avoid confusion", this);
        }
        EndAndPostStart:
        PostStart();
    }

    public virtual void PostPostStart()
    {

    }

}
