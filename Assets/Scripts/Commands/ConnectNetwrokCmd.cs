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
        DOVirtual.DelayedCall(delayTime, () =>
        {
            SetComplete();
        });
    }
}
