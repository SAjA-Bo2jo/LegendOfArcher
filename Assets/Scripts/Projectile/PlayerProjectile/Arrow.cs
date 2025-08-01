using UnityEngine;

public class Arrow : Projectile
{
    private float damage;
    private float critRate;
    private float speed;
    private Rigidbody2D rb;

    // Awake()에서 Rigidbody2D 가져오기 (Projectile 클래스에 이미 있다면 중복 안 해도 됨)
    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Arrow Prefab에 Rigidbody2D 컴포넌트가 없습니다!", this);
        }
    }

    public void Setup(float damage, float size, float critRate, float speed)
    {
        this.damage = damage;
        this.critRate = critRate;
        this.speed = speed;
        transform.localScale = Vector3.one * size;
    }

    // LaunchTowards 메서드는 이미 '방향' 벡터를 받습니다.
    public void LaunchTowards(Vector3 direction) // 매개변수 이름을 'direction'으로 변경하여 의미를 명확히 합니다.
    {
        // targetPosition = targetPos; // 이 줄은 제거합니다. targetPos는 이제 direction입니다.
        if (rb == null) rb = GetComponent<Rigidbody2D>(); // Awake에서 가져오지 못했을 경우 대비한 안전 장치

        // 이미 정규화된 방향 벡터를 사용하므로 다시 정규화할 필요는 없지만, 안전을 위해 .normalized를 붙여도 무방합니다.
        rb.velocity = direction.normalized * speed;

        // 화살의 시각적인 회전을 이동 방향에 맞춥니다.
        // Bow.cs에서 이미 생성 시 회전을 맞춰줬지만, 한 번 더 확실하게 해주는 것도 좋습니다.
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
            Debug.Log("데미지 계산을 실행합니다");
            Destroy(gameObject);
            
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.isKinematic = true; // 비활성화 상태에서는 물리적 영향X
            ObjectPoolManager.Instance.Return("Arrow", gameObject);
        }
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
            
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.isKinematic = true; // 비활성화 상태에서는 물리적 영향X
            ObjectPoolManager.Instance.Return("Arrow", gameObject);
        }
    }
}
