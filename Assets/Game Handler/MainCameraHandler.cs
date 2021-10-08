using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraHandler : MonoBehaviour
{

    public static MainCameraHandler Instance;

    Camera AttachedCamera;

    public float InitialZoom = 10f;
    public float TargetZoom = 10f;
    public float ZoomScrollChange = 0.25f;
    public float ZoomScrollChangeDivisor = 1.5f;
    public float ZoomVelocityDivisor = 0.5f;

    public float MovementSpeedPerSecond = 1f;
    public float MovementMultipliedByZoomCoefficient = 1f;

    private void Awake()
    {
        Instance = this;
        AttachedCamera ??= GetComponent<Camera>();
        AttachedCamera.orthographicSize = InitialZoom;
    }

    private void Update()
    {

        if (!(TargetZoom + Input.mouseScrollDelta.y * -ZoomScrollChange <= 0))
            TargetZoom += Input.mouseScrollDelta.y * -ZoomScrollChange * (TargetZoom / ZoomScrollChangeDivisor);
        else return;

        
        AttachedCamera.orthographicSize += (TargetZoom - AttachedCamera.orthographicSize) / ZoomVelocityDivisor * Time.deltaTime;

        if (AttachedCamera.orthographicSize <= 0)
            AttachedCamera.orthographicSize = 0;

        transform.position += (Vector3)new Vector2(Input.GetAxis("Horizontal") *  Time.deltaTime, Input.GetAxis("Vertical") * Time.deltaTime) * MovementSpeedPerSecond * (TargetZoom / InitialZoom) * MovementMultipliedByZoomCoefficient;


    }


}
