using System;
using Core.MVC;

public class TransitionProxy : IProxy
{
    public Action completeCallback;
    public void RequestTransition(Action callBack = null)
    {
        completeCallback = callBack;
        listener.BroadCast(TransitionEvent.REQUEST_TRANSITION);
    }

    public void TransitionComplete(Action callBack = null)
    {
        completeCallback = callBack;
        listener.BroadCast(TransitionEvent.TRANSITION_COMPLETE);
    }
}