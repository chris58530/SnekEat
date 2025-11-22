using Core.MVC;
using UnityEngine;
using Zenject;
public class EntryLoadingViewMediator : BaseMediator<EntryLoadingView>
{
    [Listener(NetworkEvent.ON_NETWORK_CONNECTING)]
    public void OnEntryLoadingStart(float progress)
    {
        view.ShowLoading(progress);
    }

    [Listener(NetworkEvent.ON_NETWORK_CONNECTED)]
    public void OnNetworkConnected()
    {
        view.HideLoading();
    }

}