using UnityEngine;

public class Player : Entity
{
    // ������������������ ü�� ���� ������������������
    [SerializeField, Range(0f, 500f)]
    private float baseHealth = 100f;                     // ���� ü�� (���� ����� ���ذ�)
    [SerializeField, Range(0f, 500f)]
    private float healthMultiplier = 100f;               // ü�� ���� (%) - �⺻ 100%

    [SerializeField, Range(0f, 500f)]
    private float health = 100f;                          // ���� ü�� (���� �� ���ϴ� ��)

    [SerializeField, Range(0f, 100f)]
    private float healthRegen = 0f;                       // �ʴ� ü�� �����
    [SerializeField, Range(0f, 500f)]
    private float healingMultiplier = 100f;               // ü�� ȸ���� ���� ���� (%)

    // ������Ƽ (�ܺ� ���� ����, set ����)
    public float BaseHealth { get; set; }                 // ���� ü��
    public float HealthMultiplier { get; set; }           // ü�� ���� (%)

    // MaxHealth�� baseHealth * healthMultiplier / 100 ���� ���ǹǷ� set�� ���ְ� get�� ����
    public float MaxHealth => BaseHealth * (HealthMultiplier / 100f);

    public float Health
    {
        get => health;
        set
        {
            // ���� ü���� 0 �̻� MaxHealth ���Ϸ� ����
            health = Mathf.Clamp(value, 0, MaxHealth);
            if (health <= 0)
            {
                Die(); // ü�� 0�� �Ǹ� ��� ó��
            }
        }
    }

    public float HealthRegen { get; set; }                // �ʴ� ü�� �����
    public float HealingMultiplier { get; set; }          // ȸ���� ���� ���� (%)

    // ������������������ ���� ���� ������������������
    [SerializeField, Range(0f, 1000f)]
    private float baseDefense = 100f;                      // ���� ����
    [SerializeField, Range(0f, 500f)]
    private float defenseMultiplier = 100f;                // ���� ���� (%)
    [SerializeField, Range(0f, 1000f)]
    private float defense = 100f;                           // ���� ����

    public float BaseDefense { get; set; }                  // ���� ����
    public float DefenseMultiplier { get; set; }            // ���� ���� (%)
    public float MaxDefense => BaseDefense * (DefenseMultiplier / 100f); // �ִ� ���� ����
    public float Defense
    {
        get => defense;
        set => defense = Mathf.Clamp(value, 0, MaxDefense); // ���� ������ 0~�ִ� ���̷� ����
    }

    // ������������������ �̵� �ӵ� ���� ������������������
    [SerializeField, Range(0f, 20f)]
    private float baseSpeed = 5f;                           // ���� �̵��ӵ�
    [SerializeField, Range(0f, 500f)]
    private float speedMultiplier = 100f;                   // �̵��ӵ� ���� (%)
    [SerializeField, Range(0f, 20f)]
    private float speed = 5f;                               // ���� �̵��ӵ�

    public float BaseSpeed { get; set; }                     // ���� �̵��ӵ�
    public float SpeedMultiplier { get; set; }               // �̵��ӵ� ���� (%)
    public float MaxSpeed => BaseSpeed * (SpeedMultiplier / 100f); // �ִ� �̵��ӵ� ����
    public float Speed
    {
        get => speed;
        set => speed = Mathf.Clamp(value, 0, MaxSpeed);     // ���� �̵��ӵ� 0~�ִ� �̵��ӵ� ����
    }

    // ������������������ ���ݷ� ���� ������������������
    [Header("���� ����")]

    [SerializeField, Range(0f, 1000f)]
    private float baseAttackDamage = 100f;                   // ���� ���ݷ�
    [SerializeField, Range(0f, 500f)]
    private float attackDamageMultiplier = 100f;             // ���ݷ� ���� (%)
    [SerializeField, Range(0f, 1000f)]
    private float attackDamage = 100f;                        // ���� ���ݷ�

    public float BaseAttackDamage { get; set; }               // ���� ���ݷ�
    public float AttackDamageMultiplier { get; set; }         // ���ݷ� ���� (%)
    public float MaxAttackDamage => BaseAttackDamage * (AttackDamageMultiplier / 100f); // �ִ� ���ݷ�
    public float AttackDamage
    {
        get => attackDamage;
        set => attackDamage = Mathf.Clamp(value, 0, MaxAttackDamage); // ���� ���ݷ� 0~�ִ� ����
    }

    // ������������������ ���� ��Ÿ� ���� ������������������
    [SerializeField, Range(0f, 50f)]
    private float baseAttackRange = 10f;                       // ���� ���� ��Ÿ�
    [SerializeField, Range(0f, 500f)]
    private float attackRangeMultiplier = 100f;                // ��Ÿ� ���� (%)
    [SerializeField, Range(0f, 50f)]
    private float attackRange = 10f;                            // ���� ���� ��Ÿ�

    public float BaseAttackRange { get; set; }                  // ���� ���� ��Ÿ�
    public float AttackRangeMultiplier { get; set; }            // ��Ÿ� ���� (%)
    public float MaxAttackRange => BaseAttackRange * (AttackRangeMultiplier / 100f); // �ִ� ���� ��Ÿ�
    public float AttackRange
    {
        get => attackRange;
        set => attackRange = Mathf.Clamp(value, 0, MaxAttackRange); // ���� ��Ÿ� 0~�ִ� ����
    }

    // ������������������ ���� �ӵ� ���� ������������������
    [SerializeField, Range(0f, 50f)]
    private float baseAttackSpeed = 5f;                         // ���� ���� �ӵ�
    [SerializeField, Range(0f, 5f)]
    private float attackSpeedMultiplier = 1f;                   // ���� �ӵ� ���� (%)
    [SerializeField, Range(0f, 50f)]
    private float attackSpeed = 5f;                             // ���� ���� �ӵ�

    public float BaseAttackSpeed { get; set; }                   // ���� ���� �ӵ�
    public float AttackSpeedMultiplier { get; set; }             // ���� �ӵ� ���� (%)
    public float MaxAttackSpeed => BaseAttackSpeed * (AttackSpeedMultiplier / 100f); // �ִ� ���� �ӵ�
    public float AttackSpeed
    {
        get => attackSpeed;
        set => attackSpeed = Mathf.Clamp(value, 0, MaxAttackSpeed); // ���� ���� �ӵ� 0~�ִ� ����
    }

    // ������������������ ���� ũ�� ���� ������������������
    [SerializeField, Range(0f, 10f)]
    private float baseAttackSize = 1f;                           // ���� ���� ũ��
    [SerializeField, Range(0f, 500f)]
    private float attackSizeMultiplier = 100f;                   // ���� ũ�� ���� (%)
    [SerializeField, Range(0f, 10f)]
    private float attackSize = 1f;                               // ���� ���� ũ��

    public float BaseAttackSize { get; set; }                     // ���� ���� ũ��
    public float AttackSizeMultiplier { get; set; }               // ���� ũ�� ���� (%)
    public float MaxAttackSize => BaseAttackSize * (AttackSizeMultiplier / 100f); // �ִ� ���� ũ��
    public float AttackSize
    {
        get => attackSize;
        set => attackSize = Mathf.Clamp(value, 0, MaxAttackSize); // ���� ���� ũ�� 0~�ִ� ����
    }

    // ������������������ ����ü �� ���� (���� ����) ������������������
    [SerializeField, Range(0f, 100f)]
    private float baseNumberOfAttack = 1f;                        // ���� ����ü ��
    [SerializeField, Range(0f, 100f)]
    private float numberOfAttack = 1f;                            // ���� ����ü ��

    public float BaseNumberOfAttack { get; set; }                 // ���� ����ü ��
    public float NumberOfAttack
    {
        get => numberOfAttack;
        set => numberOfAttack = Mathf.Max(0, value);             // 0 �̻� ���� (�ִ�� ����)
    }

    // ������������������ ���� �߻� �� ���� (���� ����) ������������������
    [SerializeField, Range(0f, 10f)]
    private float baseMultipleShot = 1f;                          // ���� ���� �߻� ��
    [SerializeField, Range(0f, 10f)]
    private float multipleShot = 1f;                              // ���� ���� �߻� ��

    public float BaseMultipleShot { get; set; }                   // ���� ���� �߻� ��
    public float MultipleShot
    {
        get => multipleShot;
        set => multipleShot = Mathf.Max(0, value);               // 0 �̻� ���� (�ִ�� ����)
    }

    // ������������������ ġ��Ÿ Ȯ�� �� ���� ���� ������������������
    [SerializeField, Range(0f, 100f)]
    private float baseCriticalRate = 0f;                          // ���� ġ��Ÿ Ȯ�� (%)
    [SerializeField, Range(0f, 500f)]
    private float criticalDamageMultiplier = 100f;                // ġ��Ÿ ������ ���� (%)
    [SerializeField, Range(0f, 100f)]
    private float criticalRate = 0f;                              // ���� ġ��Ÿ Ȯ�� (%)

    public float BaseCriticalRate { get; set; }                    // ���� ġ��Ÿ Ȯ��
    public float CriticalDamageMultiplier { get; set; }            // ġ��Ÿ ������ ���� (%)
    public float CriticalRate
    {
        get => criticalRate;
        set => criticalRate = Mathf.Clamp(value, 0, 100);         // 0~100% ����
    }

    // ������������������ ���߷� ���� ������������������
    [SerializeField, Range(0f, 100f)]
    private float baseAccuracyRate = 100f;                         // ���� ���߷� (%)
    [SerializeField, Range(0f, 100f)]
    private float accuracyRate = 100f;                             // ���� ���߷� (%)

    public float BaseAccuracyRate { get; set; }                     // ���� ���߷�
    public float AccuracyRate
    {
        get => accuracyRate;
        set => accuracyRate = Mathf.Clamp(value, 0, 100);         // 0~100% ����
    }

    // ������������������ ȸ���� ���� ������������������
    [SerializeField, Range(0f, 100f)]
    private float baseEvasionRate = 0f;                            // ���� ȸ���� (%)
    [SerializeField, Range(0f, 100f)]
    private float evasionRate = 0f;                                // ���� ȸ���� (%)

    public float BaseEvasionRate { get; set; }                      // ���� ȸ����
    public float EvasionRate
    {
        get => evasionRate;
        set => evasionRate = Mathf.Clamp(value, 0, 100);           // 0~100% ����
    }


    // ������������������ �ʱ�ȭ �޼��� ������������������
    public void InitializeStats()
    {
        // ���ذ��� ���� ���� �ʵ忡�� ������Ƽ�� ����
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


    // ������������������ ü�� 1�ʴ� ��� ó�� (���ÿ�) ������������������
    private void Update()
    {
        RegenerateHealth();
    }
    private void RegenerateHealth()
    {
        if (Health < MaxHealth)
        {
            // ������� ���� ������ ������
            float regenAmount = HealthRegen * (HealingMultiplier / 100f) * Time.deltaTime;
            Health += regenAmount;
        }
    }

    // ������������������ ��� ó�� (����) ������������������
    private void Die()
    {
        Debug.Log("�÷��̾ ����߽��ϴ�.");
        // ��� ���� ó��(�ִϸ��̼�, ���� ���� ��)�� ���⿡ �ۼ��ϼ���.
    }
}
