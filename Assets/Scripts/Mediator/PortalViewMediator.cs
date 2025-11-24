using Core.MVC;
using UnityEngine;

public class PortalViewMediator : BaseMediator<PortalView>
{
    [Listener(PortalEvent.ON_PORTAL_SPAWN)]
    public void OnPortalSpawn()
    {
        view.SpawnPortal();
    }


}
