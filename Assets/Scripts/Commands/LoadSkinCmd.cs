using Core.MVC;
using UnityEngine;
using Zenject;

public class LoadSkinCmd : ICommand
{
    [SerializeField] private SnekkiesAssetSetting snekkiesAssetSetting;
    [Inject] private SkinProxy skinProxy;
    [Inject] private GameProxy gameProxy;
    public override void Execute(MonoBehaviour mono)
    {
        int skinId = gameProxy.selectNFTid;
        SnekkiesAsset skinAsset = snekkiesAssetSetting.GetSnekkiesAsset(skinId);
        skinProxy.SetCurrentSkin(skinId, skinAsset);
    }

    [Listener(SkinEvent.RUNNER_SKIN_SETUP_COMPLETE)]
    private void OnRunnerSkinSetupComplete()
    {
        SetComplete();
    }
}



