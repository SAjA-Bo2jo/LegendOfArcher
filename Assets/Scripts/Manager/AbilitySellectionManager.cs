// AbilitySelectionManager.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelectionManager : MonoBehaviour
{
    // --- �̱��� ���� ---
    public static AbilitySelectionManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject abilitySelectionPanel; // UI Panel Game Object
    [SerializeField] private AbilitySlotUI[] abilitySlots;

    [Header("Ability Pool")]
    [SerializeField] private List<GameObject> allAvailableAbilityPrefabs;

    [Header("Probability Settings")]
    [Tooltip("���� �����Ƽ ������ Ȯ�� (0-1 ������ ��). �������� ���ο� �����Ƽ.")]
    [SerializeField] private float existingAbilityUpgradeChance = 0.6f;
    [Tooltip("�ռ� �����Ƽ�� ���� Ȯ�� (0-1 ������ ��). �ٸ� Ȯ������ �켱 ����.")]
    [SerializeField] private float combineAbilityChance = 0.3f;

    [SerializeField] private Player player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (abilitySelectionPanel != null)
        {
            abilitySelectionPanel.SetActive(false);
        }
    }

    public void ShowAbilitySelection()
    {
        if (player == null)
        {
            if (StageManager.Instance != null && StageManager.Instance._Player != null)
            {
                player = StageManager.Instance._Player.GetComponent<Player>();
            }
            else
            {
                Debug.LogError("StageManager�� �÷��̾� �ν��Ͻ��� ���ų� Player ��ũ��Ʈ�� ã�� �� �����ϴ�.");
                return;
            }
        }

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

    public void HideAbilitySelection()
    {
        if (abilitySelectionPanel != null)
        {
            abilitySelectionPanel.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    private void GenerateAbilityChoices()
    {
        if (player == null)
        {
            Debug.LogError("Player �ν��Ͻ��� �������� �ʽ��ϴ�!");
            return;
        }

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
                    Debug.Log($"[����׷α�] ���õ� �����Ƽ: {tempAbility.AbilityName}, ����: {tempAbility.Description}, ������: {tempAbility.AbilityIcon?.name}");
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

    private GameObject SelectRandomAbilityUnique(List<GameObject> excludeList)
    {
        // 1. �ռ� ���� �����Ƽ ���
        List<GameObject> combinableAbilities = player.GetCombinableAbilities()
            .Where(prefab => prefab != null && !excludeList.Contains(prefab))
            .ToList();

        // 2. ���׷��̵� ���� �����Ƽ ���
        List<GameObject> upgradableAbilities = player.activeAbilities.Values
            .Where(ability => ability != null && ability.CurrentLevel < ability.MaxLevel && !excludeList.Contains(ability.AbilityPrefab))
            .Select(ability => ability.AbilityPrefab)
            .ToList();

        // 3. ���ο� �����Ƽ ���
        List<GameObject> newAbilities = allAvailableAbilityPrefabs
            .Where(prefab => prefab != null && !player.activeAbilities.ContainsKey(prefab) && !excludeList.Contains(prefab))
            .ToList();

        float randomValue = Random.value;

        // �켱������ ���� �����Ƽ ����
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
        else if (upgradableAbilities.Any()) // ���ο� �����Ƽ�� ���� ��� ���׷��̵� ��ȸ ����
        {
            return upgradableAbilities[Random.Range(0, upgradableAbilities.Count)];
        }
        else if (combinableAbilities.Any()) // ���׷��̵��� ���� ���� ��� �ռ� ��ȸ ����
        {
            return combinableAbilities[Random.Range(0, combinableAbilities.Count)];
        }
        else
        {
            Debug.LogWarning("������ �� �ִ� �����Ƽ�� �����ϴ�. (��� �����Ƽ�� �ִ� �����̰ų� Ǯ�� ����)");
            return null;
        }
    }

    public void OnAbilitySelected(GameObject selectedAbilityPrefab)
    {
        if (player == null || selectedAbilityPrefab == null)
        {
            Debug.LogWarning("�÷��̾� �Ǵ� ���õ� �����Ƽ �������� ��ȿ���� �ʽ��ϴ�.");
            return;
        }

        AbilityRecipe selectedRecipe = player.abilityRecipes.FirstOrDefault(
            r => r != null && r.CombinedAbilityPrefab == selectedAbilityPrefab);

        if (selectedRecipe != null)
        {
            Debug.Log($"�ռ� �����Ƽ [{selectedAbilityPrefab.name}] ����!");
            foreach (var req in selectedRecipe.RequiredAbilities)
            {
                if (req.AbilityPrefab != null && player.activeAbilities.ContainsKey(req.AbilityPrefab))
                {
                    player.RemoveAbility(req.AbilityPrefab);
                }
            }
        }
        else
        {
            Debug.Log($"�Ϲ�/������ �����Ƽ [{selectedAbilityPrefab.name}] ����!");
        }

        player.AcquireAbility(selectedAbilityPrefab);
        player.RecalculateStats();
        HideAbilitySelection();
    }

    private bool IsCombinedAbility(GameObject prefab)
    {
        if (player.abilityRecipes == null) return false;
        return player.abilityRecipes.Any(r => r != null && r.CombinedAbilityPrefab == prefab);
    }
}