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

    [Listener(PortalEvent.ON_PORTAL_DESPAWN)]
    public void OnPortalDespawn()
    {
        view.ResetView();
    }

    [Listener(PortalEvent.ON_SPAWN_EXIT_PORTAL)]
    public void OnSpawnExitPortal(Vector3 position)
    {
        view.SpawnExitPortal(position);
    }
}
