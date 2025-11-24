using Core.MVC;
using UnityEngine;
using Zenject;

public class BossCmd : ICommand
{
    [Inject] private BossProxy bossProxy;
    [SerializeField] private string[] bossFeatureList;
    [SerializeField] private string currentBoss;
    public override void Execute(MonoBehaviour mono)
    {
        isLazy = true;
    }
    [Listener(BossEvent.REQUEST_START_FEATURE)]
    private void OnRequestStartFeature()
    {
        currentBoss = bossFeatureList[Random.Range(0, bossFeatureList.Length)];
        bossProxy.SetCurrentBossFeature(currentBoss);
        SetComplete();
    }

}
