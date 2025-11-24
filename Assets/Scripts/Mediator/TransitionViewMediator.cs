using System;
using Core.MVC;
using UnityEngine;
using Zenject;

public class TransitionViewMediator : BaseMediator<TransitionView>
{
    [Inject] private TransitionProxy transitionProxy;
    [Listener(TransitionEvent.REQUEST_TRANSITION)]

    public void OnRequestTransition()
    {
        Action callBack = transitionProxy.completeCallback;
        transitionProxy.completeCallback = null;
        view.FadeOut(0.5f, () =>
        {
            callBack?.Invoke();
            listener.BroadCast(TransitionEvent.FADE_OUT_COMPLETE);
        });
    }

    [Listener(TransitionEvent.TRANSITION_COMPLETE)]
    public void OnTransitionComplete()
    {
        Action callBack = transitionProxy.completeCallback;
        transitionProxy.completeCallback = null;
        view.FadeIn(0.5f, () =>
        {
            listener.BroadCast(TransitionEvent.FADE_IN_COMPLETE);
        });
    }
}
