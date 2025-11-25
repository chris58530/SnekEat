using UnityEngine;
using DG.Tweening;

public class GreenBossObjectView : MonoBehaviour, IHittable
{
    public Transform firePoint;
    public GameObject projectilePrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void FireProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
    }

    public void OnHit(int damage)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.transform.DOShakePosition(0.5f, 0.5f, 10, 90, false, true);
            spriteRenderer.DOColor(Color.red, 0.1f).OnComplete(() => spriteRenderer.DOColor(Color.white, 0.1f));
        }
        else
        {
            transform.DOShakePosition(0.5f, 0.5f, 10, 90, false, true);
        }
    }
}