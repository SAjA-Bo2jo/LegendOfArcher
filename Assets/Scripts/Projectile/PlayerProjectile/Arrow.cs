using UnityEngine;

public class Arrow : Projectile
{
    private float damage;
    private float critRate;
    private float speed;
    private Rigidbody2D rb;

    public AudioClip attackSoundClip;

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

        // 화살 발사 전 상태 초기화
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    // LaunchTowards 메서드는 이미 '방향' 벡터를 받습니다.
    public void LaunchTowards(Vector3 direction) // 매개변수 이름을 'direction'으로 변경하여 의미를 명확히 합니다.
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>(); // Awake에서 실패했을 경우를 대비해 한 번 더 확인
        if (rb != null)
        {
            // 이미 정규화된 방향 벡터를 사용하므로 다시 .normalized를 쓰지 않습니다.
            rb.velocity = direction * speed; // Arrow에 속도 부여

            // 화살이 진행 방향에 따라 회전하도록 합니다.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌 대상이 'Enemy' 또는 'Wall' 태그를 가지고 있는지 확인
        if (other.CompareTag("Enemy"))
        {
            bool isCrit = Random.value * 100f < critRate;
            float finalDamage = isCrit ? damage * 2f : damage;
            EnemyController enemyController = other.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.GetDamage(finalDamage);
            }

            if (attackSoundClip != null)
            {
                SoundManager.Instance.PlaySoundEffect(attackSoundClip, Vector3.zero);
            }

            // 충돌 후 화살을 풀로 반환
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.isKinematic = true; // 풀로 돌아갈 때 정지 상태 유지
            ObjectPoolManager.Instance.Return("Arrow", gameObject);
        }
        if (other.CompareTag("Wall"))
        {
            // 벽에 닿았을 때도 풀로 반환
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.isKinematic = true; // 풀로 돌아갈 때 정지 상태 유지
            ObjectPoolManager.Instance.Return("Arrow", gameObject);
        }
        if (other.CompareTag("LowObject"))
        {
            // 낮은 오브젝트와 충돌 시
            return;
        }
    }
}
