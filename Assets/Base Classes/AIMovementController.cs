using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovementController : AIModule
{

    public float Speed = 2f;
    public float TurnRatePerSecond = 45f;
    public float PointAngleOffset = 0f;
    public bool LockMovementToHeading = false;
    public bool DoTurnToTarget = true;
    

    public bool Pushy = true;
    public float PushDistancePerSecond = 3f;

    public void FixedUpdate()
    {
        transform.position -= (Vector3)PEntity.pushQueue;
        PEntity.pushQueue = Vector2.zero;

        if(Target! != null)
            TurnToVec(Target.gameObject.transform.position);

        PostFixedUpdate();
    }

    public virtual void PostFixedUpdate()
    {

    }

    public virtual void OnTriggerStay2D(Collider2D collider)
    {
        if (Pushy)
        {
            Entity entity = collider.gameObject.GetComponent<Entity>();

            if (entity != null)
            {
                entity.pushQueue = ((Vector2)transform.position - (Vector2)entity.transform.position).normalized * PushDistancePerSecond * Time.fixedDeltaTime;
            }
        }
    }

    public Vector2 GetNormalizedDirectionVecToTarget()
    {
        Vector2 targetPos = Target.transform.position;
        Vector2 thisPos = PEntity.transform.position;
        Vector2 vecMove = (targetPos - thisPos).normalized;
        return vecMove;
    }

    public float GetDistanceToTarget()
    {
        return Vector2.Distance(PEntity.transform.position, Target.transform.position);
    }

    public float GetDistanceTo(Vector2 vector)
    {
        return Vector2.Distance(PEntity.transform.position, vector);
    }

    public Vector2 GetVectorTo(Vector2 vector)
    {
        return vector - (Vector2)PEntity.transform.position;
    }


    public void TurnToVec(Vector2 vec)
    {

        Vector2 direction = vec - (Vector2)transform.position;
        //Debug.DrawRay(transform.position, transform.up, Color.red);
        //Debug.DrawRay(transform.position, direction, Color.green);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + PointAngleOffset;
        float posAngle = Vector2.Angle(direction, transform.right);
        /* if (useArcFacing)
        {
            float angleToMouseFromFacing = Vector2.Angle(arcBase.transform.up, direction);
            float angleFacing = Vector2.Angle(arcBase.transform.up, transform.up);
            if (angleFacing > rotationArc && angleToMouseFromFacing > rotationArc)
                return;
        } */
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, TurnRatePerSecond * Time.deltaTime / posAngle);
    }
}
