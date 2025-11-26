using Core.MVC;
using UnityEngine;
using DG.Tweening;

public class CameraView : BaseView<CameraViewMediator>
{
    [SerializeField] private Camera cam;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    private Transform target;
    private float camHeight;
    private float camWidth;

    [SerializeField] private float limitX;
    [SerializeField] private float limitY;
    protected override void Awake()
    {
        base.Awake();
        if (cam == null)
        {
            cam = GetComponent<Camera>();
            if (cam == null)
            {
                cam = Camera.main;
            }
        }
        UpdateCameraBounds();
    }

    private void UpdateCameraBounds()
    {
        if (cam != null)
        {
            camHeight = cam.orthographicSize;
            camWidth = camHeight * cam.aspect;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void LateUpdate()
    {
        if (cam != null)
        {
            camHeight = cam.orthographicSize;
            camWidth = camHeight * cam.aspect;
        }

        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        // Clamp position within bounds based on absolute X/Y limits
        // Assuming generateAreaMax.x is the positive limit and generateAreaMin.x is the negative limit (or symmetric)
        // If the area is symmetric like (-75, 75), we can just use the positive value as the limit.

        float xLimit = limitX != 0 ? Mathf.Abs(limitX) : GameMathService.generateAreaMax.x;
        float yLimit = limitY != 0 ? Mathf.Abs(limitY) : GameMathService.generateAreaMax.y;

        float minX = -xLimit + camWidth;
        float maxX = xLimit - camWidth;
        float minY = -yLimit + camHeight;
        float maxY = yLimit - camHeight;

        // If the area is smaller than the camera view, center it
        if (minX > maxX)
        {
            desiredPosition.x = 0f;
        }
        else
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        }

        if (minY > maxY)
        {
            desiredPosition.y = 0f;
        }
        else
        {
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    private Tween focusTween;

    public void FocusTemporary(CameraFocusSetting setting)
    {
        float originalSize = cam.orthographicSize;
        float originalOffsetZ = offset.z;
        Transform originalTarget = target;

        focusTween?.Kill();
        Sequence seq = DOTween.Sequence();

        // Step 1: Transition In
        seq.AppendCallback(() =>
        {
            target = setting.Target;
        });

        if (setting.InDuration > 0)
        {
            seq.Append(cam.DOOrthoSize(setting.FocusSize, setting.InDuration).SetEase(Ease.InOutSine));
            if (setting.FocusZ.HasValue)
            {
                seq.Join(DOTween.To(() => offset.z, x => offset.z = x, setting.FocusZ.Value, setting.InDuration).SetEase(Ease.InOutSine));
            }
        }
        else
        {
            seq.AppendCallback(() =>
            {
                cam.orthographicSize = setting.FocusSize;
                if (setting.FocusZ.HasValue)
                {
                    offset.z = setting.FocusZ.Value;
                }
            });
        }

        // Step 2: Stay
        seq.AppendInterval(setting.StayDuration);

        // Step 3: Transition Out
        seq.Append(cam.DOOrthoSize(originalSize, setting.OutDuration).SetEase(Ease.InOutSine));

        if (setting.FocusZ.HasValue)
        {
            seq.Join(DOTween.To(() => offset.z, x => offset.z = x, originalOffsetZ, setting.OutDuration).SetEase(Ease.InOutSine));
        }

        seq.AppendCallback(() =>
        {
            target = originalTarget;
            offset.z = originalOffsetZ;
        });

        focusTween = seq;
    }

    public void Shake()
    {
        cam.transform.DOShakePosition(0.5f, 1f, 10, 90, false, true);
    }
}
