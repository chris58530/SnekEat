using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.UIElements;
using Core.MVC;

public class TransitionView : BaseView<TransitionViewMediator>
{
    [SerializeField] private UnityEngine.UI.Image overlayImage;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        // 確保一開始是透明的，且不阻擋射線
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;


    }

    public void FadeOut(float duration)
    {
        Debug.Log("FadeOut called");
    }

    public void FadeIn(float duration)
    {
        canvasGroup.DOFade(0f, duration).OnComplete(() =>
        {
            canvasGroup.blocksRaycasts = false;
        });
    }
}
