using UnityEngine;

public class Arrow : Projectile
{
    private float damage;
    private float critRate;
    private float speed;
    private Rigidbody2D rb;

    // Awake()에서 Rigidbody2D 가져오기
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

        // 풀에서 재활용될 때를 위해 Rigidbody2D를 활성화합니다.
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    // LaunchTowards 메서드는 이미 '방향' 벡터를 받습니다.
    public void LaunchTowards(Vector3 direction) // 매개변수 이름을 'direction'으로 변경하여 의미를 명확히 합니다.
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>(); // Awake에서 가져오지 못했을 경우 대비한 안전 장치
        if (rb != null)
        {
            // 이미 정규화된 방향 벡터를 사용하므로 다시 정규화할 필요는 없지만, 안전을 위해 .normalized를 붙여도 무방합니다.
            rb.velocity = direction.normalized * speed;
            // 화살의 시각적인 회전을 이동 방향에 맞춥니다.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌 대상이 'Enemy' 또는 'Wall' 태그를 가지고 있는지 확인
        if (other.CompareTag("Enemy") || other.CompareTag("Wall"))
        {
            // 적에게 닿았을 경우에만 데미지 처리
            if (other.CompareTag("Enemy"))
            {
                var enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    bool isCrit = Random.value * 100f < critRate;
                    float finalDamage = isCrit ? damage * 2f : damage;
                    // 적에게 데미지를 주는 로직을 여기에 작성하세요.
                }
            }

            // 충돌 후 오브젝트를 파괴하는 대신 풀로 반환
            // Rigidbody2D의 속도와 상태를 초기화하여 재활용 시 문제가 없도록 합니다.
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                rb.isKinematic = true; // 비활성화 상태에서는 물리적 영향X
            }

            // 오브젝트 풀 매니저를 통해 풀로 되돌려놓습니다.
            ObjectPoolManager.Instance.Return("Arrow", gameObject);
        }
    }
}
