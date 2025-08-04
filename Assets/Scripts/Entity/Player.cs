using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{

    [Header("�⺻ ����")]
    // --- ü�� ���� ���� ---
    [SerializeField]
    private float maxHealth = 100f; // �ִ� ü��

    // �ִ� ü��

    // Health ������Ƽ�� ���� ���� ������ private �ʵ�
    private float _health;
    public float Health
    {
        get => _health;
        set
        {
            // �Ҵ��Ϸ��� �� ü�� ���� ���� ü�º��� ���� �� (���ظ� �Դ� ��Ȳ)
            if (value < _health)
            {
                // Health ������Ƽ�� ���� �����ϴ� ���, TakeDamage �޼��带 ȣ���Ͽ� ���� ó�� ������ ����
                // �̶�, 'value'�� '_health'�� ���̸�ŭ�� ���ط����� �Ѱ��ݴϴ�.
                // TakeDamage �޼��� ���ο��� _health ���� ���� �����ϹǷ� ���� ��Ͱ� �߻����� �ʽ��ϴ�.
                float damageAmount = _health - value;
                TakeDamage(damageAmount); // ���������� �������� ó���ϴ� �� �޼��� ȣ��
            }
            else // ü���� �����ϰų� ���� ������ ������ �� (ȸ��, �ʱ�ȭ ��)
            {
                _health = Mathf.Min(value, MaxHealth); // �ִ� ü���� ���� �ʵ��� ����
                Debug.Log($"ü�� ������Ʈ: {_health}. (TakeDamageExternal)");
            }

            // ü���� 0 ���ϰ� �Ǹ� ��� ó�� (�� �κ��� �ܺο��� ü���� ���� 0 ���Ϸ� ������ ��츦 ���)
            if (_health <= 0)
            {
                Death();
            }
        }
    }
    public float MaxHealth // �ִ� ü��
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    [SerializeField] private float baseDefense = 0f;
    public float Defense { get; set; } // ���� ���� (�޴� ���� ����)

    [SerializeField] private float baseMoveSpeed = 5.0f;
    public float MoveSpeed { get; set; } // ���� �̵� �ӵ�

    [SerializeField] private float baseAttackDamage = 10f;
    public float AttackDamage { get; set; } // ���� ���� ������

    [SerializeField] private float baseAttackRange = 3f;
    public float AttackRange { get; set; } // ���� ���� ����

    [SerializeField] private float baseAttackSize = 1.0f;
    public float AttackSize { get; set; } // ����ü/������ ũ�� ����

    [SerializeField] private float baseCriticalRate = 10f; // �⺻ ġ��Ÿ Ȯ�� (%)
    public float CriticalRate { get; set; } // ���� ġ��Ÿ Ȯ�� (%)

    [SerializeField] private float baseProjectileSpeed = 7f;
    public float ProjectileSpeed { get; set; } // ���� ����ü �ӵ�

    [SerializeField] private float baseAttackSpeed = 1.0f; // �⺻ ���� �ӵ� (�ʴ� ���� Ƚ��)
    public float AttackSpeedMultiplier { get; set; } = 100f; // ���� �ӵ� ���� (100 = 100%)
    public float MaxAttackSpeed => baseAttackSpeed * (AttackSpeedMultiplier / 100f); // ���� ���� �ӵ� ���

    // --- ����ġ/���� ���� ���� ---
    [SerializeField] private int level = 1; // ���� ����
    public int Level => level; // ������ �б� �������� �ܺ� ����

    [SerializeField] private float experience = 0f; // ���� ����ġ
    public float Experience => experience; // ����ġ �б� ����

    [SerializeField] private float[] expToNextLevel; // �������� �ʿ��� ����ġ �迭 (�ν����Ϳ��� ����)

    // --- �ɷ� ���� �ʵ� ---
    // ȹ���� �ɷµ��� ������ ��ųʸ�: <�ɷ� ������ (Key), �ش� �ɷ� �ν��Ͻ� (Value)>
    public Dictionary<GameObject, Abillity> activeAbilities = new Dictionary<GameObject, Abillity>();

    [Header("�ɷ� �ռ� ������")]
    // �ռ� ������ ���
    public List<AbilityRecipe> abilityRecipes;

    void Awake()
    {
        // === �ٽ� ���� �κ� ===
        // AnimationHandler�� Player GameObject�� �ڽĿ� �����Ƿ� GetComponentInChildren�� ã���ϴ�.
        animationHandler = GetComponentInChildren<AnimationHandler>();
        if (animationHandler == null)
        {
            Debug.LogError("Player ��ũ��Ʈ: AnimationHandler ������Ʈ�� �ڽĿ��� ã�� �� �����ϴ�! ���� ��������Ʈ ������Ʈ�� �ִ��� Ȯ���ϼ���.");
        }
        // === ������� ���� ===

        RecalculateStats();
        // Awake �ÿ��� Health ������Ƽ�� set �����ڸ� ������ �ʰ� ���� _health �ʵ带 �ʱ�ȭ
        _health = maxHealth;

        if (abilityRecipes == null || abilityRecipes.Count == 0)
        {
            Debug.LogWarning("Player: Ability Recipes�� �Ҵ���� �ʾҽ��ϴ�. �ռ� ����� �۵����� ���� �� �ֽ��ϴ�.");
        }
    }

    /// <summary>
    /// �÷��̾��� ��� ������ �⺻������ �缳���ϰ�, Ȱ��ȭ�� �ɷ��� ȿ���� �ٽ� �����մϴ�.
    /// �ɷ� ȹ��/���� �� �Ǵ� ���ȿ� ������ �ִ� ������ ���� �� ȣ��˴ϴ�.
    /// </summary>
    public void RecalculateStats()
    {
        // ��� ������ �⺻������ �ʱ�ȭ
        MaxHealth = maxHealth; // �ִ� ü�µ� �⺻������ ����
        MoveSpeed = baseMoveSpeed;
        AttackDamage = baseAttackDamage;
        AttackRange = baseAttackRange;
        AttackSize = baseAttackSize;
        CriticalRate = baseCriticalRate;
        ProjectileSpeed = baseProjectileSpeed;
        AttackSpeedMultiplier = 100f;
        Defense = baseDefense;

        // Ȱ��ȭ�� ��� �ɷµ��� ȿ���� �������մϴ�.
        foreach (var abilityEntry in activeAbilities)
        {
            abilityEntry.Value.ApplyEffect();
        }

        Debug.Log("��� ���� ���� �Ϸ�.");
    }


    /// <summary>
    /// ������ �� ȣ��� �ɷ� ȹ�� �޼���.
    /// </summary>
    /// <param name="abilityPrefab">���õ� �ɷ��� ������.</param>
    public void AcquireAbility(GameObject abilityPrefab)
    {
        Abillity existingAbility = null;
        if (activeAbilities.TryGetValue(abilityPrefab, out existingAbility))
        {
            if (existingAbility.CurrentLevel < existingAbility.MaxLevel)
            {
                existingAbility.OnAcquire(this);
                Debug.Log($"[{existingAbility.AbilityName}] �ɷ��� ������! (Lv.{existingAbility.CurrentLevel})");
            }
            else
            {
                Debug.Log($"[{existingAbility.AbilityName}] �ɷ��� �̹� �ִ� �����Դϴ�. (Lv.{existingAbility.MaxLevel}). �ٸ� ������ ������ �� �ֽ��ϴ�.");
            }
        }
        else
        {
            GameObject abilityGO = Instantiate(abilityPrefab, transform);
            Abillity newAbility = abilityGO.GetComponent<Abillity>();

            if (newAbility != null)
            {
                newAbility.InitializeAbility(abilityPrefab);
                newAbility.OnAcquire(this);
                activeAbilities.Add(abilityPrefab, newAbility);
                Debug.Log($"[{newAbility.AbilityName}] ���ο� �ɷ� ȹ��! (Lv.{newAbility.CurrentLevel})");
            }
            else
            {
                Debug.LogError($"���õ� ������ {abilityPrefab.name}�� Abillity ������Ʈ�� �����ϴ�!");
                Destroy(abilityGO);
            }
        }
        RecalculateStats();
    }

    /// <summary>
    /// Ȱ��ȭ�� �ɷµ� �� Ư�� �ɷ��� �����մϴ�. (�ַ� �ռ� �� ���� �ɷ� ���ſ� ���)
    /// </summary>
    /// <param name="abilityPrefab">������ �ɷ��� ������.</param>
    public void RemoveAbility(GameObject abilityPrefab)
    {
        if (activeAbilities.TryGetValue(abilityPrefab, out Abillity abilityToRemove))
        {
            Debug.Log($"[{abilityToRemove.AbilityName}] �ɷ��� �����մϴ�.");
            abilityToRemove.OnRemove();
            Destroy(abilityToRemove.gameObject);
            activeAbilities.Remove(abilityPrefab);
            RecalculateStats();
        }
        else
        {
            Debug.LogWarning($"�����Ϸ��� �ɷ� ������ {abilityPrefab.name}��(��) Ȱ��ȭ�� ��Ͽ��� ã�� �� �����ϴ�.");
        }
    }

    /// <summary>
    /// ���� Ȱ��ȭ�� �ɷµ�� ������ ������� �ռ� ������ �����Ǹ� ã���ϴ�.
    /// </summary>
    /// <returns>�ռ� ������ �ɷ��� ������ ���.</returns>
    public List<GameObject> GetCombinableAbilities()
    {
        List<GameObject> combinableList = new List<GameObject>();

        if (abilityRecipes == null || abilityRecipes.Count == 0) return combinableList;

        foreach (AbilityRecipe recipe in abilityRecipes)
        {
            if (activeAbilities.ContainsKey(recipe.CombinedAbilityPrefab)) continue;

            bool canCombine = true;
            foreach (AbilityRecipe.RequiredAbility req in recipe.RequiredAbilities)
            {
                if (!activeAbilities.ContainsKey(req.AbilityPrefab) ||
                    activeAbilities[req.AbilityPrefab].CurrentLevel < req.RequiredLevel)
                {
                    canCombine = false;
                    break;
                }
            }

            if (canCombine)
            {
                combinableList.Add(recipe.CombinedAbilityPrefab);
            }
        }
        return combinableList;
    }

    /// <summary>
    /// Bow�� ȭ���� �߻��ϱ� ������ �� �Լ��� ȣ���Ͽ� Ư�� ȭ�� �ߵ��� �õ��մϴ�.
    /// </summary>
    /// <param name="regularArrowGO">�߻��Ϸ��� �Ϲ� ȭ�� GameObject.</param>
    /// <param name="regularArrowScript">�߻��Ϸ��� �Ϲ� ȭ�� Arrow ������Ʈ.</param>
    /// <returns>Ư�� ȭ���� �߻�Ǿ����� true, �ƴϸ� false.</returns>
    public bool TryActivateSpecialArrowAbility(GameObject regularArrowGO, Arrow regularArrowScript)
    {
        foreach (var entry in activeAbilities)
        {
            Abillity ability = entry.Value;
            if (ability is FireArrowAbility fireArrowAbility)
            {
                if (fireArrowAbility.TryActivateFireArrow(regularArrowGO, regularArrowScript))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // --- ����ġ ȹ�� �� ������ ���� ---
    /// <summary>
    /// �÷��̾�� ����ġ�� �߰��մϴ�.
    /// </summary>
    /// <param name="expAmount">�߰��� ����ġ ��.</param>
    public void AddExperience(float expAmount)
    {
        experience += expAmount;
        Debug.Log($"����ġ ȹ��: {expAmount}. ���� ����ġ: {experience}");

        if (level < expToNextLevel.Length && experience >= expToNextLevel[level - 1])
        {
            LevelUp();
        }
        else if (level >= expToNextLevel.Length)
        {
            Debug.Log("�ִ� ������ �����߽��ϴ�. �� �̻� ����ġ�� ȹ���� �� �����ϴ�.");
        }
    }

    /// <summary>
    /// �÷��̾ ������ ��Ű�� ���� ������ ������ŵ�ϴ�.
    /// </summary>
    private void LevelUp()
    {
        level++;
        experience = 0;
        Debug.Log($"������! ���� ����: {level}");

        maxHealth += 10;
        // MaxHealth ������Ƽ�� RecalculateStats���� ������Ʈ�ǹǷ�, ���⼭ ���� _health�� ����
        _health = maxHealth;
        baseAttackDamage += 1;
        baseMoveSpeed += 0.1f;

        RecalculateStats();
    }

    // --- ü�� ���� �޼��� ---
    /// <summary>
    /// �÷��̾�� ���ظ� ������ �ܺ� ȣ��� �޼���.
    /// �� �޼��带 ���� �÷��̾�� �������� �� �� �ֽ��ϴ�.
    /// </summary>
    /// <param name="damageAmount">���� ���ط�.</param>
    public void TakeDamage(float damageAmount)
    {
        // === �ٽ� ���� �κ� ===
        // animationHandler�� null�� �ƴ� ���� Hurt() ȣ��
        if (animationHandler != null)
        {
            animationHandler.Hurt(); // �ִϸ��̼� �ڵ鷯�� ���� ���� �ִϸ��̼� ����
        }
        else
        {
            Debug.LogWarning("Player.TakeDamage: AnimationHandler�� �Ҵ���� �ʾ� ���� �ִϸ��̼��� ����� �� �����ϴ�.");
        }
        // === ������� ���� ===

        float finalDamage = Mathf.Max(0, (damageAmount / (damageAmount + this.Defense)));

        // _health �ʵ带 ���� �����Ͽ� ���� ��͸� �����մϴ�.
        _health -= finalDamage;
        Debug.Log($"���ظ� �޾ҽ��ϴ�: {finalDamage}. ���� ü��: {_health}");

        if (_health <= 0)
        {
            Death();
        }
    }


    /// <summary>
    /// �÷��̾��� ü���� ȸ����ŵ�ϴ�.
    /// </summary>
    /// <param name="healAmount">ȸ���� ü�� ��.</param>
    public void Heal(float healAmount)
    {
        // Heal �޼���� ü���� ������Ű�� ����̹Ƿ�, Health ������Ƽ�� set �����ڸ� ����ص� �˴ϴ�.
        // �̶� Health ������Ƽ�� set ������ ���ο� �ִ� 'value < _health' ���ǿ� �ɸ��� �����Ƿ� �����մϴ�.
        Health = Mathf.Min(MaxHealth, Health + healAmount);
        Debug.Log($"ü�� ȸ��: {healAmount}. ���� ü��: {Health}");
    }

    /// <summary>
    /// �÷��̾� ��� ó�� ���� (�ʿ�� ����).
    /// </summary>
    private void Death()
    {
        // === �ٽ� ���� �κ� ===
        // animationHandler�� null�� �ƴ� ���� Death() ȣ��
        if (animationHandler != null)
        {
            animationHandler.Death();
        }
        else
        {
            Debug.LogWarning("Player.Death: AnimationHandler�� �Ҵ���� �ʾ� ��� �ִϸ��̼��� ����� �� �����ϴ�.");
        }
        // === ������� ���� ===

        Debug.Log("�÷��̾ ����߽��ϴ�!");
        // ���� ���� ó��, UI ǥ�� ��
        // Time.timeScale = 0f; // ���� �Ͻ� ���� (����)
    }
}