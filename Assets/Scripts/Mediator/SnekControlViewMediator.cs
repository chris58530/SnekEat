using Core.MVC;
using UnityEngine;
using Zenject;

public class SnekControlViewMediator : BaseMediator<SnekControlView>
{
    [Inject] private SkinProxy skinProxy;
    [Inject] private TransitionProxy transitionProxy;
    [Inject] private BossProxy bossProxy;

    [Listener(SkinEvent.ON_SETUP_SKIN)]
    private void OnSetupSkin()
    {
        var skinAsset = skinProxy.currentSkinAsset;
        view.SetupRunnerSkin(skinAsset);
    }

    public void OnRunnerSkinSetupComplete(Transform snekTransform)
    {
        bossProxy.SetSnekTransform(snekTransform);
        listener.BroadCast(SkinEvent.RUNNER_SKIN_SETUP_COMPLETE);
        listener.BroadCast(CameraEvent.ON_SET_CAMERA_TARGET, snekTransform);

        var focusSetting = new CameraFocusSetting
        {
            Target = snekTransform,
            FocusSize = 5f,
            InDuration = 0f,
            StayDuration = 6f,
            OutDuration = 0.5f
        };
        listener.BroadCast(CameraEvent.ON_FOCUS_TEMPORARY, focusSetting);
    }

    public void OnEnterPortal()
    {
        transitionProxy.RequestTransition(() =>
        {
            listener.BroadCast(BossEvent.REQUEST_START_FEATURE);

            //變換場景 等場景更新完畢後再Complete Transition
            transitionProxy.TransitionComplete(() =>
            {
                listener.BroadCast(PortalEvent.ON_PORTAL_DESPAWN);
                view.EnableMove(true);
            });
        });
    }

    public void OnStartEnterPortal(Transform snekTransform, Transform portalTransform)
    {
        listener.BroadCast(PortalEvent.ON_PORTAL_HIT, snekTransform, portalTransform.position);
    }

    [Listener(PlayerActionEvent.ON_PLAYER_SHOOT)]
    public void OnPlayerShoot()
    {
        view.OnShoot();
    }

    [Listener(PlayerActionEvent.ON_PLAYER_DASH)]
    public void OnPlayerDash()
    {
        view.OnDash();
    }

    public void OnSnekHit()
    {
        // Broadcast camera shake event
        // Assuming there is an event for camera shake, or we can reuse ON_FOCUS_TEMPORARY with shake parameters?
        // Or maybe create a new event?
        // The user said "讓相機震動".
        // I should check if there is a camera shake event.
        // If not, I might need to add one or use DOTween in CameraView directly via an event.
        // Let's check EventTable.cs first.
        // But I can't check it inside this tool call.
        // I'll assume I need to add a new event or use a generic one.
        // Let's add ON_CAMERA_SHAKE to CameraEvent.
        listener.BroadCast(CameraEvent.ON_CAMERA_SHAKE);
    }
}
