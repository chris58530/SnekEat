using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;

public class GreenBossObjectView : MonoBehaviour, IHittable
{
    public Transform firePoint;
    public BulletObjectView projectilePrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector3 originalPosition;

    private Transform targetTransform;

    private void Start()
    {
        originalPosition = transform.position;
        StartCoroutine(BossBehavior());
    }

    public void OnHit(int damage)
    {
        transform.DOKill(complete: true);

        if (spriteRenderer != null)
        {
            if (spriteRenderer.transform != transform)
            {
                spriteRenderer.transform.DOShakePosition(0.5f, new Vector3(0.5f, 0.5f, 0), 10, 90, false, true);
            }
            spriteRenderer.DOColor(Color.red, 0.1f).OnComplete(() => spriteRenderer.DOColor(Color.white, 0.1f));
        }

        transform.DOShakePosition(0.5f, new Vector3(0.5f, 0.5f, 0), 10, 90, false, true)
                 .OnComplete(() => transform.DOMove(originalPosition, 0.2f));
    }
    private IEnumerator BossBehavior()
    {
        while (true)
        {
            yield return StartCoroutine(IdleState());

            int rand = UnityEngine.Random.Range(0, 3);
            switch (rand)
            {
                case 0:
                    yield return StartCoroutine(ShootSingleState());
                    break;
                case 1:
                    yield return StartCoroutine(ShootMultipleState());
                    break;
                case 2:
                    yield return StartCoroutine(ShootTripleState());
                    break;
            }
        }
    }

    private IEnumerator IdleState()
    {
        yield return new WaitForSeconds(2f);
    }

    private IEnumerator ShootSingleState()
    {
        // Single shot, 5x size
        FireProjectile(BulletTarget.Player, 5f);
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator ShootMultipleState()
    {
        float startAngle = -30f;
        float endAngle = 30f;
        int count = 5;
        float step = (endAngle - startAngle) / (count - 1);

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + step * i;
            FireProjectile(BulletTarget.Player, 1f, angle);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator ShootTripleState()
    {
        for (int i = 0; i < 3; i++)
        {
            FireProjectile(BulletTarget.Player, 2f);
            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(1f);
    }

    public Action<Vector3, Quaternion, BulletTarget, float> onShoot;

    private void FireProjectile(BulletTarget target, float size, float angleOffset = 0f)
    {
        if (firePoint != null)
        {
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, 0, angleOffset);
            onShoot?.Invoke(firePoint.position, rotation, target, size);
        }
    }

    public void SetTarget(Transform target)
    {
        targetTransform = target;
    }

    private void Update()
    {
        if (targetTransform != null)
        {
            // Rotate to face the target
            Vector3 direction = targetTransform.position - firePoint.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            firePoint.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}