using Core.MVC;
using UnityEngine;
using Zenject;

public class SnekControlViewMediator : BaseMediator<SnekControlView>
{
    [Inject] private SkinProxy skinProxy;

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

}
