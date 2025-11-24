using Core.MVC;
using UnityEngine;

public class GreenBossView : BaseView<GreenBossViewMediator>
{
    [SerializeField] private GameObject root;
    protected override void Awake()
    {
        base.Awake();
        RestView();
    }
    public void RestView()
    {
        root.SetActive(false);
    }

    public void StartFeature()
    {
        root.SetActive(true);
    }

}
