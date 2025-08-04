using System.Collections.Generic;
using System.Linq;
using UnityEditor.Playables;
using UnityEngine;

public class Player : MonoBehaviour
{
    private AnimationHandler animationHandler;

    [Header("기본 스탯")]
    // 이 maxHealth는 이제 어빌리티 효과가 적용되기 전의 순수 기본 체력이 됩니다.
    // 어빌리티가 체력을 올리면 MaxHealth 프로퍼티를 통해 반영됩니다.
    [SerializeField] private float baseMaxHealth = 100f; // 레벨 1일 때의 기본 최대 체력
    private float _maxHealth; // 실제 스탯 계산에 사용될 현재 최대 체력
    public float MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    private float _health;
    public float Health
    {
        get => _health;
        private set
        {
            _health = Mathf.Clamp(value, 0, MaxHealth);
        }
    }

    // 이제 baseHealth는 필요 없습니다. baseMaxHealth로 통합됩니다.
    // [SerializeField] private float baseHealth = 100f; 

    [SerializeField] private float baseDefense = 0f;
    public float Defense { get; set; }

    [SerializeField] private float baseMoveSpeed = 5.0f;
    public float MoveSpeed { get; set; }

    [SerializeField] private float baseAttackDamage = 10f;
    public float AttackDamage { get; set; }

    [SerializeField] private float baseAttackRange = 3f;
    public float AttackRange { get; set; }

    [SerializeField] private float baseAttackSize = 1.0f;
    public float AttackSize { get; set; }

    [SerializeField] private float baseCriticalRate = 10f;
    public float CriticalRate { get; set; }

    [SerializeField] private float baseProjectileSpeed = 7f;
    public float ProjectileSpeed { get; set; }

    [SerializeField] private float baseAttackSpeed = 1.0f;
    public float AttackSpeedMultiplier { get; set; } = 100f;
    public float MaxAttackSpeed => baseAttackSpeed * (AttackSpeedMultiplier / 100f);

    public Dictionary<GameObject, Ability> activeAbilities = new Dictionary<GameObject, Ability>();

    [Header("능력 합성 레시피")]
    public List<AbilityRecipe> abilityRecipes;

    private LevelManager levelManager; // 레벨업 이벤트 수신용으로 유지

    void Awake()
    {
        animationHandler = GetComponentInChildren<AnimationHandler>();
        if (animationHandler == null)
        {
            Debug.LogError("Player 스크립트: AnimationHandler 컴포넌트를 자식에서 찾을 수 없습니다! 메인 스프라이트 오브젝트에 있는지 확인하세요.");
        }

        levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("Player: 씬에서 LevelManager 오브젝트를 찾을 수 없습니다! 레벨업 기능이 작동하지 않을 수 있습니다.");
        }

        RecalculateStats(); // 초기 스탯 계산 (기본 스탯 + 어빌리티)
        _health = MaxHealth; // 체력을 초기 최대 체력으로 설정

        if (abilityRecipes == null || abilityRecipes.Count == 0)
        {
            Debug.LogWarning("Player: Ability Recipes가 할당되지 않았습니다. 합성 기능이 작동하지 않을 수 있습니다.");
        }
    }

    /// <summary>
    /// 플레이어의 모든 스탯을 기본값으로 재설정하고, 활성화된 능력의 효과를 다시 적용합니다.
    /// 능력 획득/제거 시 호출됩니다. 레벨업 시에는 스탯을 직접적으로 올리지 않습니다.
    /// </summary>
    public void RecalculateStats()
    {
        // 기본 스탯으로 초기화 (레벨에 따른 증가분 없음)
        MaxHealth = baseMaxHealth; // 기본 최대 체력으로 초기화
        MoveSpeed = baseMoveSpeed;
        AttackDamage = baseAttackDamage;
        AttackRange = baseAttackRange;
        AttackSize = baseAttackSize;
        CriticalRate = baseCriticalRate;
        ProjectileSpeed = baseProjectileSpeed;
        AttackSpeedMultiplier = 100f;
        Defense = baseDefense;

        // 활성화된 능력 효과 재적용
        // 능력들이 이 기본 스탯에 더하거나 곱하는 방식으로 스탯을 변경합니다.
        foreach (var abilityEntry in activeAbilities.Values)
        {
            abilityEntry.ApplyEffect();
        }

        // 스탯 재계산 후 현재 체력이 새로운 MaxHealth를 초과하지 않도록 조정
        Health = _health; // _health 값을 MaxHealth 범위에 맞게 클램프 (Health 프로퍼티의 set 로직이 적용됨)

        Debug.Log("모든 스탯 재계산 완료 (어빌리티 효과만 적용됨). 현재 MaxHealth: " + MaxHealth + ", AttackDamage: " + AttackDamage);
    }

    /// <summary>
    /// 레벨업 시 호출될 능력 획득 메서드.
    /// </summary>
    /// <param name="abilityPrefab">선택된 능력의 프리팹.</param>
    public void AcquireAbility(GameObject abilityPrefab)
    {
        Ability existingAbility = null;
        if (activeAbilities.TryGetValue(abilityPrefab, out existingAbility))
        {
            if (existingAbility.CurrentLevel < existingAbility.MaxLevel)
            {
                existingAbility.OnAcquire(this);
                Debug.Log($"[{existingAbility.AbilityName}] 능력이 레벨업! (Lv.{existingAbility.CurrentLevel})");
            }
            else
            {
                Debug.Log($"[{existingAbility.AbilityName}] 능력은 이미 최대 레벨입니다. (Lv.{existingAbility.MaxLevel}). 다른 보상을 제공할 수 있습니다.");
            }
        }
        else
        {
            GameObject abilityGO = Instantiate(abilityPrefab, transform);
            Ability newAbility = abilityGO.GetComponent<Ability>();

            if (newAbility != null)
            {
                newAbility.InitializeAbility(abilityPrefab);
                newAbility.OnAcquire(this);
                activeAbilities.Add(abilityPrefab, newAbility);
                Debug.Log($"[{newAbility.AbilityName}] 새로운 능력 획득! (Lv.{newAbility.CurrentLevel})");
            }
            else
            {
                Debug.LogError($"선택된 프리팹 {abilityPrefab.name}에 Ability 컴포넌트가 없습니다!");
                Destroy(abilityGO);
            }
        }
        RecalculateStats(); // 능력 획득/강화 후 스탯 재계산
    }

    /// <summary>
    /// 활성화된 능력들 중 특정 능력을 제거합니다. (주로 합성 시 원료 능력 제거에 사용)
    /// </summary>
    /// <param name="abilityPrefab">제거할 능력의 프리팹.</param>
    public void RemoveAbility(GameObject abilityPrefab)
    {
        if (activeAbilities.TryGetValue(abilityPrefab, out Ability abilityToRemove))
        {
            Debug.Log($"[{abilityToRemove.AbilityName}] 능력을 제거합니다.");
            abilityToRemove.OnRemove();
            Destroy(abilityToRemove.gameObject);
            activeAbilities.Remove(abilityPrefab);
            RecalculateStats(); // 능력 제거 후 스탯 재계산
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
            // 이미 합성된 능력이면 건너뛰기
            if (activeAbilities.ContainsKey(recipe.CombinedAbilityPrefab)) continue;

            bool canCombine = true;
            foreach (AbilityRecipe.RequiredAbility req in recipe.RequiredAbilities)
            {
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
            Ability ability = entry.Value;
            if (ability is FireArrow fireArrow) // FireArrow는 Ability를 상속하는 특정 능력 클래스라고 가정
            {
                if (fireArrow.TryActivateFireArrow(regularArrowGO, regularArrowScript))
                {
                    return true;
                }
            }
            // 다른 특수 화살 능력이 있다면 여기에 추가
            // if (ability is IceArrow iceArrow) { ... }
        }
        return false;
    }

    // --- 체력 관리 메서드 ---
    /// <summary>
    /// 플레이어에게 피해를 입히는 외부 호출용 메서드.
    /// 이 메서드를 통해 플레이어에게 데미지를 줄 수 있습니다.
    /// </summary>
    /// <param name="damageAmount">받을 피해량.</param>
    /// <param name="attacker">피해를 입힌 GameObject (선택 사항).</param>
    public void TakeDamage(float damageAmount, GameObject attacker = null)
    {
        if (animationHandler != null)
        {
            animationHandler.Hurt();
        }
        else
        {
            Debug.LogWarning("Player.TakeDamage: AnimationHandler가 할당되지 않아 피해 애니메이션을 재생할 수 없습니다.");
        }

        float finalDamage = Mathf.Max(0, damageAmount - this.Defense);

        Health -= finalDamage;

        Debug.Log($"피해를 받았습니다: {finalDamage:F2}. 남은 체력: {Health:F2}");

        if (Health <= 0)
        {
            Death(attacker);
        }
    }

    /// <summary>
    /// 플레이어 사망 처리 로직.
    /// </summary>
    /// <param name="killer">플레이어를 사망하게 만든 GameObject (선택 사항).</param>
    private void Death(GameObject killer = null)
    {
        if (animationHandler != null)
        {
            animationHandler.Death();
        }
        else
        {
            Debug.LogWarning("Player.Death: AnimationHandler가 할당되지 않아 사망 애니메이션을 재생할 수 없습니다.");
        }

        string killerInfo = killer != null ? killer.name : "알 수 없는 원인";
        Debug.Log($"플레이어가 사망했습니다! 사망 원인: {killerInfo}");

        Time.timeScale = 0f; // 게임 일시 정지 (예시)
        // 다른 게임 오버 로직 (씬 로드, UI 활성화 등)
    }

    /// <summary>
    /// 플레이어의 체력을 회복합니다.
    /// </summary>
    /// <param name="healAmount">회복할 체력 양.</param>
    public void Heal(float healAmount)
    {
        Health += healAmount;
        Debug.Log($"체력 회복: {healAmount:F2}. 현재 체력: {Health:F2}");
    }
}