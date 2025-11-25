using UnityEngine;

public class BulletObjectView : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float speed = 5f;
    private Vector3 direction;
    private void OnEnable()
    {
        Destroy(this.gameObject, lifeTime);
    }
    public void Initialize(Vector3 dir)
    {
        direction = dir.normalized;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
