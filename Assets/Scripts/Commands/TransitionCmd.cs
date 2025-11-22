using UnityEngine;
using Zenject;
using Core.MVC;
using DG.Tweening;
using System.Collections;

[CreateAssetMenu(fileName = "TransitionCmd", menuName = "SnekRun/Commands/TransitionCmd")]
public class TransitionCmd : ICommand
{
    public override void Execute(MonoBehaviour mono)
    {
        isLazy = true;
    }
}
