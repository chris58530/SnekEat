using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TransitionOverlay : MonoBehaviour
{
    [SerializeField] private Image overlayImage;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (overlayImage == null) overlayImage = GetComponentInChildren<Image>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        // 確保一開始是透明的，且不阻擋射線
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public void FadeOut(float duration)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(1f, duration);
    }

    public void FadeIn(float duration)
    {
        canvasGroup.DOFade(0f, duration).OnComplete(() =>
        {
            canvasGroup.blocksRaycasts = false;
        });
    }
}
