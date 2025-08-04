using System.Collections;
using System.Collections.Generic;
using System.Linq; // LINQ�� ����ϱ� ���� �ʿ�
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelectionManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject abilitySelectionPanel;
    [SerializeField] private AbilitySlotUI[] abilitySlots;

    [Header("Ability Pool")]
    [SerializeField] private List<GameObject> allAvailableAbilityPrefabs;

    [Header("Probability Settings")]
    [Tooltip("���� �����Ƽ ������ Ȯ�� (0-1 ������ ��). �������� ���ο� �����Ƽ.")]
    [SerializeField] private float existingAbilityUpgradeChance = 0.6f;
    [Tooltip("�ռ� �����Ƽ�� ���� Ȯ�� (0-1 ������ ��). �ٸ� Ȯ������ �켱 ����.")]
    [SerializeField] private float combineAbilityChance = 0.3f;

    private Player player;

    void Awake()
    {
        player = FindObjectOfType<Player>();

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
        Time.timeScale = 1f;
    }

    /// <summary>
    /// �����Ƽ ������ 3���� �����Ͽ� UI�� ǥ���մϴ�.
    /// </summary>
    private void GenerateAbilityChoices()
    {
        foreach (var slot in abilitySlots)
        {
            slot.ClearSlot();
        }

        List<GameObject> possibleChoices = new List<GameObject>();
        for (int i = 0; i < abilitySlots.Length; i++)
        {
            GameObject selectedAbilityPrefab = SelectRandomAbilityUnique(possibleChoices);
            if (selectedAbilityPrefab != null)
            {
                Ability tempAbility = selectedAbilityPrefab.GetComponent<Ability>();
                if (tempAbility != null)
                {
                    abilitySlots[i].SetAbility(selectedAbilityPrefab, tempAbility.AbilityName, tempAbility.Description, tempAbility.AbilityIcon);
                    possibleChoices.Add(selectedAbilityPrefab);
                }
                else
                {
                    Debug.LogWarning($"������ '{selectedAbilityPrefab.name}'�� Ability ������Ʈ�� �����ϴ�.");
                    abilitySlots[i].ClearSlot();
                }
            }
            else
            {
                abilitySlots[i].ClearSlot();
            }
        }
    }

    /// <summary>
    /// Ȯ���� ���� �����Ƽ�� �����ϰ�, �̹� ���õ� �����Ƽ�� �����մϴ�.
    /// </summary>
    private GameObject SelectRandomAbilityUnique(List<GameObject> excludeList)
    {
        List<GameObject> upgradableAbilities = player.activeAbilities
            .Where(entry => entry.Value.CurrentLevel < entry.Value.MaxLevel && !excludeList.Contains(entry.Key))
            .Select(entry => entry.Key)
            .ToList();

        List<GameObject> newAbilities = allAvailableAbilityPrefabs
            .Where(prefab => !player.activeAbilities.ContainsKey(prefab) && !excludeList.Contains(prefab))
            .ToList();

        List<GameObject> combinableAbilities = player.GetCombinableAbilities()
            .Where(prefab => !excludeList.Contains(prefab))
            .ToList();

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
        else if (upgradableAbilities.Any())
        {
            return upgradableAbilities[Random.Range(0, upgradableAbilities.Count)];
        }
        else if (combinableAbilities.Any())
        {
            return combinableAbilities[Random.Range(0, combinableAbilities.Count)];
        }
        else if (newAbilities.Any())
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
    public void OnAbilitySelected(GameObject selectedAbilityPrefab)
    {
        if (player == null) return;

        // ���õ� �����Ƽ �����տ� �ش��ϴ� �����Ǹ� ã���ϴ�.
        // ���� Recipe.CombinedAbilityPrefab�� ���մϴ�.
        AbilityRecipe selectedRecipe = player.abilityRecipes.FirstOrDefault(
            r => r.CombinedAbilityPrefab == selectedAbilityPrefab);

        if (selectedRecipe != null)
        {
            Debug.Log($"�ռ� �����Ƽ [{selectedAbilityPrefab.name}] ����!");

            foreach (var req in selectedRecipe.RequiredAbilities)
            {
                player.RemoveAbility(req.AbilityPrefab);
            }
            player.AcquireAbility(selectedAbilityPrefab);
        }
        else
        {
            player.AcquireAbility(selectedAbilityPrefab);
        }

        player.RecalculateStats();

        HideAbilitySelection();
    }
}