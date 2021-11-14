using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAI_StopInRange : AIMovementController
{
    public bool DoStopInRange = true;
    public float StopDistance = 2f;

    public bool DoBackOffIfTooClose = true;
    public float BackOffDistance = 0.8f;

    public bool DoSmoothStop = true;
    public float SmoothBrakeCoefficient = 1f;
    public float SmoothBrakeDistance = 4f;

    /*
    public float Speed = 2f;
    public float TurnRate = 1f;
    public bool LockMovementToHeading = false;
    public bool DoTurnToTarget = true;

    public bool Pushy = true;
    public float PushDistancePerSecond = 3f;
    */

    //public bool DEBUG_logSmoothBrakeCoefficient = false;

    public override void PostStart()
    {
        if (SmoothBrakeCoefficient < 1f)
        {
            Debug.LogWarning("SmoothBrakeCoefficient is less than 1, this will cause instantaneous velocity reduction (looks weird)", this);
        }

        if (SmoothBrakeDistance < 0f)
        {
            Debug.LogError("SmoothBrakeDistance was set to a negative number", this);
            SmoothBrakeDistance = 0;
        }

        if (StopDistance < BackOffDistance)
        {
            Debug.LogWarning("Setting StopDistance to less than BackOffDistance will cause jittering behavior and screw with aim prediction", this);
        }
    }

    public override void PostFixedUpdate()
    {

        if (!Targets.IsValidTarget(Target))
            return;

        if (DoStopInRange)
        {
            if (GetDistanceToTarget() > StopDistance)
            {
                //if(DEBUG_logSmoothBrakeCoefficient)
                //Debug.Log(GetClampedSmoothSpeedCoefficientTo(PRadar.target.transform.position));
                if (DoSmoothStop)
                {
                    if (!LockMovementToHeading)
                    {
                        Vector2 vecToTarget = GetVectorTo(Target.transform.position);
                        float dist = vecToTarget.magnitude - StopDistance;
                        transform.position += (Vector3)GetNormalizedDirectionVecToTarget() * Speed * GetClampedSmoothSpeedCoefficientTo(dist) * Time.fixedDeltaTime;
                    }
                    else
                    {
                        Vector2 vecToTarget = GetVectorTo(Target.transform.position);
                        float dist = vecToTarget.magnitude - StopDistance;
                        transform.position += (transform.right * Speed * GetClampedSmoothSpeedCoefficientTo(dist) * Time.fixedDeltaTime);
                    }
                }
                else
                {
                    if (!LockMovementToHeading)
                    {
                        transform.position -= (Vector3)GetNormalizedDirectionVecToTarget() * Speed * Time.fixedDeltaTime;
                    }
                    else
                    {
                        transform.position -= transform.right * Speed * Time.fixedDeltaTime;
                    }
                }
                    
                    

            }
            else if (DoBackOffIfTooClose && GetDistanceToTarget() < BackOffDistance)
            {
                if (DoSmoothStop)
                {
                    if (!LockMovementToHeading)
                    {
                        Vector2 vecToTarget = GetVectorTo(Target.transform.position);
                        float dist = vecToTarget.magnitude;
                        transform.position -= (Vector3)GetNormalizedDirectionVecToTarget() * Speed * (1 - GetClampedSmoothSpeedCoefficientTo(dist)) * Time.fixedDeltaTime;
                    } 
                    else
                    {
                        Vector2 vecToTarget = GetVectorTo(Target.transform.position);
                        float dist = vecToTarget.magnitude;
                        transform.position -= transform.right * Speed * (1 - GetClampedSmoothSpeedCoefficientTo(dist)) * Time.fixedDeltaTime;
                    }
                        /*Vector2 vecToTarget = GetVectorTo(target.transform.position);
                        Vector2 targetVecMinusBrakeDist = vecToTarget - vecToTarget.normalized * StopDistance;
                        transform.position -= (Vector3)GetDirectionVecToTarget() * Speed * GetClampedSmoothSpeedCoefficientTo(PEntity.transform.position, targetVecMinusBrakeDist) * Time.fixedDeltaTime;*/
                        
                }
                //transform.position -= GetClampedVecMoveToTarget() * 1/GetClampedSmoothSpeedCoefficientTo(target.transform.position);
                else
                {
                    if (!LockMovementToHeading)
                    {
                        transform.position -= (Vector3)GetNormalizedDirectionVecToTarget() * Speed * Time.fixedDeltaTime;
                    }
                    else
                    {
                        transform.position -= transform.right * Speed * Time.fixedDeltaTime;
                    }
                }
                    
            }
        }
    }

    /*
    public void OnTriggerStay2D(Collider2D collider)
    {
        //Debug.Log("cant have shit in detroit");
        if (Pushy)
        {
            Entity entity = collider.gameObject.GetComponent<Entity>();
            
            if(entity != null)
            {
                entity.pushQueue = ((Vector2)transform.position - (Vector2)entity.transform.position).normalized * PushDistancePerSecond * Time.fixedDeltaTime;
            }
        }
    }
    */

    /*
    Vector2 GetDirectionVecToTarget()
    {
        Vector2 targetPos = target.transform.position;
        Vector2 thisPos = PEntity.transform.position;
        Vector2 vecMove = (targetPos - thisPos).normalized;
        return vecMove;
    }


    float GetDistanceToTarget()
    {
        return Vector2.Distance(PEntity.transform.position, target.transform.position);
    }

    float GetDistanceTo(Vector2 vector)
    {
        return Vector2.Distance(PEntity.transform.position, vector);
    }

    Vector2 GetVectorTo(Vector2 vector)
    {
        return vector - (Vector2)PEntity.transform.position;
    }
    */

    //return value (clamped to 1) = -a(x+1)(x-1) where a is SmoothBrakeCoefficient
    float GetClampedSmoothSpeedCoefficientTo(Vector2 origin, Vector2 vectorToStopAt)
    {
        float dist = Vector2.Distance(origin, vectorToStopAt);
        //Debug.Log(dist);
        float x = 1f - (dist / SmoothBrakeDistance);
        //Debug.Log(x);
        //should never return a negative — rule is: x > 0, x < 1 else return 1; 
        if (x < 0) return 1f;
        return -(SmoothBrakeCoefficient) * ((x * x) - 1f);
    }

    float GetClampedSmoothSpeedCoefficientTo(float distance)
    {
        float x = 1f - (distance / SmoothBrakeDistance);
        //Debug.Log(x);
        //should never return a negative — rule is: x > 0, x < 1 else return 1; 
        if (x < 0) return 1f;
        return -(SmoothBrakeCoefficient) * ((x * x) - 1f);

    }

}
