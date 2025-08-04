using System.Collections.Generic;
using UnityEngine;
using System.Linq; // LINQ 사용을 위해 추가

public class Player : MonoBehaviour
{
    // --- 기존 스탯 및 필드 (예시) ---
    public float AttackDamage { get; set; } = 10f;
    public float AttackSize { get; set; } = 1f;
    public float CriticalRate { get; set; } = 10f; // 확률 %
    public float ProjectileSpeed { get; set; } = 10f;
    public float AttackRange { get; set; } = 5f;

    // 기본 공격 속도 (단위: 초당 공격 횟수)
    public float BaseAttackSpeed = 1.0f;
    // 공격 속도 배율 (예: 100 = 100%, 120 = 120%)
    public float AttackSpeedMultiplier { get; set; } = 100f;
    // 최종 계산된 공격 속도
    public float MaxAttackSpeed => BaseAttackSpeed * (AttackSpeedMultiplier / 100f);

    // --- 능력 관리 필드 ---
    // 획득한 능력들을 저장할 딕셔너리: <능력 프리팹 (Key), 해당 능력 인스턴스 (Value)>
    // 이를 통해 같은 능력을 얻었는지 쉽게 확인하고, 인스턴스에 접근할 수 있습니다.
    public Dictionary<GameObject, Abillity> activeAbilities = new Dictionary<GameObject, Abillity>();

    [Header("능력 합성 레시피")]
    // 합성 레시피 목록 (Unity 에디터에서 AbilityRecipe ScriptableObject 에셋들을 할당)
    public List<AbilityRecipe> abilityRecipes;

    // --- 기타 플레이어 관련 필드 (예: 체력, 이동 속도 등) ---
    public float Speed = 5f; // PlayerController에서 참조하는 이동 속도

    void Awake()
    {
        // 시작 시 초기 스탯 설정 (필요시)
        AttackSpeedMultiplier = 100f; // 기본값으로 초기화

        // Debug.Log("Player Awake");
        // if (abilityRecipes == null || abilityRecipes.Count == 0)
        // {
        //     Debug.LogWarning("Player: Ability Recipes가 할당되지 않았습니다. 합성 기능이 작동하지 않을 수 있습니다.");
        // }
    }

    // 레벨업 시 호출될 능력 획득 메서드
    public void AcquireAbility(GameObject abilityPrefab) // 선택된 능력의 프리팹
    {
        Abillity existingAbility = null;
        // 이미 이 능력을 가지고 있는지 확인
        if (activeAbilities.TryGetValue(abilityPrefab, out existingAbility))
        {
            // 이미 능력을 가지고 있고, 최대 레벨이 아니라면 레벨업 시도
            if (existingAbility.CurrentLevel < existingAbility.MaxLevel)
            {
                existingAbility.OnAcquire(this); // 플레이어 인스턴스를 전달하여 레벨업 및 효과 적용
                Debug.Log($"[{existingAbility.AbilityName}] 능력이 레벨업! (Lv.{existingAbility.CurrentLevel})");
            }
            else
            {
                Debug.Log($"[{existingAbility.AbilityName}] 능력은 이미 최대 레벨입니다. (Lv.{existingAbility.MaxLevel}). 다른 보상을 제공할 수 있습니다.");
                // TODO: 최대 레벨 능력 선택 시 다른 보상 (예: 골드, 재선택 기회 등) 제공 로직
            }
        }
        else
        {
            // 새로운 능력 획득 (프리팹을 인스턴스화하여 게임 오브젝트로 만들고 컴포넌트 가져오기)
            GameObject abilityGO = Instantiate(abilityPrefab, transform); // 플레이어의 자식으로 추가 (관리 용이)
            Abillity newAbility = abilityGO.GetComponent<Abillity>();

            if (newAbility != null)
            {
                newAbility.InitializeAbility(abilityPrefab); // 능력 초기화 시 프리팹 정보 전달
                newAbility.OnAcquire(this); // 플레이어 인스턴스를 전달하여 초기 획득 및 효과 적용
                activeAbilities.Add(abilityPrefab, newAbility); // 딕셔너리에 추가
                Debug.Log($"[{newAbility.AbilityName}] 새로운 능력 획득! (Lv.{newAbility.CurrentLevel})");
            }
            else
            {
                Debug.LogError($"선택된 프리팹 {abilityPrefab.name}에 Abillity 컴포넌트가 없습니다!");
                Destroy(abilityGO); // 잘못된 프리팹이면 생성된 오브젝트 삭제
            }
        }

        // 능력 획득 후 합성 가능한 능력이 있는지 확인 (이 메서드는 능력 선택 UI에서 호출될 수 있음)
        // CheckForCombinations(); // 이 로직은 능력 선택 UI가 열리기 전에 호출하는 것이 더 적절할 수 있습니다.
    }

    /// <summary>
    /// 활성화된 능력들 중 특정 능력을 제거합니다. (주로 합성 시 원료 능력 제거에 사용)
    /// </summary>
    /// <param name="abilityPrefab">제거할 능력의 프리팹</param>
    public void RemoveAbility(GameObject abilityPrefab)
    {
        if (activeAbilities.TryGetValue(abilityPrefab, out Abillity abilityToRemove))
        {
            Debug.Log($"[{abilityToRemove.AbilityName}] 능력을 제거합니다.");
            abilityToRemove.OnRemove(); // 능력 제거 효과 호출 (예: 스탯 복원)
            Destroy(abilityToRemove.gameObject); // 게임 오브젝트 파괴
            activeAbilities.Remove(abilityPrefab); // 딕셔너리에서 제거
        }
        else
        {
            Debug.LogWarning($"제거하려는 능력 프리팹 {abilityPrefab.name}을(를) 활성화된 목록에서 찾을 수 없습니다.");
        }
    }

    /// <summary>
    /// 현재 활성화된 능력들과 레벨을 기반으로 합성 가능한 레시피를 찾습니다.
    /// </summary>
    /// <returns>합성 가능한 능력의 프리팹 목록</returns>
    public List<GameObject> GetCombinableAbilities()
    {
        List<GameObject> combinableList = new List<GameObject>();

        if (abilityRecipes == null || abilityRecipes.Count == 0) return combinableList;

        foreach (AbilityRecipe recipe in abilityRecipes)
        {
            // 이미 이 합성 능력을 가지고 있다면 스킵
            if (activeAbilities.ContainsKey(recipe.CombinedAbilityPrefab)) continue;

            bool canCombine = true;
            foreach (AbilityRecipe.RequiredAbility req in recipe.RequiredAbilities)
            {
                // 필요한 능력의 프리팹이 활성화된 능력 목록에 없거나, 요구 레벨에 미치지 못하면 합성 불가
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

    // --- PlayerController에서 Bow와 연동을 위한 함수들 (Bow에서만 사용) ---
    // 플레이어의 바라보는 방향과 이동 상태는 PlayerController에서 직접 가져오므로,
    // 이 Player 클래스에서는 별도로 정의하지 않습니다.

    // FireArrowAbility와 같은 조건부 발동 능력을 Bow가 활용할 수 있도록
    // 활성화된 능력 목록을 외부에 노출 (public 프로퍼티)
    // 혹은 private으로 유지하고, Bow에서 Player의 특정 메서드를 호출하도록 할 수도 있습니다.
    public IReadOnlyDictionary<GameObject, Abillity> GetActiveAbilities()
    {
        return activeAbilities;
    }

    // Bow가 화살을 발사하기 직전에 이 함수를 호출하여 특수 화살 발동을 시도
    public bool TryActivateSpecialArrowAbility(GameObject regularArrowGO, Arrow regularArrowScript)
    {
        foreach (var entry in activeAbilities)
        {
            Abillity ability = entry.Value;
            if (ability is FireArrowAbility fireArrowAbility)
            {
                // FireArrowAbility가 불화살을 발사하면 true 반환
                if (fireArrowAbility.TryActivateFireArrow(regularArrowGO, regularArrowScript))
                {
                    return true;
                }
            }
            // 다른 특수 화살 능력이 있다면 여기에 추가
        }
        return false; // 특수 화살 발사 실패
    }
}