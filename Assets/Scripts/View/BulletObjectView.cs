using UnityEngine;

public enum BulletTarget
{
    Player,
    Boss
}

public class BulletObjectView : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float speed = 5f;
    private Vector3 direction;
    private BulletTarget targetType;

    private void Start()
    {
        Destroy(this.gameObject, lifeTime);
    }

    public void Initialize(Vector3 dir, BulletTarget target, float sizeMultiplier = 1f)
    {
        direction = dir.normalized;
        targetType = target;
        transform.localScale *= sizeMultiplier;
    }

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    public void OnHitTarget(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<IHittable>(out var damageable))
        {
            if (targetType == BulletTarget.Player)
            {
                // If target is Player, we only hit if the object is Player
                // Assuming Player implements IHittable (SnekObjectView does now)
                if (collision.gameObject.CompareTag("Player") || collision.GetComponent<SnekObjectView>() != null)
                {
                    damageable.OnHit(1);
                    Destroy(this.gameObject);
                }
            }
            else if (targetType == BulletTarget.Boss)
            {
                // If target is Boss, we only hit if the object is NOT Player (assuming Boss is not tagged Player)
                // And we avoid hitting the shooter (Player)
                if (!collision.gameObject.CompareTag("Player") && collision.GetComponent<SnekObjectView>() == null)
                {
                    damageable.OnHit(1);
                    Destroy(this.gameObject);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnHitTarget(collision);
    }
}
