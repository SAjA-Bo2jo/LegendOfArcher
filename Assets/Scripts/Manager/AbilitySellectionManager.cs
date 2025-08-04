using System.Collections;
using System.Collections.Generic;
using System.Linq; // LINQ를 사용하기 위해 필요
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
    [Tooltip("기존 어빌리티 레벨업 확률 (0-1 사이의 값). 나머지는 새로운 어빌리티.")]
    [SerializeField] private float existingAbilityUpgradeChance = 0.6f;
    [Tooltip("합성 어빌리티가 나올 확률 (0-1 사이의 값). 다른 확률보다 우선 적용.")]
    [SerializeField] private float combineAbilityChance = 0.3f;

    [SerializeField] private Player player;

    void Awake()
    {
        // Unity 에디터에서 player가 할당되지 않은 경우, FindObjectOfType으로 찾습니다.
        if (player == null)
        {
            player = FindObjectOfType<Player>();
            if (player == null)
            {
                Debug.LogError("AbilitySelectionManager: Player 오브젝트를 찾을 수 없습니다! Unity Inspector에서 직접 할당해주세요.");
            }
        }

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
        // Player 변수가 null인지 확인하는 방어 코드 추가
        if (player == null)
        {
            Debug.LogError("Player 오브젝트가 없으므로 어빌리티 선택 창을 열 수 없습니다.");
            return;
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

    /// <summary>
    /// 어빌리티 선택 창을 숨기고 게임을 다시 시작합니다.
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
    /// 어빌리티 선택지 3개를 생성하여 UI에 표시합니다.
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
                    // 이 로그가 콘솔 창에 출력됩니다.
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

    /// <summary>
    /// 확률에 따라 어빌리티를 선택하고, 이미 선택된 어빌리티는 제외합니다.
    /// </summary>
    private GameObject SelectRandomAbilityUnique(List<GameObject> excludeList)
    {
        // 수정된 부분: player.activeAbilities의 value가 null인 경우를 대비한 방어 코드 추가
        List<GameObject> upgradableAbilities = player.activeAbilities
            .Where(entry => entry.Value != null && entry.Value.CurrentLevel < entry.Value.MaxLevel && !excludeList.Contains(entry.Key))
            .Select(entry => entry.Key)
            .ToList();

        // 수정된 부분: prefab이 null인 경우를 대비한 방어 코드 추가
        List<GameObject> newAbilities = allAvailableAbilityPrefabs
            .Where(prefab => prefab != null && !player.activeAbilities.ContainsKey(prefab) && !excludeList.Contains(prefab))
            .ToList();

        // GetCombinableAbilities() 메서드 내부에 null 체크가 필요할 수 있습니다.
        List<GameObject> combinableAbilities = player.GetCombinableAbilities()
            .Where(prefab => prefab != null && !excludeList.Contains(prefab))
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
            Debug.LogWarning("제공할 수 있는 어빌리티가 없습니다. (모든 어빌리티가 최대 레벨이거나 풀에 없음)");
            return null;
        }
    }

    /// <summary>
    /// 어빌리티 슬롯에서 선택 버튼을 클릭했을 때 호출됩니다.
    /// </summary>
    public void OnAbilitySelected(GameObject selectedAbilityPrefab)
    {
        if (player == null) return;
        if (selectedAbilityPrefab == null) return; // 추가된 방어 코드

        // 선택된 어빌리티 프리팹에 해당하는 레시피를 찾습니다.
        AbilityRecipe selectedRecipe = player.abilityRecipes.FirstOrDefault(
            r => r.CombinedAbilityPrefab == selectedAbilityPrefab);

        if (selectedRecipe != null)
        {
            Debug.Log($"합성 어빌리티 [{selectedAbilityPrefab.name}] 선택!");

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