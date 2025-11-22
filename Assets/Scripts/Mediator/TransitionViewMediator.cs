using Core.MVC;
using UnityEngine;

public class TransitionViewMediator : BaseMediator<TransitionView>
{
    [Listener(TransitionEvent.REQUEST_TRANSITION)]
    private void OnRequestTransition()
    {
        view.FadeOut(1.0f);
    }
}
