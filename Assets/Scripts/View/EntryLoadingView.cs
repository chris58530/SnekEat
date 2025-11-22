using Core.MVC;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class EntryLoadingView : BaseView<EntryLoadingViewMediator>
{
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private Image loadingBarFill;

    public void ShowLoading(float progress)
    {
        Debug.Log("ShowLoading called with progress: " + progress);
        loadingBarFill.fillAmount = progress;
    }

    public void HideLoading()
    {
        loadingIndicator.SetActive(false);
    }
}