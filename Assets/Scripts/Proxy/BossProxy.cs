using Core.MVC;
using UnityEngine;

public class BossProxy : IProxy
{
    private string currentBossFeature;
    public void SetCurrentBossFeature(string bossFeature)
    {
        currentBossFeature = bossFeature;
        listener.BroadCast(BossEvent.REQUEST_FEATURE + "_" + bossFeature);
    }

    public Transform SnekTransform { get; private set; }

    public void SetSnekTransform(Transform snekTransform)
    {
        SnekTransform = snekTransform;
    }
}
