using Core.MVC;
using UnityEngine;

public class PortalViewMediator : BaseMediator<PortalView>
{
    [Listener(PortalEvent.ON_PORTAL_SPAWN)]
    public void OnPortalSpawn()
    {
        view.SpawnPortal();
    }

    [Listener(PortalEvent.ON_PORTAL_HIT)]
    public void OnPortalHit(Transform snekTransform, Vector3 portalPosition)
    {
        view.ShowHoleMask(true, snekTransform, portalPosition);
    }
}
