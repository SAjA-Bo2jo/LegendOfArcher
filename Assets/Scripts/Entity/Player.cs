using UnityEngine;

public class Player : Entity
{
    // ───────── 체력 관련 ─────────
    [SerializeField, Range(0f, 500f)]
    private float baseHealth = 100f;                     // 기준 체력 (배율 계산의 기준값)
    [SerializeField, Range(0f, 500f)]
    private float healthMultiplier = 100f;               // 체력 배율 (%) - 기본 100%

    [SerializeField, Range(0f, 500f)]
    private float health = 100f;                          // 현재 체력 (게임 중 변하는 값)

    [SerializeField, Range(0f, 100f)]
    private float healthRegen = 0f;                       // 초당 체력 재생량
    [SerializeField, Range(0f, 500f)]
    private float healingMultiplier = 100f;               // 체력 회복량 증폭 배율 (%)

    // 프로퍼티 (외부 접근 가능, set 가능)
    public float BaseHealth { get; set; }                 // 기준 체력
    public float HealthMultiplier { get; set; }           // 체력 배율 (%)

    // MaxHealth는 baseHealth * healthMultiplier / 100 으로 계산되므로 set을 안주고 get만 제공
    public float MaxHealth => BaseHealth * (HealthMultiplier / 100f);

    public float Health
    {
        get => health;
        set
        {
            // 현재 체력은 0 이상 MaxHealth 이하로 제한
            health = Mathf.Clamp(value, 0, MaxHealth);
            if (health <= 0)
            {
                Die(); // 체력 0이 되면 사망 처리
            }
        }
    }

    public float HealthRegen { get; set; }                // 초당 체력 재생량
    public float HealingMultiplier { get; set; }          // 회복량 증폭 배율 (%)

    // ───────── 방어력 관련 ─────────
    [SerializeField, Range(0f, 1000f)]
    private float baseDefense = 100f;                      // 기준 방어력
    [SerializeField, Range(0f, 500f)]
    private float defenseMultiplier = 100f;                // 방어력 배율 (%)
    [SerializeField, Range(0f, 1000f)]
    private float defense = 100f;                           // 현재 방어력

    public float BaseDefense { get; set; }                  // 기준 방어력
    public float DefenseMultiplier { get; set; }            // 방어력 배율 (%)
    public float MaxDefense => BaseDefense * (DefenseMultiplier / 100f); // 최대 방어력 계산식
    public float Defense
    {
        get => defense;
        set => defense = Mathf.Clamp(value, 0, MaxDefense); // 현재 방어력은 0~최대 사이로 제한
    }

    // ───────── 이동 속도 관련 ─────────
    [SerializeField, Range(0f, 20f)]
    private float baseSpeed = 5f;                           // 기준 이동속도
    [SerializeField, Range(0f, 500f)]
    private float speedMultiplier = 100f;                   // 이동속도 배율 (%)
    [SerializeField, Range(0f, 20f)]
    private float speed = 5f;                               // 현재 이동속도

    public float BaseSpeed { get; set; }                     // 기준 이동속도
    public float SpeedMultiplier { get; set; }               // 이동속도 배율 (%)
    public float MaxSpeed => BaseSpeed * (SpeedMultiplier / 100f); // 최대 이동속도 계산식
    public float Speed
    {
        get => speed;
        set => speed = Mathf.Clamp(value, 0, MaxSpeed);     // 현재 이동속도 0~최대 이동속도 제한
    }

    // ───────── 공격력 관련 ─────────
    [Header("공격 관련")]

    [SerializeField, Range(0f, 1000f)]
    private float baseAttackDamage = 100f;                   // 기준 공격력
    [SerializeField, Range(0f, 500f)]
    private float attackDamageMultiplier = 100f;             // 공격력 배율 (%)
    [SerializeField, Range(0f, 1000f)]
    private float attackDamage = 100f;                        // 현재 공격력

    public float BaseAttackDamage { get; set; }               // 기준 공격력
    public float AttackDamageMultiplier { get; set; }         // 공격력 배율 (%)
    public float MaxAttackDamage => BaseAttackDamage * (AttackDamageMultiplier / 100f); // 최대 공격력
    public float AttackDamage
    {
        get => attackDamage;
        set => attackDamage = Mathf.Clamp(value, 0, MaxAttackDamage); // 현재 공격력 0~최대 제한
    }

    // ───────── 공격 사거리 관련 ─────────
    [SerializeField, Range(0f, 50f)]
    private float baseAttackRange = 10f;                       // 기준 공격 사거리
    [SerializeField, Range(0f, 500f)]
    private float attackRangeMultiplier = 100f;                // 사거리 배율 (%)
    [SerializeField, Range(0f, 50f)]
    private float attackRange = 10f;                            // 현재 공격 사거리

    public float BaseAttackRange { get; set; }                  // 기준 공격 사거리
    public float AttackRangeMultiplier { get; set; }            // 사거리 배율 (%)
    public float MaxAttackRange => BaseAttackRange * (AttackRangeMultiplier / 100f); // 최대 공격 사거리
    public float AttackRange
    {
        get => attackRange;
        set => attackRange = Mathf.Clamp(value, 0, MaxAttackRange); // 현재 사거리 0~최대 제한
    }

    // ───────── 공격 속도 관련 ─────────
    [SerializeField, Range(0f, 50f)]
    private float baseAttackSpeed = 5f;                         // 기준 공격 속도
    [SerializeField, Range(0f, 5f)]
    private float attackSpeedMultiplier = 1f;                   // 공격 속도 배율 (%)
    [SerializeField, Range(0f, 50f)]
    private float attackSpeed = 5f;                             // 현재 공격 속도

    public float BaseAttackSpeed { get; set; }                   // 기준 공격 속도
    public float AttackSpeedMultiplier { get; set; }             // 공격 속도 배율 (%)
    public float MaxAttackSpeed => BaseAttackSpeed * (AttackSpeedMultiplier / 100f); // 최대 공격 속도
    public float AttackSpeed
    {
        get => attackSpeed;
        set => attackSpeed = Mathf.Clamp(value, 0, MaxAttackSpeed); // 현재 공격 속도 0~최대 제한
    }

    // ───────── 공격 크기 관련 ─────────
    [SerializeField, Range(0f, 10f)]
    private float baseAttackSize = 1f;                           // 기준 공격 크기
    [SerializeField, Range(0f, 500f)]
    private float attackSizeMultiplier = 100f;                   // 공격 크기 배율 (%)
    [SerializeField, Range(0f, 10f)]
    private float attackSize = 1f;                               // 현재 공격 크기

    public float BaseAttackSize { get; set; }                     // 기준 공격 크기
    public float AttackSizeMultiplier { get; set; }               // 공격 크기 배율 (%)
    public float MaxAttackSize => BaseAttackSize * (AttackSizeMultiplier / 100f); // 최대 공격 크기
    public float AttackSize
    {
        get => attackSize;
        set => attackSize = Mathf.Clamp(value, 0, MaxAttackSize); // 현재 공격 크기 0~최대 제한
    }

    // ───────── 투사체 수 관련 (배율 없음) ─────────
    [SerializeField, Range(0f, 100f)]
    private float baseNumberOfAttack = 1f;                        // 기준 투사체 수
    [SerializeField, Range(0f, 100f)]
    private float numberOfAttack = 1f;                            // 현재 투사체 수

    public float BaseNumberOfAttack { get; set; }                 // 기준 투사체 수
    public float NumberOfAttack
    {
        get => numberOfAttack;
        set => numberOfAttack = Mathf.Max(0, value);             // 0 이상 제한 (최대는 없음)
    }

    // ───────── 동시 발사 수 관련 (배율 없음) ─────────
    [SerializeField, Range(0f, 10f)]
    private float baseMultipleShot = 1f;                          // 기준 동시 발사 수
    [SerializeField, Range(0f, 10f)]
    private float multipleShot = 1f;                              // 현재 동시 발사 수

    public float BaseMultipleShot { get; set; }                   // 기준 동시 발사 수
    public float MultipleShot
    {
        get => multipleShot;
        set => multipleShot = Mathf.Max(0, value);               // 0 이상 제한 (최대는 없음)
    }

    // ───────── 치명타 확률 및 배율 관련 ─────────
    [SerializeField, Range(0f, 100f)]
    private float baseCriticalRate = 0f;                          // 기준 치명타 확률 (%)
    [SerializeField, Range(0f, 500f)]
    private float criticalDamageMultiplier = 100f;                // 치명타 데미지 배율 (%)
    [SerializeField, Range(0f, 100f)]
    private float criticalRate = 0f;                              // 현재 치명타 확률 (%)

    public float BaseCriticalRate { get; set; }                    // 기준 치명타 확률
    public float CriticalDamageMultiplier { get; set; }            // 치명타 데미지 배율 (%)
    public float CriticalRate
    {
        get => criticalRate;
        set => criticalRate = Mathf.Clamp(value, 0, 100);         // 0~100% 제한
    }

    // ───────── 명중률 관련 ─────────
    [SerializeField, Range(0f, 100f)]
    private float baseAccuracyRate = 100f;                         // 기준 명중률 (%)
    [SerializeField, Range(0f, 100f)]
    private float accuracyRate = 100f;                             // 현재 명중률 (%)

    public float BaseAccuracyRate { get; set; }                     // 기준 명중률
    public float AccuracyRate
    {
        get => accuracyRate;
        set => accuracyRate = Mathf.Clamp(value, 0, 100);         // 0~100% 제한
    }

    // ───────── 회피율 관련 ─────────
    [SerializeField, Range(0f, 100f)]
    private float baseEvasionRate = 0f;                            // 기준 회피율 (%)
    [SerializeField, Range(0f, 100f)]
    private float evasionRate = 0f;                                // 현재 회피율 (%)

    public float BaseEvasionRate { get; set; }                      // 기준 회피율
    public float EvasionRate
    {
        get => evasionRate;
        set => evasionRate = Mathf.Clamp(value, 0, 100);           // 0~100% 제한
    }


    // ───────── 초기화 메서드 ─────────
    public void InitializeStats()
    {
        // 기준값과 배율 값을 필드에서 프로퍼티로 복사
        BaseHealth = baseHealth;
        HealthMultiplier = healthMultiplier;
        Health = health;
        HealthRegen = healthRegen;
        HealingMultiplier = healingMultiplier;

        BaseDefense = baseDefense;
        DefenseMultiplier = defenseMultiplier;
        Defense = defense;

        BaseSpeed = baseSpeed;
        SpeedMultiplier = speedMultiplier;
        Speed = speed;

        BaseAttackDamage = baseAttackDamage;
        AttackDamageMultiplier = attackDamageMultiplier;
        AttackDamage = attackDamage;

        BaseAttackRange = baseAttackRange;
        AttackRangeMultiplier = attackRangeMultiplier;
        AttackRange = attackRange;

        BaseAttackSpeed = baseAttackSpeed;
        AttackSpeedMultiplier = attackSpeedMultiplier;
        AttackSpeed = attackSpeed;

        BaseAttackSize = baseAttackSize;
        AttackSizeMultiplier = attackSizeMultiplier;
        AttackSize = attackSize;

        BaseNumberOfAttack = baseNumberOfAttack;
        NumberOfAttack = numberOfAttack;

        BaseMultipleShot = baseMultipleShot;
        MultipleShot = multipleShot;

        BaseCriticalRate = baseCriticalRate;
        CriticalDamageMultiplier = criticalDamageMultiplier;
        CriticalRate = criticalRate;

        BaseAccuracyRate = baseAccuracyRate;
        AccuracyRate = accuracyRate;

        BaseEvasionRate = baseEvasionRate;
        EvasionRate = evasionRate;
    }


    // ───────── 체력 1초당 재생 처리 (예시용) ─────────
    private void Update()
    {
        RegenerateHealth();
    }
    private void RegenerateHealth()
    {
        if (Health < MaxHealth)
        {
            // 재생량에 증폭 배율을 곱해줌
            float regenAmount = HealthRegen * (HealingMultiplier / 100f) * Time.deltaTime;
            Health += regenAmount;
        }
    }

    // ───────── 사망 처리 (예시) ─────────
    private void Die()
    {
        Debug.Log("플레이어가 사망했습니다.");
        // 사망 관련 처리(애니메이션, 상태 변경 등)를 여기에 작성하세요.
    }
}
