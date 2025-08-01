using UnityEngine;

public class Player : Entity
{
    [Header("�⺻ ����")]                                                              // ���������� �⺻ ���� ����������

    [SerializeField, Range(0f, 500f)] private float health = 100f;                     // �⺻ ü��
    [SerializeField, Range(0f, 500f)] private float healthMultiplier = 100f;           // ü�� ���� (%)

    [SerializeField, Range(0f, 1000f)] private float defense = 100f;                   // �⺻ ����
    [SerializeField, Range(0f, 500f)] private float defenseMultiplier = 100f;          // ���� ���� (%)

    [SerializeField, Range(0f, 20f)] private float speed = 5f;                         // �̵� �ӵ�
    [SerializeField, Range(0f, 500f)] private float speedMultiplier = 100f;            // �̵� �ӵ� ���� (%)

    [Header("���� ����")]                                                              // ���������� ���� ���� ����������

    [SerializeField, Range(0f, 1000f)] private float attackDamage = 100f;              // �⺻ ���ݷ�
    [SerializeField, Range(0f, 500f)] private float attackDamageMultiplier = 100f;     // ���ݷ� ����

    [SerializeField, Range(0f, 50f)] private float attackRange = 10f;                  // ���� ��Ÿ�
    [SerializeField, Range(0f, 500f)] private float attackRangeMultiplier = 100f;      // ��Ÿ� ����

    [SerializeField, Range(0f, 50f)] private float attackSpeed = 5f;                   // ���� �ӵ�
    [SerializeField, Range(0f, 5f)] private float attackSpeedMultiplier = 1f;          // ���� �ӵ� ���� (%)

    [SerializeField, Range(0f, 10f)] private float attackSize = 1f;                    // ���� ũ��
    [SerializeField, Range(0f, 500f)] private float attackSizeMultiplier = 100f;       // ���� ũ�� ���� (%)

    [SerializeField, Range(0f, 100f)] private float numberOfAttack = 1f;               // ���� Ƚ�� (����ü ��)
    [SerializeField, Range(0f, 10f)] private float multipleShot = 1f;                  // ���� �߻� ��

    [SerializeField, Range(0f, 100f)] private float criticalRate = 0f;                 // ġ��Ÿ Ȯ�� (%)
    [SerializeField, Range(0f, 500f)] private float criticalDamageMultiplier = 100f;   // ġ��Ÿ ������ ���� (%)

    [SerializeField, Range(0f, 100f)] private float accuracyRate = 100f;               // ���߷� (%)
    [SerializeField, Range(0f, 100f)] private float evasionRate = 0f;                  // ȸ���� (%)

    // ���������� ������Ƽ (�ܺ� ���ٿ�) ����������

    public float Health { get; set; }                      // ���� ü��
    public float HealthMultiplier { get; set; }            // ü�� ���� (%)

    public float Defense { get; set; }                     // ����
    public float DefenseMultiplier { get; set; }           // ���� ���� (%)

    public float Speed { get; set; }                       // �̵� �ӵ�
    public float SpeedMultiplier { get; set; }             // �̵� �ӵ� ���� (%)

    public float AttackDamage { get; set; }                // ���ݷ�
    public float AttackDamageMultiplier { get; set; }      // ���ݷ� ���� (%)

    public float AttackRange { get; set; }                 // ���� ��Ÿ�
    public float AttackRangeMultiplier { get; set; }       // ��Ÿ� ���� (%)

    public float AttackSpeed { get; set; }                 // ���� �ӵ�
    public float AttackSpeedMultiplier { get; set; }       // ���� �ӵ� ���� (%)

    public float AttackSize { get; set; }                  // ���� ũ��
    public float AttackSizeMultiplier { get; set; }        // ���� ũ�� ���� (%)

    public float NumberOfAttack { get; set; }              // ����ü ��
    public float MultipleShot { get; set; }                // ���� �߻� ��

    public float CriticalRate { get; set; }                // ġ��Ÿ Ȯ�� (%)
    public float CriticalDamageMultiplier { get; set; }    // ġ��Ÿ ������ ���� (%)

    public float AccuracyRate { get; set; }                // ���߷� (%)
    public float EvasionRate { get; set; }                 // ȸ���� (%)

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
    }    // �ʱ�ȭ (�ʵ� �� �� ������Ƽ)
}
