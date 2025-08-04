using System.Collections.Generic;
using UnityEngine;
using System.Linq; // LINQ ����� ���� �߰�

public class Player : MonoBehaviour
{
    [Header("�⺻ ����")]
    // --- ü�� ���� ���� ---
    [SerializeField] private float maxHealth = 100f; // �ִ� ü��
    public float Health { get; set; } // ���� ü�� (Public���� �ܺ� ���� ���)

    // --- ��� ���� ���� ---
    [SerializeField] private float baseDefense = 0f;
    public float Defense { get; set; } // ���� ���� (�޴� ���� ����)

    // --- �̵� �ӵ� ���� ---
    [SerializeField] private float baseMoveSpeed = 5.0f;
    public float MoveSpeed { get; set; } // ���� �̵� �ӵ�

    // --- ���� ���� ���� ---
    [SerializeField] private float baseAttackDamage = 10f;
    public float AttackDamage { get; set; } // ���� ���� ������

    [SerializeField] private float baseAttackRange = 5f;
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
    public int Level => level; // ������ �б� �������� �ܺ� ���� (���� ����)

    [SerializeField] private float experience = 0f; // ���� ����ġ
    public float Experience => experience; // ����ġ �б� ���� (���� ����)

    [SerializeField] private float[] expToNextLevel; // �������� �ʿ��� ����ġ �迭 (�ν����Ϳ��� ����)

    // --- �ɷ� ���� �ʵ� ---
    // ȹ���� �ɷµ��� ������ ��ųʸ�: <�ɷ� ������ (Key), �ش� �ɷ� �ν��Ͻ� (Value)>
    public Dictionary<GameObject, Abillity> activeAbilities = new Dictionary<GameObject, Abillity>();

    [Header("�ɷ� �ռ� ������")]
    // �ռ� ������ ��� (Unity �����Ϳ��� AbilityRecipe ScriptableObject ���µ��� �Ҵ�)
    public List<AbilityRecipe> abilityRecipes;

    void Awake()
    {
        // ��� ���� ������Ƽ�� �⺻������ �ʱ�ȭ
        Health = maxHealth; // ���� �� ���� ü���� �ִ� ü������ ����
        MoveSpeed = baseMoveSpeed;
        AttackDamage = baseAttackDamage;
        AttackRange = baseAttackRange;
        AttackSize = baseAttackSize;
        CriticalRate = baseCriticalRate;
        ProjectileSpeed = baseProjectileSpeed;
        AttackSpeedMultiplier = 100f; // �⺻���� 100% (���� ����)
        Defense = baseDefense;

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
        // Health�� �������� ���� ������ �ƴ�, ȸ��/���� �������� �����ǹǷ� ���꿡�� ����
        MoveSpeed = baseMoveSpeed;
        AttackDamage = baseAttackDamage;
        AttackRange = baseAttackRange;
        AttackSize = baseAttackSize;
        CriticalRate = baseCriticalRate;
        ProjectileSpeed = baseProjectileSpeed;
        AttackSpeedMultiplier = 100f;
        Defense = baseDefense;

        // Ȱ��ȭ�� ��� �ɷµ��� ȿ���� �������մϴ�.
        // �� Abillity�� ApplyEffect() �޼���� '���� ȿ���� ������ �� ���� ������ ȿ���� ����'�ϴ� ������� �����Ǿ�� �մϴ�.
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
        // �̹� �� �ɷ��� ������ �ִ��� Ȯ��
        if (activeAbilities.TryGetValue(abilityPrefab, out existingAbility))
        {
            // �̹� �ɷ��� ������ �ְ�, �ִ� ������ �ƴ϶�� ������ �õ�
            if (existingAbility.CurrentLevel < existingAbility.MaxLevel)
            {
                existingAbility.OnAcquire(this); // �÷��̾� �ν��Ͻ��� �����Ͽ� ������ �� ȿ�� ����
                Debug.Log($"[{existingAbility.AbilityName}] �ɷ��� ������! (Lv.{existingAbility.CurrentLevel})");
            }
            else
            {
                Debug.Log($"[{existingAbility.AbilityName}] �ɷ��� �̹� �ִ� �����Դϴ�. (Lv.{existingAbility.MaxLevel}). �ٸ� ������ ������ �� �ֽ��ϴ�.");
                // TODO: �ִ� ���� �ɷ� ���� �� �ٸ� ���� (��: ���, �缱�� ��ȸ ��) ���� ����
            }
        }
        else
        {
            // ���ο� �ɷ� ȹ�� (�������� �ν��Ͻ�ȭ�Ͽ� ���� ������Ʈ�� ����� ������Ʈ ��������)
            GameObject abilityGO = Instantiate(abilityPrefab, transform); // �÷��̾��� �ڽ����� �߰� (���� ����)
            Abillity newAbility = abilityGO.GetComponent<Abillity>();

            if (newAbility != null)
            {
                newAbility.InitializeAbility(abilityPrefab); // �ɷ� �ʱ�ȭ �� ������ ���� ����
                newAbility.OnAcquire(this); // �÷��̾� �ν��Ͻ��� �����Ͽ� �ʱ� ȹ�� �� ȿ�� ����
                activeAbilities.Add(abilityPrefab, newAbility); // ��ųʸ��� �߰�
                Debug.Log($"[{newAbility.AbilityName}] ���ο� �ɷ� ȹ��! (Lv.{newAbility.CurrentLevel})");
            }
            else
            {
                Debug.LogError($"���õ� ������ {abilityPrefab.name}�� Abillity ������Ʈ�� �����ϴ�!");
                Destroy(abilityGO); // �߸��� �������̸� ������ ������Ʈ ����
            }
        }

        // �ɷ� ȹ�� �� ���� ���� (���ȿ� ������ �ִ� ��� �ɷ��� ����ǵ���)
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
            abilityToRemove.OnRemove(); // �ɷ� ���� ȿ�� ȣ�� (��: ���� ����)
            Destroy(abilityToRemove.gameObject); // ���� ������Ʈ �ı�
            activeAbilities.Remove(abilityPrefab); // ��ųʸ����� ����

            // �ɷ� ���� �� ���� ����
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
            // �̹� �� �ռ� �ɷ��� ������ �ִٸ� ��ŵ (�ߺ� ȹ�� ����)
            if (activeAbilities.ContainsKey(recipe.CombinedAbilityPrefab)) continue;

            bool canCombine = true;
            foreach (AbilityRecipe.RequiredAbility req in recipe.RequiredAbilities)
            {
                // �ʿ��� �ɷ��� �������� Ȱ��ȭ�� �ɷ� ��Ͽ� ���ų�, �䱸 ������ ��ġ�� ���ϸ� �ռ� �Ұ�
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
                // FireArrowAbility�� ��ȭ���� �߻��ϸ� true ��ȯ
                if (fireArrowAbility.TryActivateFireArrow(regularArrowGO, regularArrowScript))
                {
                    return true;
                }
            }
            // �ٸ� Ư�� ȭ�� �ɷ��� �ִٸ� ���⿡ �߰�
            // if (ability is <SomeOtherSpecialArrowAbility> otherAbility) { ... }
        }
        return false; // Ư�� ȭ�� �߻� ����
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

        // ������ ���� Ȯ��
        if (level < expToNextLevel.Length && experience >= expToNextLevel[level - 1])
        {
            LevelUp();
        }
    }

    /// <summary>
    /// �÷��̾ ������ ��Ű�� ���� ������ ������ŵ�ϴ�.
    /// </summary>
    private void LevelUp()
    {
        level++;
        experience = 0; // ����ġ �ʱ�ȭ (�Ǵ� ���� �������� ���� ����ġ�� ����)
        Debug.Log($"������! ���� ����: {level}");

        // �������� ���� �⺻ ���� ���� (���� ����)
        maxHealth += 10; // �ִ� ü�� ����
        Health = maxHealth; // ���� ü���� ������ �ִ� ü������ ���� (ȸ�� ȿ��)
        baseAttackDamage += 1; // �⺻ ���ݷ� ����
        baseMoveSpeed += 0.1f; // �⺻ �̵� �ӵ� ����

        RecalculateStats(); // ��� ���� ����

        // �ɷ� ���� UI�� ���� �̺�Ʈ �Ǵ� �޼��� ȣ�� (����)
        // AbilitySelectionUI.Instance.ShowAbilitySelection(); 
    }

    // --- ü�� ���� �޼��� (����) ---
    /// <summary>
    /// �÷��̾�� ���ظ� �����ϴ�.
    /// </summary>
    /// <param name="damageAmount">���� ���ط�.</param>
    public void TakeDamage(float damageAmount)
    {
        float finalDamage = Mathf.Max(0, damageAmount - Defense); // ���� ����
        Health -= finalDamage;
        Debug.Log($"���ظ� �޾ҽ��ϴ�: {finalDamage}. ���� ü��: {Health}");

        if (Health <= 0)
        {
            Die(); // �÷��̾� ��� ó��
        }
    }

    /// <summary>
    /// �÷��̾��� ü���� ȸ����ŵ�ϴ�.
    /// </summary>
    /// <param name="healAmount">ȸ���� ü�� ��.</param>
    public void Heal(float healAmount)
    {
        Health = Mathf.Min(maxHealth, Health + healAmount);
        Debug.Log($"ü�� ȸ��: {healAmount}. ���� ü��: {Health}");
    }

    /// <summary>
    /// �÷��̾� ��� ó�� ���� (�ʿ�� ����).
    /// </summary>
    private void Die()
    {
        Debug.Log("�÷��̾ ����߽��ϴ�!");
        // ���� ���� ó��, UI ǥ�� ��
        // Time.timeScale = 0f; // ���� �Ͻ� ���� (����)
    }
}