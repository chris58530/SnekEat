using Core.MVC;
using UnityEngine;
using Zenject;

public class GreenBossViewMediator : BaseMediator<GreenBossView>
{
    [Inject] private BossProxy bossProxy;
    [Listener(BossEvent.REQUEST_FEATURE_GREEN)]
    private void OnRequestStartFeatureGreen()
    {
        view.SetTarget(bossProxy.SnekTransform);
    }

    [Listener(PortalEvent.ON_PORTAL_EXIT_COMPLETE)]
    private void OnStartFeature()
    {
        view.StartFeature();
    }
}
