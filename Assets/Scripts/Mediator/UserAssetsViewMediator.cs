using System.Collections.Generic;
using System.Diagnostics;
using Core.MVC;
using Zenject;

public class UserAssetsViewMediator : BaseMediator<UserAssetsView>
{
    [Inject] private WalletProxy walletProxy;
    public void OnRequestFetchWalletData(string walletAddress)
    {
        listener.BroadCast(ConnectWalletEvent.REQUEST_FETCH_WALLET_DATA, walletAddress);
    }

    [Listener(ConnectWalletEvent.ON_ADA_BALANCE_UPDATED)]
    public void OnAdaBalanceUpdated()
    {
        float balance = walletProxy.adaBalance;
        view.UpdateAdaBalance(balance);
    }

    [Listener(ConnectWalletEvent.ON_SNEK_UPDATED)]
    public void OnSnekUpdated()
    {
        long balance = walletProxy.snekBalance;
        view.UpdateSnekBalance(balance);
    }

    [Listener(ConnectWalletEvent.ON_NFTS_UPDATED)]
    public void OnNftsUpdated()
    {
        view.UpdateNftCount(walletProxy.snekOwnedIds, walletProxy.snekOwnedIds.Length);
    }
}