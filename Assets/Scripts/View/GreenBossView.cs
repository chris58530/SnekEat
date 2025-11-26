using System.Collections;
using Core.MVC;
using UnityEngine;

public class GreenBossView : BaseView<GreenBossViewMediator>
{
    [SerializeField] private GameObject root;
    [SerializeField] private BulletObjectView bulletObjectView;
    [SerializeField] private GreenBossObjectView greenBossObjectView;
    [SerializeField] private int health;
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
        greenBossObjectView.onShoot = OnBossShoot;
    }

    private void OnBossShoot(Vector3 position, Quaternion rotation, BulletTarget target, float size)
    {
        if (bulletObjectView != null)
        {
            var bullet = Instantiate(bulletObjectView, position, rotation);
            bullet.Initialize(bullet.transform.up, target, size);
        }
    }

    public void SetTarget(Transform target)
    {
        greenBossObjectView.SetTarget(target);
    }
}
