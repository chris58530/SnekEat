using Core.MVC;
using UnityEngine;

public class BossProxy : IProxy
{
    private string currentBossFeature;
    public void SetCurrentBossFeature(string bossFeature)
    {
        currentBossFeature = bossFeature;
        listener.BroadCast(BossEvent.REQUEST_START_FEATURE + "_" + bossFeature);
    }
}
