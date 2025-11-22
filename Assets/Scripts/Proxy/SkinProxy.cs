using UnityEngine;
using Core.MVC;

public class SkinProxy : IProxy
{
    public int currentSkinId;
    public SnekkiesAsset currentSkinAsset;
    public void SetCurrentSkin(int skinId, SnekkiesAsset skinAsset)
    {
        currentSkinId = skinId;
        currentSkinAsset = skinAsset;
        listener.BroadCast(SkinEvent.ON_SETUP_SKIN);
    }
}