using System.Collections.Generic;
using System.Diagnostics;
using Core.MVC;
using Zenject;

public class UserAssetsViewMediator : BaseMediator<UserAssetsView>
{
    public void OnRequestFetchWalletData(string walletAddress)
    {
        listener.BroadCast(ConnectWalletEvent.REQUEST_FETCH_WALLET_DATA, walletAddress);
    }

    [Listener(ConnectWalletEvent.ON_ADA_BALANCE_UPDATED)]
    public void OnAdaBalanceUpdated(float balance)
    {
        if (view != null)
        {
            view.UpdateAdaBalance(balance);
        }
    }

    [Listener(ConnectWalletEvent.ON_SNEK_UPDATED)]
    public void OnSnekUpdated(float balance)
    {
        if (view != null)
        {
            view.UpdateSnekBalance(balance);
        }
    }

    [Listener(ConnectWalletEvent.ON_NFTS_UPDATED)]
    public void OnNftsUpdated(List<string> nfts)
    {
        if (view != null)
        {
            view.UpdateNftCount(nfts != null ? nfts.Count : 0);
        }
    }
}
