using Core.MVC;
using UnityEngine;

public class PortalViewMediator : BaseMediator<PortalView>
{
    [Listener(PortalEvent.ON_PORTAL_ENTERED)]
    public void OnPortalEntered()
    {
        view.SpawnPortal();
    }
}
