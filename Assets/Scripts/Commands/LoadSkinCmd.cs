using Core.MVC;
using UnityEngine;
using Zenject;

public class LoadSkinCmd : ICommand
{
    [SerializeField] private SnekkiesAssetSetting snekkiesAssetSetting;
    [Inject] private SkinProxy skinProxy;
    [Inject] private GameProxy gameProxy;
    [SerializeField] private int defaultSkinId = 11111;
    public override void Execute(MonoBehaviour mono)
    {
        int skinId = gameProxy.selectNFTid;
        if (skinId <= 0)
        {
            skinId = defaultSkinId;
        }
        SnekkiesAsset skinAsset = snekkiesAssetSetting.GetSnekkiesAsset(skinId);
        skinProxy.SetCurrentSkin(skinId, skinAsset);
    }

    [Listener(SkinEvent.RUNNER_SKIN_SETUP_COMPLETE)]
    private void OnRunnerSkinSetupComplete()
    {
        SetComplete();
    }
}



