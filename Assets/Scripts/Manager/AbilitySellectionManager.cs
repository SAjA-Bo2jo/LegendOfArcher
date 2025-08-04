using System.Collections;
using System.Collections.Generic;
using System.Linq; // LINQ를 사용하기 위해 필요 (랜덤 선택 등)
using TMPro; // TextMeshPro를 사용하기 위해 필요
using UnityEngine;
using UnityEngine.UI; // UI 요소를 사용하기 위해 필요

public class AbilitySelectionManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject abilitySelectionPanel; // 어빌리티 선택창 전체 UI 패널 (LevelUpUI)
    [SerializeField] private AbilitySlotUI[] abilitySlots; // 각 어빌리티 슬롯 UI (Choice1, Choice2, Choice3)

    [Header("Ability Pool")]
    // 모든 어빌리티 프리팹 목록. 인스펙터에서 할당
    [SerializeField] private List<GameObject> allAvailableAbilityPrefabs;

    [Header("Probability Settings")]
    [Tooltip("기존 어빌리티 레벨업 확률 (0-1 사이의 값). 나머지는 새로운 어빌리티.")]
    [SerializeField] private float existingAbilityUpgradeChance = 0.6f;
    [Tooltip("합성 어빌리티가 나올 확률 (0-1 사이의 값). 다른 확률보다 우선 적용.")]
    [SerializeField] private float combineAbilityChance = 0.3f;

    private Player player; // 플레이어 참조

    void Awake()
    {
        // 씬에서 플레이어 컴포넌트를 찾습니다.
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError("Player 컴포넌트를 씬에서 찾을 수 없습니다! 플레이어가 씬에 있는지 확인하세요.");
        }

        // 초기에는 선택창을 비활성화합니다.
        if (abilitySelectionPanel != null)
        {
            abilitySelectionPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 플레이어 레벨업 시 호출되어 어빌리티 선택 창을 표시합니다.
    /// </summary>
    public void ShowAbilitySelection()
    {
        // 게임 일시 정지
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

    /// <summary>
    /// 어빌리티 선택 창을 숨기고 게임을 다시 시작합니다.
    /// </summary>
    public void HideAbilitySelection()
    {
        if (abilitySelectionPanel != null)
        {
            abilitySelectionPanel.SetActive(false);
        }
        Time.timeScale = 1f; // 게임 다시 시작
    }

    /// <summary>
    /// 어빌리티 선택지 3개를 생성하여 UI에 표시합니다.
    /// </summary>
    private void GenerateAbilityChoices()
    {
        // 모든 슬롯을 초기화하고 비활성화합니다. (선택된 것만 활성화)
        foreach (var slot in abilitySlots)
        {
            slot.ClearSlot();
        }

        List<GameObject> possibleChoices = new List<GameObject>(); // 이번 턴에 제공될 수 있는 후보 목록
        // 어빌리티 선택 로직을 개선하여 중복을 피하고 다양한 선택지를 제공합니다.
        // 최대 3개의 고유한 어빌리티를 선택하려고 시도합니다.
        for (int i = 0; i < abilitySlots.Length; i++)
        {
            GameObject selectedAbilityPrefab = SelectRandomAbilityUnique(possibleChoices);
            if (selectedAbilityPrefab != null)
            {
                Ability tempAbility = selectedAbilityPrefab.GetComponent<Ability>();
                if (tempAbility != null)
                {
                    // UI 슬롯에 어빌리티 정보를 설정합니다.
                    abilitySlots[i].SetAbility(selectedAbilityPrefab, tempAbility.AbilityName, tempAbility.Description, tempAbility.AbilityIcon);
                    possibleChoices.Add(selectedAbilityPrefab); // 선택된 어빌리티를 후보 목록에 추가하여 다음 선택에서 제외
                }
                else
                {
                    Debug.LogWarning($"프리팹 '{selectedAbilityPrefab.name}'에 Ability 컴포넌트가 없습니다.");
                    abilitySlots[i].ClearSlot();
                }
            }
            else
            {
                // 더 이상 선택할 어빌리티가 없는 경우 해당 슬롯은 비활성화 상태 유지
                abilitySlots[i].ClearSlot();
            }
        }
    }

    /// <summary>
    /// 확률에 따라 어빌리티를 선택하고, 이미 선택된 어빌리티는 제외합니다.
    /// </summary>
    /// <param name="excludeList">이미 선택된 어빌리티 프리팹 목록.</param>
    /// <returns>선택된 어빌리티의 프리팹 GameObject.</returns>
    private GameObject SelectRandomAbilityUnique(List<GameObject> excludeList)
    {
        // 1. 플레이어가 이미 보유하고 있는 어빌리티 중에서 강화 가능한 목록을 찾습니다.
        List<GameObject> upgradableAbilities = player.activeAbilities
            .Where(entry => entry.Value.CurrentLevel < entry.Value.MaxLevel && !excludeList.Contains(entry.Key))
            .Select(entry => entry.Key)
            .ToList();

        // 2. 플레이어가 아직 보유하지 않은 어빌리티 목록을 찾습니다.
        List<GameObject> newAbilities = allAvailableAbilityPrefabs
            .Where(prefab => !player.activeAbilities.ContainsKey(prefab) && !excludeList.Contains(prefab))
            .ToList();

        // 3. 합성 가능한 어빌리티 목록을 가져옵니다.
        List<GameObject> combinableAbilities = player.GetCombinableAbilities()
            .Where(prefab => !excludeList.Contains(prefab))
            .ToList();

        // 4. 어떤 유형의 어빌리티를 제공할지 확률적으로 결정합니다.
        // 우선순위: 합성 > 강화 > 신규 (확률 기반)
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
        // 위 확률에 맞지 않거나 해당 유형의 어빌리티가 없을 경우, 남은 어빌리티 중 아무거나 시도
        else if (upgradableAbilities.Any())
        {
            return upgradableAbilities[Random.Range(0, upgradableAbilities.Count)];
        }
        else if (combinableAbilities.Any())
        {
            return combinableAbilities[Random.Range(0, combinableAbilities.Count)];
        }
        else if (newAbilities.Any()) // 마지막으로 새로운 어빌리티라도 있다면
        {
            return newAbilities[Random.Range(0, newAbilities.Count)];
        }
        else
        {
            Debug.LogWarning("제공할 수 있는 어빌리티가 없습니다. (모든 어빌리티가 최대 레벨이거나 풀에 없음)");
            return null;
        }
    }

    /// <summary>
    /// 어빌리티 슬롯에서 선택 버튼을 클릭했을 때 호출됩니다.
    /// </summary>
    /// <param name="selectedAbilityPrefab">선택된 어빌리티의 프리팹 GameObject.</param>
    public void OnAbilitySelected(GameObject selectedAbilityPrefab)
    {
        if (player == null) return;

        // 선택된 어빌리티가 합성 어빌리티인지 확인합니다.
        AbilityRecipe selectedRecipe = player.abilityRecipes.FirstOrDefault(
            r => r.CombinedAbilityPrefab == selectedAbilityPrefab);

        if (selectedRecipe != null)
        {
            // 합성 어빌리티인 경우
            Debug.Log($"합성 어빌리티 [{selectedAbilityPrefab.name}] 선택!");

            // 기존 원료 어빌리티 제거
            foreach (var req in selectedRecipe.RequiredAbilities)
            {
                player.RemoveAbility(req.AbilityPrefab);
            }
            // 합성된 어빌리티 획득
            player.AcquireAbility(selectedAbilityPrefab);
        }
        else
        {
            // 일반 어빌리티 (강화 또는 신규 획득)
            player.AcquireAbility(selectedAbilityPrefab);
        }

        HideAbilitySelection(); // 선택창 숨기기
    }
}