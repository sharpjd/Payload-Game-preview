using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FrameBasedExecutor : MonoBehaviour
{

    public static FrameBasedExecutor Instance;

    private static LinkedList<FrameAction> FrameActions = new LinkedList<FrameAction>();

    void Start()
    {
        Instance = this;
        Debug.Log("Started FrameBasedExecutor");
    }

    void Update()
    {
        for (int i = 0; i < FrameActions.Count; i++)
        {
            LinkedListNode<FrameAction> linkedListNode = FrameActions.First;

            if (linkedListNode.Value.FrameToExecute == Time.frameCount)
            {
                linkedListNode.Value.Action.Invoke();
                FrameActions.RemoveFirst();
            } else
            {
                FrameActions.RemoveFirst();
                FrameActions.AddLast(linkedListNode);
            }
        }
    }

    public FrameAction ExecuteNextFrame(Action action)
    {
        return EnqueueAction(action, Time.frameCount + 1);
    }

    private FrameAction EnqueueAction(Action action, int executionFrame, bool surpressWarningForMultiFrameDelay = false)
    {

        if(!surpressWarningForMultiFrameDelay && Time.frameCount - executionFrame > 1)
        {
            Debug.LogError("Warning: there should be very few circumstances you need a method to execute after more than" +
                " one frame (dirty code?), pass true to surpressWarningForMultiFrameDelay as a parameter to surpress error");
        }

        FrameAction frameAction = new FrameAction(action, executionFrame);

        FrameActions.AddLast(frameAction);

        return frameAction;

    }

    public class FrameAction
    {
        public int FrameToExecute;

        public Action Action;

        public FrameAction(Action action, int frameToExecute)
        {
            if (action == null)
                throw new FrameActionNullActionException();

            if(frameToExecute <= Time.frameCount)
            {
                throw new FrameActionInvalidInputFrameException(frameToExecute);
            }

            FrameToExecute = frameToExecute;
            Action = action;
        }

        public void Cancel()
        {
            FrameActions.Remove(this);
        }

    }
}

public class FrameActionNullActionException : Exception
{

}

public class FrameActionInvalidInputFrameException : Exception
{
    public FrameActionInvalidInputFrameException(int frame)
    {
        Debug.LogError("Tried to make a FrameAction with an invalid frame or frame <= current frame: " + frame);
    }
}