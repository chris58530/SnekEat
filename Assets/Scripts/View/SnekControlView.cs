using Core.MVC;
using UnityEngine;

public class SnekControlView : BaseView<SnekControlViewMediator>
{
    [SerializeField] private SnekRunner snekRunner;

    public void SetupRunnerSkin(SnekkiesAsset skinAsset)
    {
        snekRunner.Setup(skinAsset, () =>
        {
            mediator.OnRunnerSkinSetupComplete();
        });
    }

}
