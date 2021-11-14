using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITargetAcquisitor : AIModule
{

    [SerializeField]
    private List<AIModule> ListOfAIModulesToSetTarget = new List<AIModule>();

    public override void PostStart()
    {
        if (ListOfAIModulesToSetTarget.Count == 0)
        {
             Debug.LogWarning("Warning: this AITargetAcquisitor was not assigned an AIModule component", this);
        }
        PostStart();
    }

    public virtual void SetAllTargets(Entity entity)
    {

        for(int i = 0; i < ListOfAIModulesToSetTarget.Count; i++)
        {
            Target = entity;
            ListOfAIModulesToSetTarget[i].Target = entity;
        }
    }

}
