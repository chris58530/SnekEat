using UnityEngine;
using Core.MVC;

public class ConnectWalletCmd : ICommand
{
    public override void Execute(MonoBehaviour mono)
    {
        isLazy = true;
    }
}
