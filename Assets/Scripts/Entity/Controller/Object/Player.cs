using UnityEngine;

public class Player : Entity
{
    [Header("기본 스탯")]                                                              // ───── 기본 스탯 ─────

    [SerializeField, Range(0f, 500f)] private float health = 100f;                     // 기본 체력
    [SerializeField, Range(0f, 500f)] private float healthMultiplier = 100f;           // 체력 배율 (%)

    [SerializeField, Range(0f, 1000f)] private float defense = 100f;                   // 기본 방어력
    [SerializeField, Range(0f, 500f)] private float defenseMultiplier = 100f;          // 방어력 배율 (%)

    [SerializeField, Range(0f, 20f)] private float speed = 5f;                         // 이동 속도
    [SerializeField, Range(0f, 500f)] private float speedMultiplier = 100f;            // 이동 속도 배율 (%)

    [Header("공격 관련")]                                                              // ───── 공격 관련 ─────

    [SerializeField, Range(0f, 1000f)] private float attackDamage = 100f;              // 기본 공격력
    [SerializeField, Range(0f, 500f)] private float attackDamageMultiplier = 100f;     // 공격력 배율

    [SerializeField, Range(0f, 50f)] private float attackRange = 10f;                  // 공격 사거리
    [SerializeField, Range(0f, 500f)] private float attackRangeMultiplier = 100f;      // 사거리 배율

    [SerializeField, Range(0f, 50f)] private float attackSpeed = 5f;                   // 공격 속도
    [SerializeField, Range(0f, 5f)] private float attackSpeedMultiplier = 1f;          // 공격 속도 배율 (%)

    [SerializeField, Range(0f, 10f)] private float attackSize = 1f;                    // 공격 크기
    [SerializeField, Range(0f, 500f)] private float attackSizeMultiplier = 100f;       // 공격 크기 배율 (%)

    [SerializeField, Range(0f, 100f)] private float numberOfAttack = 1f;               // 공격 횟수 (투사체 수)
    [SerializeField, Range(0f, 10f)] private float multipleShot = 1f;                  // 동시 발사 수

    [SerializeField, Range(0f, 100f)] private float criticalRate = 0f;                 // 치명타 확률 (%)
    [SerializeField, Range(0f, 500f)] private float criticalDamageMultiplier = 100f;   // 치명타 데미지 배율 (%)

    [SerializeField, Range(0f, 100f)] private float accuracyRate = 100f;               // 명중률 (%)
    [SerializeField, Range(0f, 100f)] private float evasionRate = 0f;                  // 회피율 (%)

    // ───── 프로퍼티 (외부 접근용) ─────

    public float Health { get; set; }                      // 현재 체력
    public float HealthMultiplier { get; set; }            // 체력 배율 (%)

    public float Defense { get; set; }                     // 방어력
    public float DefenseMultiplier { get; set; }           // 방어력 배율 (%)

    public float Speed { get; set; }                       // 이동 속도
    public float SpeedMultiplier { get; set; }             // 이동 속도 배율 (%)

    public float AttackDamage { get; set; }                // 공격력
    public float AttackDamageMultiplier { get; set; }      // 공격력 배율 (%)

    public float AttackRange { get; set; }                 // 공격 사거리
    public float AttackRangeMultiplier { get; set; }       // 사거리 배율 (%)

    public float AttackSpeed { get; set; }                 // 공격 속도
    public float AttackSpeedMultiplier { get; set; }       // 공격 속도 배율 (%)

    public float AttackSize { get; set; }                  // 공격 크기
    public float AttackSizeMultiplier { get; set; }        // 공격 크기 배율 (%)

    public float NumberOfAttack { get; set; }              // 투사체 수
    public float MultipleShot { get; set; }                // 동시 발사 수

    public float CriticalRate { get; set; }                // 치명타 확률 (%)
    public float CriticalDamageMultiplier { get; set; }    // 치명타 데미지 배율 (%)

    public float AccuracyRate { get; set; }                // 명중률 (%)
    public float EvasionRate { get; set; }                 // 회피율 (%)

    protected override void Awake()
    {
        base.Awake();
        InitialStatSettings();
    }
    void InitialStatSettings()
    {
        Health = health;
        HealthMultiplier = healthMultiplier;
        Defense = defense;
        DefenseMultiplier = defenseMultiplier;
        Speed = speed;
        SpeedMultiplier = speedMultiplier;
        AttackDamage = attackDamage;
        AttackDamageMultiplier = attackDamageMultiplier;
        AttackRange = attackRange;
        AttackRangeMultiplier = attackRangeMultiplier;
        AttackSpeed = attackSpeed;
        AttackSpeedMultiplier = attackSpeedMultiplier;
        AttackSize = attackSize;
        AttackSizeMultiplier = attackSizeMultiplier;
        NumberOfAttack = numberOfAttack;
        MultipleShot = multipleShot;
        CriticalRate = criticalRate;
        CriticalDamageMultiplier = criticalDamageMultiplier;
        AccuracyRate = accuracyRate;
        EvasionRate = evasionRate;
    }    // 초기화 (필드 값 → 프로퍼티)
}
