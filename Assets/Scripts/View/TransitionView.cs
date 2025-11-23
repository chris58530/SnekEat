using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Core.MVC;
using System;

public class TransitionView : BaseView<TransitionViewMediator>
{
    [SerializeField] private GameObject root;
    [SerializeField] private Image overlayImage;

    public void FadeOut(float duration, Action completeCallback = null)
    {
        root.SetActive(true);
        DOTween.To(() => overlayImage.color.a, x =>
        {
            var color = overlayImage.color;
            color.a = x;
            overlayImage.color = color;
        }, 1f, duration).OnComplete(() =>
        {
            completeCallback?.Invoke();
        });
    }

    public void FadeIn(float duration, Action completeCallback = null)
    {
        DOTween.To(() => overlayImage.color.a, x =>
        {
            var color = overlayImage.color;
            color.a = x;
            overlayImage.color = color;
        }, 0f, duration).OnComplete(() =>
        {
            root.SetActive(false);
            completeCallback?.Invoke();
        });
    }
}
