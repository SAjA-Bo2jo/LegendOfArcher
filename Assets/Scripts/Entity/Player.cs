using UnityEngine;

public class Player : Entity
{
    // ───────── 체력 관련 ─────────
    [SerializeField, Range(0f, 500f)]
    private float health = 100;                              // 현재 체력 (게임 중 변하는 값, 시작 시 MaxHealth로 설정)

    [SerializeField, Range(0f, 500f)]
    private float baseHealth = 100f;                   // 기준 체력 (배율 계산의 기준값)

    [SerializeField, Range(0f, 500f)]
    private float healthMultiplier = 100f;             // 체력 배율 (%) - 기본 100%

    [SerializeField] private float maxHealth = 100;

    [SerializeField, Range(0f, 100f)]
    private float healthRegen = 0f;                    // 초당 체력 재생량

    [SerializeField, Range(0f, 500f)]
    private float healingMultiplier = 100f;            // 체력 회복량 증폭 배율 (%)

    // 프로퍼티 (외부 접근 가능, set 가능)
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
    public float BaseHealth { get; private set; }              // 기준 체력 (내부에서만 set)
    public float HealthMultiplier { get; private set; }        // 체력 배율 (%) (내부에서만 set)

    // MaxHealth는 baseHealth * healthMultiplier / 100 으로 계산되므로 set을 안주고 get만 제공
    public float MaxHealth => BaseHealth * (HealthMultiplier / 100f);

    public float HealthRegen { get; private set; }             // 초당 체력 재생량 (내부에서만 set)
    public float HealingMultiplier { get; private set; }       // 회복량 증폭 배율 (%) (내부에서만 set)

    // ───────── 방어력 관련 ─────────
    [SerializeField, Range(0f, 1000f)]
    private float defense = 100f;                              // 현재 방어력 (시작 시 MaxDefense로 설정)

    [SerializeField, Range(0f, 1000f)]
    private float baseDefense = 100f;                          // 기준 방어력

    [SerializeField, Range(0f, 500f)]
    private float defenseMultiplier = 100f;                    // 방어력 배율 (%)

    public float Defense
    {
        get => defense;
        set => defense = Mathf.Clamp(value, 0, MaxDefense); // 현재 방어력은 0~최대 사이로 제한
    }

    public float BaseDefense { get; private set; }             // 기준 방어력 (내부에서만 set)
    public float DefenseMultiplier { get; private set; }       // 방어력 배율 (%) (내부에서만 set)
    public float MaxDefense => BaseDefense * (DefenseMultiplier / 100f); // 최대 방어력 계산식

    // ───────── 이동 속도 관련 ─────────
    [SerializeField, Range(0f, 20f)]
    private float speed = 5f;                                  // 현재 이동속도 (시작 시 MaxSpeed로 설정)

    [SerializeField, Range(0f, 20f)]
    private float baseSpeed = 5f;                              // 기준 이동속도

    [SerializeField, Range(0f, 500f)]
    private float speedMultiplier = 100f;                      // 이동속도 배율 (%)

    public float Speed
    {
        get => speed;
        set => speed = Mathf.Clamp(value, 0, MaxSpeed);      // 현재 이동속도 0~최대 이동속도 제한
    }
    public float BaseSpeed { get; private set; }               // 기준 이동속도 (내부에서만 set)
    public float SpeedMultiplier { get; private set; }         // 이동속도 배율 (%) (내부에서만 set)
    public float MaxSpeed => BaseSpeed * (SpeedMultiplier / 100f); // 최대 이동속도 계산식

    // ───────── 공격력 관련 ─────────
    [Header("공격 관련")]

    [SerializeField, Range(0f, 1000f)]
    private float attackDamage = 100f;                         // 현재 공격력 (시작 시 MaxAttackDamage로 설정)

    [SerializeField, Range(0f, 1000f)]
    private float baseAttackDamage = 100f;                     // 기준 공격력

    [SerializeField, Range(0f, 500f)]
    private float attackDamageMultiplier = 100f;               // 공격력 배율 (%)

    public float AttackDamage
    {
        get => attackDamage;
        set => attackDamage = Mathf.Clamp(value, 0, MaxAttackDamage); // 현재 공격력 0~최대 제한
    }
    public float BaseAttackDamage { get; private set; }        // 기준 공격력 (내부에서만 set)
    public float AttackDamageMultiplier { get; private set; }  // 공격력 배율 (%) (내부에서만 set)
    public float MaxAttackDamage => BaseAttackDamage * (AttackDamageMultiplier / 100f); // 최대 공격력

    // ───────── 공격 사거리 관련 ─────────
    [SerializeField, Range(0f, 50f)]
    private float attackRange = 10f;                           // 현재 공격 사거리 (시작 시 MaxAttackRange로 설정)

    [SerializeField, Range(0f, 50f)]
    private float baseAttackRange = 10f;                       // 기준 공격 사거리

    [SerializeField, Range(0f, 500f)]
    private float attackRangeMultiplier = 100f;                // 사거리 배율 (%)

    public float AttackRange
    {
        get => attackRange;
        set => attackRange = Mathf.Clamp(value, 0, MaxAttackRange); // 현재 사거리 0~최대 제한
    }
    public float BaseAttackRange { get; private set; }         // 기준 공격 사거리 (내부에서만 set)
    public float AttackRangeMultiplier { get; private set; }   // 사거리 배율 (%) (내부에서만 set)
    public float MaxAttackRange => BaseAttackRange * (AttackRangeMultiplier / 100f); // 최대 공격 사거리

    // ───────── 공격 속도 관련 ─────────
    [SerializeField, Range(0f, 50f)]
    private float attackSpeed = 25f;

    [SerializeField, Range(0f, 50f)]
    private float baseAttackSpeed = 5f;

    // 중요: Bow 스크립트에서 이 값을 (Multiplier/100f)로 사용해야 합니다.
    // 기본 배율은 100%이므로 100f로 설정합니다.
    [SerializeField, Range(0.01f, 500f)] // << Range 조정: 100f까지 가능하게 (필요시 더 늘릴 수도)
    private float attackSpeedMultiplier = 100f; // << 기본값을 100f로 변경!

    public float AttackSpeed
    {
        get => attackSpeed;
        set => attackSpeed = Mathf.Clamp(value, 0, MaxAttackSpeed);
    }

    public float BaseAttackSpeed { get; private set; }
    public float AttackSpeedMultiplier { get; private set; }
    public float MaxAttackSpeed => BaseAttackSpeed * (AttackSpeedMultiplier / 100f);

    // ───────── 공격 크기 관련 ─────────
    [SerializeField, Range(0f, 10f)]
    private float attackSize = 1f;                             // 현재 공격 크기 (시작 시 MaxAttackSize로 설정)

    [SerializeField, Range(0f, 10f)]
    private float baseAttackSize = 1f;                         // 기준 공격 크기

    [SerializeField, Range(0f, 500f)]
    private float attackSizeMultiplier = 100f;                 // 공격 크기 배율 (%)

    public float AttackSize
    {
        get => attackSize;
        set => attackSize = Mathf.Clamp(value, 0, MaxAttackSize); // 현재 공격 크기 0~최대 제한
    }
    public float BaseAttackSize { get; private set; }          // 기준 공격 크기 (내부에서만 set)
    public float AttackSizeMultiplier { get; private set; }    // 공격 크기 배율 (%) (내부에서만 set)
    public float MaxAttackSize => BaseAttackSize * (AttackSizeMultiplier / 100f); // 최대 공격 크기

    // ───────── 투사체 수 관련 (배율 없음) ─────────
    [SerializeField, Range(0f, 100f)]
    private float numberOfAttack = 1f;                         // 현재 투사체 수 (시작 시 BaseNumberOfAttack으로 설정)

    [SerializeField, Range(0f, 100f)]
    private float baseNumberOfAttack = 1f;                     // 기준 투사체 수

    public float NumberOfAttack
    {
        get => numberOfAttack;
        set => numberOfAttack = Mathf.Max(0, value);           // 0 이상 제한 (최대는 없음)
    }
    public float BaseNumberOfAttack { get; private set; }      // 기준 투사체 수 (내부에서만 set)

    // ───────── 동시 발사 수 관련 (배율 없음) ─────────
    [SerializeField, Range(0f, 10f)]
    private float multipleShot = 1f;                           // 현재 동시 발사 수 (시작 시 BaseMultipleShot으로 설정)

    [SerializeField, Range(0f, 10f)]
    private float baseMultipleShot = 1f;                       // 기준 동시 발사 수

    public float MultipleShot
    {
        get => multipleShot;
        set => multipleShot = Mathf.Max(0, value);             // 0 이상 제한 (최대는 없음)
    }
    public float BaseMultipleShot { get; private set; }        // 기준 동시 발사 수 (내부에서만 set)

    // ───────── 치명타 확률 및 배율 관련 ─────────
    [SerializeField, Range(0f, 100f)]
    private float criticalRate = 0f;                           // 현재 치명타 확률 (%) (시작 시 BaseCriticalRate로 설정)

    [SerializeField, Range(0f, 100f)]
    private float baseCriticalRate = 0f;                       // 기준 치명타 확률 (%)

    [SerializeField, Range(0f, 500f)]
    private float criticalDamageMultiplier = 100f;             // 치명타 데미지 배율 (%)

    public float CriticalRate
    {
        get => criticalRate;
        set => criticalRate = Mathf.Clamp(value, 0, 100);      // 0~100% 제한
    }
    public float BaseCriticalRate { get; private set; }        // 기준 치명타 확률 (내부에서만 set)
    public float CriticalDamageMultiplier { get; private set; } // 치명타 데미지 배율 (%) (내부에서만 set)

    // ───────── 명중률 관련 ─────────
    [SerializeField, Range(0f, 100f)]
    private float accuracyRate = 100f;                         // 현재 명중률 (%) (시작 시 BaseAccuracyRate로 설정)

    [SerializeField, Range(0f, 100f)]
    private float baseAccuracyRate = 100f;                     // 기준 명중률 (%)

    public float AccuracyRate
    {
        get => accuracyRate;
        set => accuracyRate = Mathf.Clamp(value, 0, 100);      // 0~100% 제한
    }
    public float BaseAccuracyRate { get; private set; }        // 기준 명중률 (내부에서만 set)

    // ───────── 회피율 관련 ─────────
    [SerializeField, Range(0f, 100f)]
    private float evasionRate = 0f;                            // 현재 회피율 (%) (시작 시 BaseEvasionRate로 설정)

    [SerializeField, Range(0f, 100f)]
    private float baseEvasionRate = 0f;                        // 기준 회피율 (%)

    public float EvasionRate
    {
        get => evasionRate;
        set => evasionRate = Mathf.Clamp(value, 0, 100);       // 0~100% 제한
    }
    public float BaseEvasionRate { get; private set; }         // 기준 회피율 (내부에서만 set)

    // ───────── 초기화 메서드 ─────────
    // Start() 이전에 호출되어 다른 스크립트가 Awake()에서 Player의 능력치에 접근할 수 있도록 합니다.
    private void Awake()
    {
        InitializeStats();
    }

    public void InitializeStats()
    {
        // 1. 인스펙터 필드 값들을 'Base' 프로퍼티에 할당합니다.
        //    이렇게 하면 인스펙터에서 설정된 값이 게임 시작 시 프로퍼티에 복사됩니다.
        BaseHealth = baseHealth;
        HealthMultiplier = healthMultiplier;

        HealthRegen = healthRegen;
        HealingMultiplier = healingMultiplier;

        BaseDefense = baseDefense;
        DefenseMultiplier = defenseMultiplier;

        BaseSpeed = baseSpeed;
        SpeedMultiplier = speedMultiplier;

        BaseAttackDamage = baseAttackDamage;
        AttackDamageMultiplier = attackDamageMultiplier;

        BaseAttackRange = baseAttackRange;
        AttackRangeMultiplier = attackRangeMultiplier;

        BaseAttackSpeed = baseAttackSpeed;
        AttackSpeedMultiplier = attackSpeedMultiplier;

        BaseAttackSize = baseAttackSize;
        AttackSizeMultiplier = attackSizeMultiplier;

        BaseNumberOfAttack = baseNumberOfAttack;

        BaseMultipleShot = baseMultipleShot;

        BaseCriticalRate = baseCriticalRate;
        CriticalDamageMultiplier = criticalDamageMultiplier;

        BaseAccuracyRate = baseAccuracyRate;

        BaseEvasionRate = baseEvasionRate;

        // 2. 'Base' 및 'Multiplier' 프로퍼티를 사용하여 현재(실제 사용될) 능력치 값을 계산하여 할당합니다.
        //    이렇게 하면 게임 시작 시 모든 능력치가 올바르게 계산된 값으로 설정됩니다.
        //    (특히 Bow 스크립트에서 사용되는 AttackSpeed, AttackDamage 등이 정확해집니다.)
        health = MaxHealth; // 시작 시 체력을 최대 체력으로 설정
        defense = MaxDefense;
        speed = MaxSpeed;
        attackDamage = MaxAttackDamage;
        attackRange = MaxAttackRange;
        attackSpeed = MaxAttackSpeed;
        attackSize = MaxAttackSize;
        numberOfAttack = BaseNumberOfAttack; // 투사체 수는 배율이 없으므로 Base 값을 그대로 사용
        multipleShot = BaseMultipleShot;     // 동시 발사 수도 배율이 없으므로 Base 값을 그대로 사용
        criticalRate = BaseCriticalRate;     // 치명타 확률도 배율이 없으므로 Base 값을 그대로 사용
        accuracyRate = BaseAccuracyRate;     // 명중률도 배율이 없으므로 Base 값을 그대로 사용
        evasionRate = BaseEvasionRate;       // 회피율도 배율이 없으므로 Base 값을 그대로 사용

        Debug.Log("Player stats initialized!");
        Debug.Log($"MaxHealth: {MaxHealth}, AttackSpeed: {AttackSpeed}, AttackDamage: {AttackDamage}");
    }

    private void Start()
    {
        // Awake()에서 초기화했으므로 Start()에서는 추가 초기화가 필요 없거나,
        // 다른 스크립트의 Start()와 상호작용하는 코드를 여기에 배치할 수 있습니다.
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