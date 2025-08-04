using System.Collections;
using System.Collections.Generic;
using System.Linq; // LINQ�� ����ϱ� ���� �ʿ� (���� ���� ��)
using TMPro; // TextMeshPro�� ����ϱ� ���� �ʿ�
using UnityEngine;
using UnityEngine.UI; // UI ��Ҹ� ����ϱ� ���� �ʿ�

public class AbilitySelectionManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject abilitySelectionPanel; // �����Ƽ ����â ��ü UI �г� (LevelUpUI)
    [SerializeField] private AbilitySlotUI[] abilitySlots; // �� �����Ƽ ���� UI (Choice1, Choice2, Choice3)

    [Header("Ability Pool")]
    // ��� �����Ƽ ������ ���. �ν����Ϳ��� �Ҵ�
    [SerializeField] private List<GameObject> allAvailableAbilityPrefabs;

    [Header("Probability Settings")]
    [Tooltip("���� �����Ƽ ������ Ȯ�� (0-1 ������ ��). �������� ���ο� �����Ƽ.")]
    [SerializeField] private float existingAbilityUpgradeChance = 0.6f;
    [Tooltip("�ռ� �����Ƽ�� ���� Ȯ�� (0-1 ������ ��). �ٸ� Ȯ������ �켱 ����.")]
    [SerializeField] private float combineAbilityChance = 0.3f;

    private Player player; // �÷��̾� ����

    void Awake()
    {
        // ������ �÷��̾� ������Ʈ�� ã���ϴ�.
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError("Player ������Ʈ�� ������ ã�� �� �����ϴ�! �÷��̾ ���� �ִ��� Ȯ���ϼ���.");
        }

        // �ʱ⿡�� ����â�� ��Ȱ��ȭ�մϴ�.
        if (abilitySelectionPanel != null)
        {
            abilitySelectionPanel.SetActive(false);
        }
    }

    /// <summary>
    /// �÷��̾� ������ �� ȣ��Ǿ� �����Ƽ ���� â�� ǥ���մϴ�.
    /// </summary>
    public void ShowAbilitySelection()
    {
        // ���� �Ͻ� ����
        Time.timeScale = 0f;

        if (abilitySelectionPanel != null)
        {
            abilitySelectionPanel.SetActive(true);
            GenerateAbilityChoices();
        }
        else
        {
            Debug.LogError("Ability Selection Panel�� �Ҵ���� �ʾҽ��ϴ�!");
        }
    }

    /// <summary>
    /// �����Ƽ ���� â�� ����� ������ �ٽ� �����մϴ�.
    /// </summary>
    public void HideAbilitySelection()
    {
        if (abilitySelectionPanel != null)
        {
            abilitySelectionPanel.SetActive(false);
        }
        Time.timeScale = 1f; // ���� �ٽ� ����
    }

    /// <summary>
    /// �����Ƽ ������ 3���� �����Ͽ� UI�� ǥ���մϴ�.
    /// </summary>
    private void GenerateAbilityChoices()
    {
        // ��� ������ �ʱ�ȭ�ϰ� ��Ȱ��ȭ�մϴ�. (���õ� �͸� Ȱ��ȭ)
        foreach (var slot in abilitySlots)
        {
            slot.ClearSlot();
        }

        List<GameObject> possibleChoices = new List<GameObject>(); // �̹� �Ͽ� ������ �� �ִ� �ĺ� ���
        // �����Ƽ ���� ������ �����Ͽ� �ߺ��� ���ϰ� �پ��� �������� �����մϴ�.
        // �ִ� 3���� ������ �����Ƽ�� �����Ϸ��� �õ��մϴ�.
        for (int i = 0; i < abilitySlots.Length; i++)
        {
            GameObject selectedAbilityPrefab = SelectRandomAbilityUnique(possibleChoices);
            if (selectedAbilityPrefab != null)
            {
                Ability tempAbility = selectedAbilityPrefab.GetComponent<Ability>();
                if (tempAbility != null)
                {
                    // UI ���Կ� �����Ƽ ������ �����մϴ�.
                    abilitySlots[i].SetAbility(selectedAbilityPrefab, tempAbility.AbilityName, tempAbility.Description, tempAbility.AbilityIcon);
                    possibleChoices.Add(selectedAbilityPrefab); // ���õ� �����Ƽ�� �ĺ� ��Ͽ� �߰��Ͽ� ���� ���ÿ��� ����
                }
                else
                {
                    Debug.LogWarning($"������ '{selectedAbilityPrefab.name}'�� Ability ������Ʈ�� �����ϴ�.");
                    abilitySlots[i].ClearSlot();
                }
            }
            else
            {
                // �� �̻� ������ �����Ƽ�� ���� ��� �ش� ������ ��Ȱ��ȭ ���� ����
                abilitySlots[i].ClearSlot();
            }
        }
    }

    /// <summary>
    /// Ȯ���� ���� �����Ƽ�� �����ϰ�, �̹� ���õ� �����Ƽ�� �����մϴ�.
    /// </summary>
    /// <param name="excludeList">�̹� ���õ� �����Ƽ ������ ���.</param>
    /// <returns>���õ� �����Ƽ�� ������ GameObject.</returns>
    private GameObject SelectRandomAbilityUnique(List<GameObject> excludeList)
    {
        // 1. �÷��̾ �̹� �����ϰ� �ִ� �����Ƽ �߿��� ��ȭ ������ ����� ã���ϴ�.
        List<GameObject> upgradableAbilities = player.activeAbilities
            .Where(entry => entry.Value.CurrentLevel < entry.Value.MaxLevel && !excludeList.Contains(entry.Key))
            .Select(entry => entry.Key)
            .ToList();

        // 2. �÷��̾ ���� �������� ���� �����Ƽ ����� ã���ϴ�.
        List<GameObject> newAbilities = allAvailableAbilityPrefabs
            .Where(prefab => !player.activeAbilities.ContainsKey(prefab) && !excludeList.Contains(prefab))
            .ToList();

        // 3. �ռ� ������ �����Ƽ ����� �����ɴϴ�.
        List<GameObject> combinableAbilities = player.GetCombinableAbilities()
            .Where(prefab => !excludeList.Contains(prefab))
            .ToList();

        // 4. � ������ �����Ƽ�� �������� Ȯ�������� �����մϴ�.
        // �켱����: �ռ� > ��ȭ > �ű� (Ȯ�� ���)
        float randomValue = Random.value;

        if (combinableAbilities.Any() && randomValue < combineAbilityChance)
        {
            return combinableAbilities[Random.Range(0, combinableAbilities.Count)];
        }
        else if (upgradableAbilities.Any() && randomValue < existingAbilityUpgradeChance)
        {
            return upgradableAbilities[Random.Range(0, upgradableAbilities.Count)];
        }
        else if (newAbilities.Any())
        {
            return newAbilities[Random.Range(0, newAbilities.Count)];
        }
        // �� Ȯ���� ���� �ʰų� �ش� ������ �����Ƽ�� ���� ���, ���� �����Ƽ �� �ƹ��ų� �õ�
        else if (upgradableAbilities.Any())
        {
            return upgradableAbilities[Random.Range(0, upgradableAbilities.Count)];
        }
        else if (combinableAbilities.Any())
        {
            return combinableAbilities[Random.Range(0, combinableAbilities.Count)];
        }
        else if (newAbilities.Any()) // ���������� ���ο� �����Ƽ�� �ִٸ�
        {
            return newAbilities[Random.Range(0, newAbilities.Count)];
        }
        else
        {
            Debug.LogWarning("������ �� �ִ� �����Ƽ�� �����ϴ�. (��� �����Ƽ�� �ִ� �����̰ų� Ǯ�� ����)");
            return null;
        }
    }

    /// <summary>
    /// �����Ƽ ���Կ��� ���� ��ư�� Ŭ������ �� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="selectedAbilityPrefab">���õ� �����Ƽ�� ������ GameObject.</param>
    public void OnAbilitySelected(GameObject selectedAbilityPrefab)
    {
        if (player == null) return;

        // ���õ� �����Ƽ�� �ռ� �����Ƽ���� Ȯ���մϴ�.
        AbilityRecipe selectedRecipe = player.abilityRecipes.FirstOrDefault(
            r => r.CombinedAbilityPrefab == selectedAbilityPrefab);

        if (selectedRecipe != null)
        {
            // �ռ� �����Ƽ�� ���
            Debug.Log($"�ռ� �����Ƽ [{selectedAbilityPrefab.name}] ����!");

            // ���� ���� �����Ƽ ����
            foreach (var req in selectedRecipe.RequiredAbilities)
            {
                player.RemoveAbility(req.AbilityPrefab);
            }
            // �ռ��� �����Ƽ ȹ��
            player.AcquireAbility(selectedAbilityPrefab);
        }
        else
        {
            // �Ϲ� �����Ƽ (��ȭ �Ǵ� �ű� ȹ��)
            player.AcquireAbility(selectedAbilityPrefab);
        }

        HideAbilitySelection(); // ����â �����
    }
}