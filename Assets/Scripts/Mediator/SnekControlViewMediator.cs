using System;
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

    public void OnEnterPortal(Action enterCallback)
    {
        transitionProxy.RequestTransition(() =>
        {
            listener.BroadCast(BossEvent.REQUEST_FEATURE);

            //變換場景 等場景更新完畢後再Complete Transition
            transitionProxy.TransitionComplete(() =>
            {
                listener.BroadCast(PortalEvent.ON_PORTAL_DESPAWN);
                listener.BroadCast(PortalEvent.ON_SPAWN_EXIT_PORTAL, Vector3.zero);
                enterCallback?.Invoke();
            });
        });
    }

    public void OnExitPortalComplete()
    {
        listener.BroadCast(PortalEvent.ON_PORTAL_EXIT_COMPLETE);
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

    public void RequestCameraShake()
    {
        listener.BroadCast(CameraEvent.ON_CAMERA_SHAKE);
    }

    [Listener(DebugEvent.ON_TOGGLE_DEBUG_MODE)]
    public void OnDebugModeChanged(bool isDebug)
    {
        view.SetDebugMode(isDebug);
    }

    [Listener(DebugEvent.ON_ADD_LENGTH)]
    public void OnAddLength()
    {
        view.AddLengthDebug();
    }
}
