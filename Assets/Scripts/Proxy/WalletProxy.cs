using UnityEngine;
using Core.MVC;

public class WalletProxy : IProxy
{
    public float adaBalance; // 改成 float 以保留小數
    public long snekBalance; // 改成 long 避免溢位
    public int[] snekOwnedIds;

    public void SetADABalances(float ada)
    {
        adaBalance = ada;
        listener.BroadCast(ConnectWalletEvent.ON_ADA_BALANCE_UPDATED);
    }

    public void SetSnekBalances(long snek)
    {
        snekBalance = snek;
        listener.BroadCast(ConnectWalletEvent.ON_SNEK_UPDATED);
    }

    public void SetSnekNFT(int[] ids)
    {
        snekOwnedIds = ids;
        listener.BroadCast(ConnectWalletEvent.ON_NFTS_UPDATED);
    }
}