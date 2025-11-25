using Core.MVC;
using UnityEngine;
using Zenject;

public class CameraViewMediator : BaseMediator<CameraView>
{
    [Listener(CameraEvent.ON_SET_CAMERA_TARGET)]
    public void OnSetCameraTarget(Transform target)
    {
        view.SetTarget(target);
    }

    [Listener(CameraEvent.ON_FOCUS_TEMPORARY)]
    public void OnFocusTemporary(CameraFocusSetting setting)
    {
        view.FocusTemporary(setting);
    }
}
