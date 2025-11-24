using Core.MVC;
using UnityEngine;
using Zenject;

public class GreenBossViewMediator : BaseMediator<GreenBossView>
{
    [Inject] private BossProxy bossProxy;
    [Listener(BossEvent.REQUEST_START_FEATURE_GREEN)]
    private void OnRequestStartFeatureGreen()
    {
        view.StartFeature();
    }

}
