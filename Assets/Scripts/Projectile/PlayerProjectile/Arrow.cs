using UnityEngine;

public class Arrow : Projectile
{
    private float damage;
    private float critRate;
    private float speed;
    private Rigidbody2D rb;

    // Awake()���� Rigidbody2D ��������
    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(float damage, float size, float critRate, float speed)
    {
        this.damage = damage;
        this.critRate = critRate;
        this.speed = speed;
        transform.localScale = Vector3.one * size;

        // Ǯ���� ��Ȱ��� ���� ���� Rigidbody2D�� Ȱ��ȭ�մϴ�.
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    // LaunchTowards �޼���� �̹� '����' ���͸� �޽��ϴ�.
    public void LaunchTowards(Vector3 direction) // �Ű����� �̸��� 'direction'���� �����Ͽ� �ǹ̸� ��Ȯ�� �մϴ�.
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>(); // Awake���� �������� ������ ��� ����� ���� ��ġ
        if (rb != null)
        {
            // �̹� ����ȭ�� ���� ���͸� ����ϹǷ� �ٽ� ����ȭ�� �ʿ�� ������, ������ ���� .normalized�� �ٿ��� �����մϴ�.
            rb.velocity = direction.normalized * speed;
            // ȭ���� �ð����� ȸ���� �̵� ���⿡ ����ϴ�.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �浹 ����� 'Enemy' �Ǵ� 'Wall' �±׸� ������ �ִ��� Ȯ��
        if (other.CompareTag("Enemy"))
        {
            bool isCrit = Random.value * 100f < critRate;
            float finalDamage = isCrit ? damage * 2f : damage;
            other.GetComponent<EnemyController>().GetDamage(finalDamage);
            
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.isKinematic = true; // ��Ȱ��ȭ ���¿����� ������ ����X
            ObjectPoolManager.Instance.Return("Arrow", gameObject);
        }
        if (other.CompareTag("Wall"))
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.isKinematic = true; // ��Ȱ��ȭ ���¿����� ������ ����X
            ObjectPoolManager.Instance.Return("Arrow", gameObject);
        }
    }
}
