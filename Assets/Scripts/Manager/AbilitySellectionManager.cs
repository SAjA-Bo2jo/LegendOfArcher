// AbilitySelectionManager.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelectionManager : MonoBehaviour
{
    // --- 싱글톤 패턴 ---
    public static AbilitySelectionManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject abilitySelectionPanel; // UI Panel Game Object
    [SerializeField] private AbilitySlotUI[] abilitySlots;

    [Header("Ability Pool")]
    [SerializeField] private List<GameObject> allAvailableAbilityPrefabs;

    [Header("Probability Settings")]
    [Tooltip("기존 어빌리티 레벨업 확률 (0-1 사이의 값). 나머지는 새로운 어빌리티.")]
    [SerializeField] private float existingAbilityUpgradeChance = 0.6f;
    [Tooltip("합성 어빌리티가 나올 확률 (0-1 사이의 값). 다른 확률보다 우선 적용.")]
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
                Debug.LogError("StageManager에 플레이어 인스턴스가 없거나 Player 스크립트를 찾을 수 없습니다.");
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
            Debug.LogError("Ability Selection Panel이 할당되지 않았습니다!");
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
            Debug.LogError("Player 인스턴스가 존재하지 않습니다!");
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
                    Debug.Log($"[디버그로그] 선택된 어빌리티: {tempAbility.AbilityName}, 설명: {tempAbility.Description}, 아이콘: {tempAbility.AbilityIcon?.name}");
                    abilitySlots[i].SetAbility(selectedAbilityPrefab, tempAbility.AbilityName, tempAbility.Description, tempAbility.AbilityIcon);
                    possibleChoices.Add(selectedAbilityPrefab);
                }
                else
                {
                    Debug.LogWarning($"프리팹 '{selectedAbilityPrefab.name}'에 Ability 컴포넌트가 없습니다.");
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
        // 1. 합성 가능 어빌리티 목록
        List<GameObject> combinableAbilities = player.GetCombinableAbilities()
            .Where(prefab => prefab != null && !excludeList.Contains(prefab))
            .ToList();

        // 2. 업그레이드 가능 어빌리티 목록
        List<GameObject> upgradableAbilities = player.activeAbilities.Values
            .Where(ability => ability != null && ability.CurrentLevel < ability.MaxLevel && !excludeList.Contains(ability.AbilityPrefab))
            .Select(ability => ability.AbilityPrefab)
            .ToList();

        // 3. 새로운 어빌리티 목록
        List<GameObject> newAbilities = allAvailableAbilityPrefabs
            .Where(prefab => prefab != null && !player.activeAbilities.ContainsKey(prefab) && !excludeList.Contains(prefab))
            .ToList();

        float randomValue = Random.value;

        // 우선순위에 따라 어빌리티 선택
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
        else if (upgradableAbilities.Any()) // 새로운 어빌리티가 없을 경우 업그레이드 기회 제공
        {
            return upgradableAbilities[Random.Range(0, upgradableAbilities.Count)];
        }
        else if (combinableAbilities.Any()) // 업그레이드할 것이 없을 경우 합성 기회 제공
        {
            return combinableAbilities[Random.Range(0, combinableAbilities.Count)];
        }
        else
        {
            Debug.LogWarning("제공할 수 있는 어빌리티가 없습니다. (모든 어빌리티가 최대 레벨이거나 풀에 없음)");
            return null;
        }
    }

    public void OnAbilitySelected(GameObject selectedAbilityPrefab)
    {
        if (player == null || selectedAbilityPrefab == null)
        {
            Debug.LogWarning("플레이어 또는 선택된 어빌리티 프리팹이 유효하지 않습니다.");
            return;
        }

        AbilityRecipe selectedRecipe = player.abilityRecipes.FirstOrDefault(
            r => r != null && r.CombinedAbilityPrefab == selectedAbilityPrefab);

        if (selectedRecipe != null)
        {
            Debug.Log($"합성 어빌리티 [{selectedAbilityPrefab.name}] 선택!");
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
            Debug.Log($"일반/레벨업 어빌리티 [{selectedAbilityPrefab.name}] 선택!");
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