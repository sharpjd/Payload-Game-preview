using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElementTransformController : MonoBehaviour
{
    // Start is called before the first frame update

    public float TargetRelativePositionY;
    public float TargetRelativePositionX;

    void Start()
    {
        UIElementsHandler.UIElementTransforms.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
