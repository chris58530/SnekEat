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

    }

    public void FadeOut(float duration)
    {
        Debug.Log("FadeOut called");
    }

    public void FadeIn(float duration)
    {
        Debug.Log("FadeIn called");
    }
}
