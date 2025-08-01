using UnityEngine;

public class Arrow : Projectile
{
    private float damage;
    private float critRate;
    private float speed;
    private Rigidbody2D rb;

    // Awake()���� Rigidbody2D �������� (Projectile Ŭ������ �̹� �ִٸ� �ߺ� �� �ص� ��)
    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Arrow Prefab�� Rigidbody2D ������Ʈ�� �����ϴ�!", this);
        }
    }

    public void Setup(float damage, float size, float critRate, float speed)
    {
        this.damage = damage;
        this.critRate = critRate;
        this.speed = speed;
        transform.localScale = Vector3.one * size;
    }

    // LaunchTowards �޼���� �̹� '����' ���͸� �޽��ϴ�.
    public void LaunchTowards(Vector3 direction) // �Ű����� �̸��� 'direction'���� �����Ͽ� �ǹ̸� ��Ȯ�� �մϴ�.
    {
        // targetPosition = targetPos; // �� ���� �����մϴ�. targetPos�� ���� direction�Դϴ�.
        if (rb == null) rb = GetComponent<Rigidbody2D>(); // Awake���� �������� ������ ��� ����� ���� ��ġ

        // �̹� ����ȭ�� ���� ���͸� ����ϹǷ� �ٽ� ����ȭ�� �ʿ�� ������, ������ ���� .normalized�� �ٿ��� �����մϴ�.
        rb.velocity = direction.normalized * speed;

        // ȭ���� �ð����� ȸ���� �̵� ���⿡ ����ϴ�.
        // Bow.cs���� �̹� ���� �� ȸ���� ����������, �� �� �� Ȯ���ϰ� ���ִ� �͵� �����ϴ�.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            bool isCrit = Random.value * 100f < critRate;
            float finalDamage = isCrit ? damage * 2f : damage;
            other.GetComponent<EnemyController>().GetDamage(finalDamage);
            Debug.Log("������ ����� �����մϴ�");
            Destroy(gameObject);
            
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.isKinematic = true; // ��Ȱ��ȭ ���¿����� ������ ����X
            ObjectPoolManager.Instance.Return("Arrow", gameObject);
        }
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
            
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.isKinematic = true; // ��Ȱ��ȭ ���¿����� ������ ����X
            ObjectPoolManager.Instance.Return("Arrow", gameObject);
        }
    }
}
