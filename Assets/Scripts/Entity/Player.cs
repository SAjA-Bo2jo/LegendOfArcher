using System.Collections.Generic;
using UnityEngine;
using System.Linq; // LINQ ����� ���� �߰�

public class Player : MonoBehaviour
{
    // --- ���� ���� �� �ʵ� (����) ---
    public float AttackDamage { get; set; } = 10f;
    public float AttackSize { get; set; } = 1f;
    public float CriticalRate { get; set; } = 10f; // Ȯ�� %
    public float ProjectileSpeed { get; set; } = 10f;
    public float AttackRange { get; set; } = 5f;

    // �⺻ ���� �ӵ� (����: �ʴ� ���� Ƚ��)
    public float BaseAttackSpeed = 1.0f;
    // ���� �ӵ� ���� (��: 100 = 100%, 120 = 120%)
    public float AttackSpeedMultiplier { get; set; } = 100f;
    // ���� ���� ���� �ӵ�
    public float MaxAttackSpeed => BaseAttackSpeed * (AttackSpeedMultiplier / 100f);

    // --- �ɷ� ���� �ʵ� ---
    // ȹ���� �ɷµ��� ������ ��ųʸ�: <�ɷ� ������ (Key), �ش� �ɷ� �ν��Ͻ� (Value)>
    // �̸� ���� ���� �ɷ��� ������� ���� Ȯ���ϰ�, �ν��Ͻ��� ������ �� �ֽ��ϴ�.
    public Dictionary<GameObject, Abillity> activeAbilities = new Dictionary<GameObject, Abillity>();

    [Header("�ɷ� �ռ� ������")]
    // �ռ� ������ ��� (Unity �����Ϳ��� AbilityRecipe ScriptableObject ���µ��� �Ҵ�)
    public List<AbilityRecipe> abilityRecipes;

    // --- ��Ÿ �÷��̾� ���� �ʵ� (��: ü��, �̵� �ӵ� ��) ---
    public float Speed = 5f; // PlayerController���� �����ϴ� �̵� �ӵ�

    void Awake()
    {
        // ���� �� �ʱ� ���� ���� (�ʿ��)
        AttackSpeedMultiplier = 100f; // �⺻������ �ʱ�ȭ

        // Debug.Log("Player Awake");
        // if (abilityRecipes == null || abilityRecipes.Count == 0)
        // {
        //     Debug.LogWarning("Player: Ability Recipes�� �Ҵ���� �ʾҽ��ϴ�. �ռ� ����� �۵����� ���� �� �ֽ��ϴ�.");
        // }
    }

    // ������ �� ȣ��� �ɷ� ȹ�� �޼���
    public void AcquireAbility(GameObject abilityPrefab) // ���õ� �ɷ��� ������
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

        // �ɷ� ȹ�� �� �ռ� ������ �ɷ��� �ִ��� Ȯ�� (�� �޼���� �ɷ� ���� UI���� ȣ��� �� ����)
        // CheckForCombinations(); // �� ������ �ɷ� ���� UI�� ������ ���� ȣ���ϴ� ���� �� ������ �� �ֽ��ϴ�.
    }

    /// <summary>
    /// Ȱ��ȭ�� �ɷµ� �� Ư�� �ɷ��� �����մϴ�. (�ַ� �ռ� �� ���� �ɷ� ���ſ� ���)
    /// </summary>
    /// <param name="abilityPrefab">������ �ɷ��� ������</param>
    public void RemoveAbility(GameObject abilityPrefab)
    {
        if (activeAbilities.TryGetValue(abilityPrefab, out Abillity abilityToRemove))
        {
            Debug.Log($"[{abilityToRemove.AbilityName}] �ɷ��� �����մϴ�.");
            abilityToRemove.OnRemove(); // �ɷ� ���� ȿ�� ȣ�� (��: ���� ����)
            Destroy(abilityToRemove.gameObject); // ���� ������Ʈ �ı�
            activeAbilities.Remove(abilityPrefab); // ��ųʸ����� ����
        }
        else
        {
            Debug.LogWarning($"�����Ϸ��� �ɷ� ������ {abilityPrefab.name}��(��) Ȱ��ȭ�� ��Ͽ��� ã�� �� �����ϴ�.");
        }
    }

    /// <summary>
    /// ���� Ȱ��ȭ�� �ɷµ�� ������ ������� �ռ� ������ �����Ǹ� ã���ϴ�.
    /// </summary>
    /// <returns>�ռ� ������ �ɷ��� ������ ���</returns>
    public List<GameObject> GetCombinableAbilities()
    {
        List<GameObject> combinableList = new List<GameObject>();

        if (abilityRecipes == null || abilityRecipes.Count == 0) return combinableList;

        foreach (AbilityRecipe recipe in abilityRecipes)
        {
            // �̹� �� �ռ� �ɷ��� ������ �ִٸ� ��ŵ
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

    // --- PlayerController���� Bow�� ������ ���� �Լ��� (Bow������ ���) ---
    // �÷��̾��� �ٶ󺸴� ����� �̵� ���´� PlayerController���� ���� �������Ƿ�,
    // �� Player Ŭ���������� ������ �������� �ʽ��ϴ�.

    // FireArrowAbility�� ���� ���Ǻ� �ߵ� �ɷ��� Bow�� Ȱ���� �� �ֵ���
    // Ȱ��ȭ�� �ɷ� ����� �ܺο� ���� (public ������Ƽ)
    // Ȥ�� private���� �����ϰ�, Bow���� Player�� Ư�� �޼��带 ȣ���ϵ��� �� ���� �ֽ��ϴ�.
    public IReadOnlyDictionary<GameObject, Abillity> GetActiveAbilities()
    {
        return activeAbilities;
    }

    // Bow�� ȭ���� �߻��ϱ� ������ �� �Լ��� ȣ���Ͽ� Ư�� ȭ�� �ߵ��� �õ�
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
        }
        return false; // Ư�� ȭ�� �߻� ����
    }
}