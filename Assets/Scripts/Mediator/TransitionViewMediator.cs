using Core.MVC;
using UnityEngine;

public class TransitionViewMediator : BaseMediator<TransitionView>
{
    [Listener(TransitionEvent.REQUEST_TRANSITION)]
    public void OnRequestTransition()
    {
        view.FadeOut(0.5f);
    }

    [Listener(TransitionEvent.TRANSITION_COMPLETE)]
    public void OnTransitionComplete()
    {
        view.FadeIn(0.5f);
    }
}
