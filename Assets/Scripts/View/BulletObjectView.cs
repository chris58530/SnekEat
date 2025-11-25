using UnityEngine;

public class BulletObjectView : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float speed = 5f;
    private Vector3 direction;
    private void Start()
    {
        Destroy(this.gameObject, lifeTime);
    }
    public void Initialize(Vector3 dir)
    {
        direction = dir.normalized;
    }

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore collision with the shooter (Snake) or other bullets if needed
        if (collision.gameObject.CompareTag("Player")) return;

        if (collision.gameObject.TryGetComponent<IHittable>(out var damageable))
        {
            damageable.OnHit(1);
            Destroy(this.gameObject);
        }
        // Only destroy if it hits something relevant, or maybe a wall?
        // For now, let's not destroy immediately on ANY trigger, only on hittables or obstacles.
    }
}
