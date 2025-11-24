using Core.MVC;
using UnityEngine;
using Zenject;

public class SnekControlViewMediator : BaseMediator<SnekControlView>
{
    [Inject] private SkinProxy skinProxy;
    [Inject] private TransitionProxy transitionProxy;

    [Listener(SkinEvent.ON_SETUP_SKIN)]
    private void OnSetupSkin()
    {
        var skinAsset = skinProxy.currentSkinAsset;
        view.SetupRunnerSkin(skinAsset);
    }

    public void OnRunnerSkinSetupComplete()
    {
        listener.BroadCast(SkinEvent.RUNNER_SKIN_SETUP_COMPLETE);
    }

    public void OnEnterPortal()
    {
        transitionProxy.RequestTransition(() =>
        {
            listener.BroadCast(BossEvent.REQUEST_START_FEATURE);

            //變換場景 等場景更新完畢後再Complete Transition
            transitionProxy.TransitionComplete(() =>
            {
                view.EnableMove(true);
            });
        });
    }
}
