using System;
using Core.MVC;
using DG.Tweening;
using UnityEngine;


public class ConnectNetwrokCmd : ICommand
{
    [SerializeField] private float delayTime = 2f;

    public override void Execute(MonoBehaviour mono)
    {
        this.ProccessConnect();
    }

    public void ProccessConnect()
    {
        float progress = 0f;
        DOTween.To(() => progress, x => progress = x, 1f, delayTime).OnUpdate(() =>
        {
            listener.BroadCast(NetworkEvent.ON_NETWORK_CONNECTING, progress);

        }).OnComplete(() =>
        {
            listener.BroadCast(NetworkEvent.ON_NETWORK_CONNECTED);
            SetComplete();

        });
    }
}
