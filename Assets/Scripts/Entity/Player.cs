using System.Collections.Generic;
using UnityEngine;
using System.Linq; // LINQ 사용을 위해 추가

public class Player : MonoBehaviour
{
    [Header("기본 스탯")]
    // --- 체력 관련 스탯 ---
    [SerializeField] private float maxHealth = 100f; // 최대 체력
    public float Health { get; set; } // 현재 체력 (Public으로 외부 접근 허용)

    // --- 방어 관련 스탯 ---
    [SerializeField] private float baseDefense = 0f;
    public float Defense { get; set; } // 최종 방어력 (받는 피해 감소)

    // --- 이동 속도 스탯 ---
    [SerializeField] private float baseMoveSpeed = 5.0f;
    public float MoveSpeed { get; set; } // 최종 이동 속도

    // --- 공격 관련 스탯 ---
    [SerializeField] private float baseAttackDamage = 10f;
    public float AttackDamage { get; set; } // 최종 공격 데미지

    [SerializeField] private float baseAttackRange = 5f;
    public float AttackRange { get; set; } // 최종 공격 범위

    [SerializeField] private float baseAttackSize = 1.0f;
    public float AttackSize { get; set; } // 투사체/공격의 크기 배율

    [SerializeField] private float baseCriticalRate = 10f; // 기본 치명타 확률 (%)
    public float CriticalRate { get; set; } // 최종 치명타 확률 (%)

    [SerializeField] private float baseProjectileSpeed = 7f;
    public float ProjectileSpeed { get; set; } // 최종 투사체 속도

    [SerializeField] private float baseAttackSpeed = 1.0f; // 기본 공격 속도 (초당 공격 횟수)
    public float AttackSpeedMultiplier { get; set; } = 100f; // 공격 속도 배율 (100 = 100%)
    public float MaxAttackSpeed => baseAttackSpeed * (AttackSpeedMultiplier / 100f); // 최종 공격 속도 계산

    // --- 경험치/레벨 관련 스탯 ---
    [SerializeField] private int level = 1; // 현재 레벨
    public int Level => level; // 레벨은 읽기 전용으로 외부 노출 (선택 사항)

    [SerializeField] private float experience = 0f; // 현재 경험치
    public float Experience => experience; // 경험치 읽기 전용 (선택 사항)

    [SerializeField] private float[] expToNextLevel; // 레벨업에 필요한 경험치 배열 (인스펙터에서 설정)

    // --- 능력 관리 필드 ---
    // 획득한 능력들을 저장할 딕셔너리: <능력 프리팹 (Key), 해당 능력 인스턴스 (Value)>
    public Dictionary<GameObject, Abillity> activeAbilities = new Dictionary<GameObject, Abillity>();

    [Header("능력 합성 레시피")]
    // 합성 레시피 목록 (Unity 에디터에서 AbilityRecipe ScriptableObject 에셋들을 할당)
    public List<AbilityRecipe> abilityRecipes;

    void Awake()
    {
        // 모든 스탯 프로퍼티를 기본값으로 초기화
        Health = maxHealth; // 시작 시 현재 체력을 최대 체력으로 설정
        MoveSpeed = baseMoveSpeed;
        AttackDamage = baseAttackDamage;
        AttackRange = baseAttackRange;
        AttackSize = baseAttackSize;
        CriticalRate = baseCriticalRate;
        ProjectileSpeed = baseProjectileSpeed;
        AttackSpeedMultiplier = 100f; // 기본값은 100% (변경 없음)
        Defense = baseDefense;

        if (abilityRecipes == null || abilityRecipes.Count == 0)
        {
            Debug.LogWarning("Player: Ability Recipes가 할당되지 않았습니다. 합성 기능이 작동하지 않을 수 있습니다.");
        }
    }

    /// <summary>
    /// 플레이어의 모든 스탯을 기본값으로 재설정하고, 활성화된 능력의 효과를 다시 적용합니다.
    /// 능력 획득/제거 시 또는 스탯에 영향을 주는 아이템 변경 시 호출됩니다.
    /// </summary>
    public void RecalculateStats()
    {
        // 모든 스탯을 기본값으로 초기화
        // Health는 직접적인 스탯 변경이 아닌, 회복/피해 로직에서 관리되므로 재계산에서 제외
        MoveSpeed = baseMoveSpeed;
        AttackDamage = baseAttackDamage;
        AttackRange = baseAttackRange;
        AttackSize = baseAttackSize;
        CriticalRate = baseCriticalRate;
        ProjectileSpeed = baseProjectileSpeed;
        AttackSpeedMultiplier = 100f;
        Defense = baseDefense;

        // 활성화된 모든 능력들의 효과를 재적용합니다.
        // 각 Abillity의 ApplyEffect() 메서드는 '이전 효과를 제거한 후 현재 레벨의 효과를 적용'하는 방식으로 구현되어야 합니다.
        foreach (var abilityEntry in activeAbilities)
        {
            abilityEntry.Value.ApplyEffect();
        }

        Debug.Log("모든 스탯 재계산 완료.");
    }


    /// <summary>
    /// 레벨업 시 호출될 능력 획득 메서드.
    /// </summary>
    /// <param name="abilityPrefab">선택된 능력의 프리팹.</param>
    public void AcquireAbility(GameObject abilityPrefab)
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

        // 능력 획득 후 스탯 재계산 (스탯에 영향을 주는 모든 능력이 적용되도록)
        RecalculateStats();
    }

    /// <summary>
    /// 활성화된 능력들 중 특정 능력을 제거합니다. (주로 합성 시 원료 능력 제거에 사용)
    /// </summary>
    /// <param name="abilityPrefab">제거할 능력의 프리팹.</param>
    public void RemoveAbility(GameObject abilityPrefab)
    {
        if (activeAbilities.TryGetValue(abilityPrefab, out Abillity abilityToRemove))
        {
            Debug.Log($"[{abilityToRemove.AbilityName}] 능력을 제거합니다.");
            abilityToRemove.OnRemove(); // 능력 제거 효과 호출 (예: 스탯 복원)
            Destroy(abilityToRemove.gameObject); // 게임 오브젝트 파괴
            activeAbilities.Remove(abilityPrefab); // 딕셔너리에서 제거

            // 능력 제거 후 스탯 재계산
            RecalculateStats();
        }
        else
        {
            Debug.LogWarning($"제거하려는 능력 프리팹 {abilityPrefab.name}을(를) 활성화된 목록에서 찾을 수 없습니다.");
        }
    }

    /// <summary>
    /// 현재 활성화된 능력들과 레벨을 기반으로 합성 가능한 레시피를 찾습니다.
    /// </summary>
    /// <returns>합성 가능한 능력의 프리팹 목록.</returns>
    public List<GameObject> GetCombinableAbilities()
    {
        List<GameObject> combinableList = new List<GameObject>();

        if (abilityRecipes == null || abilityRecipes.Count == 0) return combinableList;

        foreach (AbilityRecipe recipe in abilityRecipes)
        {
            // 이미 이 합성 능력을 가지고 있다면 스킵 (중복 획득 방지)
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

    /// <summary>
    /// Bow가 화살을 발사하기 직전에 이 함수를 호출하여 특수 화살 발동을 시도합니다.
    /// </summary>
    /// <param name="regularArrowGO">발사하려던 일반 화살 GameObject.</param>
    /// <param name="regularArrowScript">발사하려던 일반 화살 Arrow 컴포넌트.</param>
    /// <returns>특수 화살이 발사되었으면 true, 아니면 false.</returns>
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
            // if (ability is <SomeOtherSpecialArrowAbility> otherAbility) { ... }
        }
        return false; // 특수 화살 발사 실패
    }

    // --- 경험치 획득 및 레벨업 로직 ---
    /// <summary>
    /// 플레이어에게 경험치를 추가합니다.
    /// </summary>
    /// <param name="expAmount">추가할 경험치 양.</param>
    public void AddExperience(float expAmount)
    {
        experience += expAmount;
        Debug.Log($"경험치 획득: {expAmount}. 현재 경험치: {experience}");

        // 레벨업 조건 확인
        if (level < expToNextLevel.Length && experience >= expToNextLevel[level - 1])
        {
            LevelUp();
        }
    }

    /// <summary>
    /// 플레이어를 레벨업 시키고 관련 스탯을 증가시킵니다.
    /// </summary>
    private void LevelUp()
    {
        level++;
        experience = 0; // 경험치 초기화 (또는 다음 레벨까지 남은 경험치로 설정)
        Debug.Log($"레벨업! 현재 레벨: {level}");

        // 레벨업에 따른 기본 스탯 증가 (선택 사항)
        maxHealth += 10; // 최대 체력 증가
        Health = maxHealth; // 현재 체력을 증가된 최대 체력으로 설정 (회복 효과)
        baseAttackDamage += 1; // 기본 공격력 증가
        baseMoveSpeed += 0.1f; // 기본 이동 속도 증가

        RecalculateStats(); // 모든 스탯 재계산

        // 능력 선택 UI를 띄우는 이벤트 또는 메서드 호출 (예시)
        // AbilitySelectionUI.Instance.ShowAbilitySelection(); 
    }

    // --- 체력 관리 메서드 (예시) ---
    /// <summary>
    /// 플레이어에게 피해를 입힙니다.
    /// </summary>
    /// <param name="damageAmount">받을 피해량.</param>
    public void TakeDamage(float damageAmount)
    {
        float finalDamage = Mathf.Max(0, damageAmount - Defense); // 방어력 적용
        Health -= finalDamage;
        Debug.Log($"피해를 받았습니다: {finalDamage}. 남은 체력: {Health}");

        if (Health <= 0)
        {
            Die(); // 플레이어 사망 처리
        }
    }

    /// <summary>
    /// 플레이어의 체력을 회복시킵니다.
    /// </summary>
    /// <param name="healAmount">회복할 체력 양.</param>
    public void Heal(float healAmount)
    {
        Health = Mathf.Min(maxHealth, Health + healAmount);
        Debug.Log($"체력 회복: {healAmount}. 현재 체력: {Health}");
    }

    /// <summary>
    /// 플레이어 사망 처리 로직 (필요시 구현).
    /// </summary>
    private void Die()
    {
        Debug.Log("플레이어가 사망했습니다!");
        // 게임 오버 처리, UI 표시 등
        // Time.timeScale = 0f; // 게임 일시 정지 (예시)
    }
}