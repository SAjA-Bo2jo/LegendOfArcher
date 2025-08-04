using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Player : MonoBehaviour
{
    // protected AnimationHandler animationHandler; // 이 필드는 삭제하거나, 아래와 같이 [SerializeField]로 바꾸고 Unity에서 연결하는 것을 고려.
                                                    // GetComponentInChildren으로 찾을 것이므로, 필드는 그냥 private 또는 protected로 유지해도 무방합니다.
    private AnimationHandler animationHandler; // PlayerController와 마찬가지로, private으로 바꾸고 GetComponentInChildren으로 찾도록 합니다.
    [Header("기본 스탯")]
    // --- 체력 관련 스탯 ---
    [SerializeField] private float maxHealth = 100f; // 최대 체력
    // Health 프로퍼티의 실제 값을 저장할 private 필드
    private float _health;
    public float Health
    {
        get => _health;
        set
        {
            // 할당하려는 새 체력 값이 현재 체력보다 낮을 때 (피해를 입는 상황)
            if (value < _health)
            {
                // Health 프로퍼티를 직접 변경하는 대신, TakeDamage 메서드를 호출하여 피해 처리 로직을 실행
                // 이때, 'value'와 '_health'의 차이만큼을 피해량으로 넘겨줍니다.
                // TakeDamage 메서드 내부에서 _health 값을 직접 변경하므로 무한 재귀가 발생하지 않습니다.
                float damageAmount = _health - value;
                TakeDamage(damageAmount); // 내부적으로 데미지를 처리하는 새 메서드 호출
            }
            else // 체력이 증가하거나 같은 값으로 설정될 때 (회복, 초기화 등)
            {
                _health = Mathf.Min(value, MaxHealth); // 최대 체력을 넘지 않도록 설정
                Debug.Log($"체력 업데이트: {_health}. (TakeDamageExternal)");
            }
            // 체력이 0 이하가 되면 사망 처리 (이 부분은 외부에서 체력을 직접 0 이하로 설정할 경우를 대비)
            if (_health <= 0)
            {
                Death();
            }
        }
    }
    public float MaxHealth // 최대 체력
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }
    [SerializeField] private float baseDefense = 0f;
    public float Defense { get; set; } // 최종 방어력 (받는 피해 감소)
    [SerializeField] private float baseMoveSpeed = 5.0f;
    public float MoveSpeed { get; set; } // 최종 이동 속도
    [SerializeField] private float baseAttackDamage = 10f;
    public float AttackDamage { get; set; } // 최종 공격 데미지
    [SerializeField] private float baseAttackRange = 3f;
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
    public int Level => level; // 레벨은 읽기 전용으로 외부 노출
    [SerializeField] private float experience = 0f; // 현재 경험치
    public float Experience => experience; // 경험치 읽기 전용
    [SerializeField] private float[] expToNextLevel; // 레벨업에 필요한 경험치 배열 (인스펙터에서 설정)
    // --- 능력 관리 필드 ---
    // 획득한 능력들을 저장할 딕셔너리: <능력 프리팹 (Key), 해당 능력 인스턴스 (Value)>
    public Dictionary<GameObject, Abillity> activeAbilities = new Dictionary<GameObject, Abillity>();
    [Header("능력 합성 레시피")]
    // 합성 레시피 목록
    public List<AbilityRecipe> abilityRecipes;
    void Awake()
    {
        // === 핵심 수정 부분 ===
        // AnimationHandler는 Player GameObject의 자식에 있으므로 GetComponentInChildren로 찾습니다.
        animationHandler = GetComponentInChildren<AnimationHandler>();
        if (animationHandler == null)
        {
            Debug.LogError("Player 스크립트: AnimationHandler 컴포넌트를 자식에서 찾을 수 없습니다! 메인 스프라이트 오브젝트에 있는지 확인하세요.");
        }
        // === 여기까지 수정 ===
        RecalculateStats();
        // Awake 시에는 Health 프로퍼티의 set 접근자를 통하지 않고 직접 _health 필드를 초기화
        _health = maxHealth;
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
        MaxHealth = maxHealth; // 최대 체력도 기본값으로 설정
        MoveSpeed = baseMoveSpeed;
        AttackDamage = baseAttackDamage;
        AttackRange = baseAttackRange;
        AttackSize = baseAttackSize;
        CriticalRate = baseCriticalRate;
        ProjectileSpeed = baseProjectileSpeed;
        AttackSpeedMultiplier = 100f;
        Defense = baseDefense;
        // 활성화된 모든 능력들의 효과를 재적용합니다.
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
            Abillity newAbility = abilityGO.GetComponent<Abillity>();
            if (newAbility != null)
            {
                newAbility.InitializeAbility(abilityPrefab);
                newAbility.OnAcquire(this);
                activeAbilities.Add(abilityPrefab, newAbility);
                Debug.Log($"[{newAbility.AbilityName}] 새로운 능력 획득! (Lv.{newAbility.CurrentLevel})");
            }
            else
            {
                Debug.LogError($"선택된 프리팹 {abilityPrefab.name}에 Abillity 컴포넌트가 없습니다!");
                Destroy(abilityGO);
            }
        }
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
            abilityToRemove.OnRemove();
            Destroy(abilityToRemove.gameObject);
            activeAbilities.Remove(abilityPrefab);
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
            Abillity ability = entry.Value;
            if (ability is FireArrowAbility fireArrowAbility)
            {
                if (fireArrowAbility.TryActivateFireArrow(regularArrowGO, regularArrowScript))
                {
                    return true;
                }
            }
        }
        return false;
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
        if (level < expToNextLevel.Length && experience >= expToNextLevel[level - 1])
        {
            LevelUp();
        }
        else if (level >= expToNextLevel.Length)
        {
            Debug.Log("최대 레벨에 도달했습니다. 더 이상 경험치를 획득할 수 없습니다.");
        }
    }
    /// <summary>
    /// 플레이어를 레벨업 시키고 관련 스탯을 증가시킵니다.
    /// </summary>
    private void LevelUp()
    {
        level++;
        experience = 0;
        Debug.Log($"레벨업! 현재 레벨: {level}");
        maxHealth += 10;
        // MaxHealth 프로퍼티는 RecalculateStats에서 업데이트되므로, 여기서 직접 _health를 설정
        _health = maxHealth;
        baseAttackDamage += 1;
        baseMoveSpeed += 0.1f;
        RecalculateStats();
    }
    // --- 체력 관리 메서드 ---
    /// <summary>
    /// 플레이어에게 피해를 입히는 외부 호출용 메서드.
    /// 이 메서드를 통해 플레이어에게 데미지를 줄 수 있습니다.
    /// </summary>
    /// <param name="damageAmount">받을 피해량.</param>
    public void TakeDamage(float damageAmount)
    {
        // === 핵심 수정 부분 ===
        // animationHandler가 null이 아닐 때만 Hurt() 호출
        if (animationHandler != null)
        {
            animationHandler.Hurt(); // 애니메이션 핸들러를 통해 피해 애니메이션 실행
        }
        else
        {
            Debug.LogWarning("Player.TakeDamage: AnimationHandler가 할당되지 않아 피해 애니메이션을 재생할 수 없습니다.");
        }
        // === 여기까지 수정 ===
        float finalDamage = Mathf.Max(0, (damageAmount / (damageAmount + this.Defense)));
        // _health 필드를 직접 수정하여 무한 재귀를 방지합니다.
        _health -= finalDamage;
        Debug.Log($"피해를 받았습니다: {finalDamage}. 남은 체력: {_health}");
        if (_health <= 0)
        {
            Death();
        }
    }
    /// <summary>
    /// 플레이어의 체력을 회복시킵니다.
    /// </summary>
    /// <param name="healAmount">회복할 체력 양.</param>
    public void Heal(float healAmount)
    {
        // Heal 메서드는 체력을 증가시키는 경우이므로, Health 프로퍼티의 set 접근자를 사용해도 됩니다.
        // 이때 Health 프로퍼티의 set 접근자 내부에 있는 'value < _health' 조건에 걸리지 않으므로 안전합니다.
        Health = Mathf.Min(MaxHealth, Health + healAmount);
        Debug.Log($"체력 회복: {healAmount}. 현재 체력: {Health}");
    }
    /// <summary>
    /// 플레이어 사망 처리 로직 (필요시 구현).
    /// </summary>
    private void Death()
    {
        // === 핵심 수정 부분 ===
        // animationHandler가 null이 아닐 때만 Death() 호출
        if (animationHandler != null)
        {
            animationHandler.Death();
        }
        else
        {
            Debug.LogWarning("Player.Death: AnimationHandler가 할당되지 않아 사망 애니메이션을 재생할 수 없습니다.");
        }
        // === 여기까지 수정 ===
        Debug.Log("플레이어가 사망했습니다!");
        // 게임 오버 처리, UI 표시 등
        // Time.timeScale = 0f; // 게임 일시 정지 (예시)
    }
}







