using UnityEngine;

public class Player : Entity
{
    // ������������������ ü�� ���� ������������������
    [SerializeField, Range(0f, 500f)]
    private float health = 100;                              // ���� ü�� (���� �� ���ϴ� ��, ���� �� MaxHealth�� ����)

    [SerializeField, Range(0f, 500f)]
    private float baseHealth = 100f;                   // ���� ü�� (���� ����� ���ذ�)

    [SerializeField, Range(0f, 500f)]
    private float healthMultiplier = 100f;             // ü�� ���� (%) - �⺻ 100%

    [SerializeField] private float maxHealth = 100;

    [SerializeField, Range(0f, 100f)]
    private float healthRegen = 0f;                    // �ʴ� ü�� �����

    [SerializeField, Range(0f, 500f)]
    private float healingMultiplier = 100f;            // ü�� ȸ���� ���� ���� (%)

    // ������Ƽ (�ܺ� ���� ����, set ����)
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
    public float BaseHealth { get; private set; }              // ���� ü�� (���ο����� set)
    public float HealthMultiplier { get; private set; }        // ü�� ���� (%) (���ο����� set)

    // MaxHealth�� baseHealth * healthMultiplier / 100 ���� ���ǹǷ� set�� ���ְ� get�� ����
    public float MaxHealth => BaseHealth * (HealthMultiplier / 100f);

    public float HealthRegen { get; private set; }             // �ʴ� ü�� ����� (���ο����� set)
    public float HealingMultiplier { get; private set; }       // ȸ���� ���� ���� (%) (���ο����� set)

    // ������������������ ���� ���� ������������������
    [SerializeField, Range(0f, 1000f)]
    private float defense = 100f;                              // ���� ���� (���� �� MaxDefense�� ����)

    [SerializeField, Range(0f, 1000f)]
    private float baseDefense = 100f;                          // ���� ����

    [SerializeField, Range(0f, 500f)]
    private float defenseMultiplier = 100f;                    // ���� ���� (%)

    public float Defense
    {
        get => defense;
        set => defense = Mathf.Clamp(value, 0, MaxDefense); // ���� ������ 0~�ִ� ���̷� ����
    }

    public float BaseDefense { get; private set; }             // ���� ���� (���ο����� set)
    public float DefenseMultiplier { get; private set; }       // ���� ���� (%) (���ο����� set)
    public float MaxDefense => BaseDefense * (DefenseMultiplier / 100f); // �ִ� ���� ����

    // ������������������ �̵� �ӵ� ���� ������������������
    [SerializeField, Range(0f, 20f)]
    private float speed = 5f;                                  // ���� �̵��ӵ� (���� �� MaxSpeed�� ����)

    [SerializeField, Range(0f, 20f)]
    private float baseSpeed = 5f;                              // ���� �̵��ӵ�

    [SerializeField, Range(0f, 500f)]
    private float speedMultiplier = 100f;                      // �̵��ӵ� ���� (%)

    public float Speed
    {
        get => speed;
        set => speed = Mathf.Clamp(value, 0, MaxSpeed);      // ���� �̵��ӵ� 0~�ִ� �̵��ӵ� ����
    }
    public float BaseSpeed { get; private set; }               // ���� �̵��ӵ� (���ο����� set)
    public float SpeedMultiplier { get; private set; }         // �̵��ӵ� ���� (%) (���ο����� set)
    public float MaxSpeed => BaseSpeed * (SpeedMultiplier / 100f); // �ִ� �̵��ӵ� ����

    // ������������������ ���ݷ� ���� ������������������
    [Header("���� ����")]

    [SerializeField, Range(0f, 1000f)]
    private float attackDamage = 100f;                         // ���� ���ݷ� (���� �� MaxAttackDamage�� ����)

    [SerializeField, Range(0f, 1000f)]
    private float baseAttackDamage = 100f;                     // ���� ���ݷ�

    [SerializeField, Range(0f, 500f)]
    private float attackDamageMultiplier = 100f;               // ���ݷ� ���� (%)

    public float AttackDamage
    {
        get => attackDamage;
        set => attackDamage = Mathf.Clamp(value, 0, MaxAttackDamage); // ���� ���ݷ� 0~�ִ� ����
    }
    public float BaseAttackDamage { get; private set; }        // ���� ���ݷ� (���ο����� set)
    public float AttackDamageMultiplier { get; private set; }  // ���ݷ� ���� (%) (���ο����� set)
    public float MaxAttackDamage => BaseAttackDamage * (AttackDamageMultiplier / 100f); // �ִ� ���ݷ�

    // ������������������ ���� ��Ÿ� ���� ������������������
    [SerializeField, Range(0f, 50f)]
    private float attackRange = 10f;                           // ���� ���� ��Ÿ� (���� �� MaxAttackRange�� ����)

    [SerializeField, Range(0f, 50f)]
    private float baseAttackRange = 10f;                       // ���� ���� ��Ÿ�

    [SerializeField, Range(0f, 500f)]
    private float attackRangeMultiplier = 100f;                // ��Ÿ� ���� (%)

    public float AttackRange
    {
        get => attackRange;
        set => attackRange = Mathf.Clamp(value, 0, MaxAttackRange); // ���� ��Ÿ� 0~�ִ� ����
    }
    public float BaseAttackRange { get; private set; }         // ���� ���� ��Ÿ� (���ο����� set)
    public float AttackRangeMultiplier { get; private set; }   // ��Ÿ� ���� (%) (���ο����� set)
    public float MaxAttackRange => BaseAttackRange * (AttackRangeMultiplier / 100f); // �ִ� ���� ��Ÿ�

    // ������������������ ���� �ӵ� ���� ������������������
    [SerializeField, Range(0f, 50f)]
    private float attackSpeed = 25f;

    [SerializeField, Range(0f, 50f)]
    private float baseAttackSpeed = 5f;

    // �߿�: Bow ��ũ��Ʈ���� �� ���� (Multiplier/100f)�� ����ؾ� �մϴ�.
    // �⺻ ������ 100%�̹Ƿ� 100f�� �����մϴ�.
    [SerializeField, Range(0.01f, 500f)] // << Range ����: 100f���� �����ϰ� (�ʿ�� �� �ø� ����)
    private float attackSpeedMultiplier = 100f; // << �⺻���� 100f�� ����!

    public float AttackSpeed
    {
        get => attackSpeed;
        set => attackSpeed = Mathf.Clamp(value, 0, MaxAttackSpeed);
    }

    public float BaseAttackSpeed { get; private set; }
    public float AttackSpeedMultiplier { get; private set; }
    public float MaxAttackSpeed => BaseAttackSpeed * (AttackSpeedMultiplier / 100f);

    // ������������������ ���� ũ�� ���� ������������������
    [SerializeField, Range(0f, 10f)]
    private float attackSize = 1f;                             // ���� ���� ũ�� (���� �� MaxAttackSize�� ����)

    [SerializeField, Range(0f, 10f)]
    private float baseAttackSize = 1f;                         // ���� ���� ũ��

    [SerializeField, Range(0f, 500f)]
    private float attackSizeMultiplier = 100f;                 // ���� ũ�� ���� (%)

    public float AttackSize
    {
        get => attackSize;
        set => attackSize = Mathf.Clamp(value, 0, MaxAttackSize); // ���� ���� ũ�� 0~�ִ� ����
    }
    public float BaseAttackSize { get; private set; }          // ���� ���� ũ�� (���ο����� set)
    public float AttackSizeMultiplier { get; private set; }    // ���� ũ�� ���� (%) (���ο����� set)
    public float MaxAttackSize => BaseAttackSize * (AttackSizeMultiplier / 100f); // �ִ� ���� ũ��

    // ������������������ ����ü �� ���� (���� ����) ������������������
    [SerializeField, Range(0f, 100f)]
    private float numberOfAttack = 1f;                         // ���� ����ü �� (���� �� BaseNumberOfAttack���� ����)

    [SerializeField, Range(0f, 100f)]
    private float baseNumberOfAttack = 1f;                     // ���� ����ü ��

    public float NumberOfAttack
    {
        get => numberOfAttack;
        set => numberOfAttack = Mathf.Max(0, value);           // 0 �̻� ���� (�ִ�� ����)
    }
    public float BaseNumberOfAttack { get; private set; }      // ���� ����ü �� (���ο����� set)

    // ������������������ ���� �߻� �� ���� (���� ����) ������������������
    [SerializeField, Range(0f, 10f)]
    private float multipleShot = 1f;                           // ���� ���� �߻� �� (���� �� BaseMultipleShot���� ����)

    [SerializeField, Range(0f, 10f)]
    private float baseMultipleShot = 1f;                       // ���� ���� �߻� ��

    public float MultipleShot
    {
        get => multipleShot;
        set => multipleShot = Mathf.Max(0, value);             // 0 �̻� ���� (�ִ�� ����)
    }
    public float BaseMultipleShot { get; private set; }        // ���� ���� �߻� �� (���ο����� set)

    // ������������������ ġ��Ÿ Ȯ�� �� ���� ���� ������������������
    [SerializeField, Range(0f, 100f)]
    private float criticalRate = 0f;                           // ���� ġ��Ÿ Ȯ�� (%) (���� �� BaseCriticalRate�� ����)

    [SerializeField, Range(0f, 100f)]
    private float baseCriticalRate = 0f;                       // ���� ġ��Ÿ Ȯ�� (%)

    [SerializeField, Range(0f, 500f)]
    private float criticalDamageMultiplier = 100f;             // ġ��Ÿ ������ ���� (%)

    public float CriticalRate
    {
        get => criticalRate;
        set => criticalRate = Mathf.Clamp(value, 0, 100);      // 0~100% ����
    }
    public float BaseCriticalRate { get; private set; }        // ���� ġ��Ÿ Ȯ�� (���ο����� set)
    public float CriticalDamageMultiplier { get; private set; } // ġ��Ÿ ������ ���� (%) (���ο����� set)

    // ������������������ ���߷� ���� ������������������
    [SerializeField, Range(0f, 100f)]
    private float accuracyRate = 100f;                         // ���� ���߷� (%) (���� �� BaseAccuracyRate�� ����)

    [SerializeField, Range(0f, 100f)]
    private float baseAccuracyRate = 100f;                     // ���� ���߷� (%)

    public float AccuracyRate
    {
        get => accuracyRate;
        set => accuracyRate = Mathf.Clamp(value, 0, 100);      // 0~100% ����
    }
    public float BaseAccuracyRate { get; private set; }        // ���� ���߷� (���ο����� set)

    // ������������������ ȸ���� ���� ������������������
    [SerializeField, Range(0f, 100f)]
    private float evasionRate = 0f;                            // ���� ȸ���� (%) (���� �� BaseEvasionRate�� ����)

    [SerializeField, Range(0f, 100f)]
    private float baseEvasionRate = 0f;                        // ���� ȸ���� (%)

    public float EvasionRate
    {
        get => evasionRate;
        set => evasionRate = Mathf.Clamp(value, 0, 100);       // 0~100% ����
    }
    public float BaseEvasionRate { get; private set; }         // ���� ȸ���� (���ο����� set)

    // ������������������ �ʱ�ȭ �޼��� ������������������
    // Start() ������ ȣ��Ǿ� �ٸ� ��ũ��Ʈ�� Awake()���� Player�� �ɷ�ġ�� ������ �� �ֵ��� �մϴ�.
    private void Awake()
    {
        InitializeStats();
    }

    public void InitializeStats()
    {
        // 1. �ν����� �ʵ� ������ 'Base' ������Ƽ�� �Ҵ��մϴ�.
        //    �̷��� �ϸ� �ν����Ϳ��� ������ ���� ���� ���� �� ������Ƽ�� ����˴ϴ�.
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

        // 2. 'Base' �� 'Multiplier' ������Ƽ�� ����Ͽ� ����(���� ����) �ɷ�ġ ���� ����Ͽ� �Ҵ��մϴ�.
        //    �̷��� �ϸ� ���� ���� �� ��� �ɷ�ġ�� �ùٸ��� ���� ������ �����˴ϴ�.
        //    (Ư�� Bow ��ũ��Ʈ���� ���Ǵ� AttackSpeed, AttackDamage ���� ��Ȯ�����ϴ�.)
        health = MaxHealth; // ���� �� ü���� �ִ� ü������ ����
        defense = MaxDefense;
        speed = MaxSpeed;
        attackDamage = MaxAttackDamage;
        attackRange = MaxAttackRange;
        attackSpeed = MaxAttackSpeed;
        attackSize = MaxAttackSize;
        numberOfAttack = BaseNumberOfAttack; // ����ü ���� ������ �����Ƿ� Base ���� �״�� ���
        multipleShot = BaseMultipleShot;     // ���� �߻� ���� ������ �����Ƿ� Base ���� �״�� ���
        criticalRate = BaseCriticalRate;     // ġ��Ÿ Ȯ���� ������ �����Ƿ� Base ���� �״�� ���
        accuracyRate = BaseAccuracyRate;     // ���߷��� ������ �����Ƿ� Base ���� �״�� ���
        evasionRate = BaseEvasionRate;       // ȸ������ ������ �����Ƿ� Base ���� �״�� ���

        Debug.Log("Player stats initialized!");
        Debug.Log($"MaxHealth: {MaxHealth}, AttackSpeed: {AttackSpeed}, AttackDamage: {AttackDamage}");
    }

    private void Start()
    {
        // Awake()���� �ʱ�ȭ�����Ƿ� Start()������ �߰� �ʱ�ȭ�� �ʿ� ���ų�,
        // �ٸ� ��ũ��Ʈ�� Start()�� ��ȣ�ۿ��ϴ� �ڵ带 ���⿡ ��ġ�� �� �ֽ��ϴ�.
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