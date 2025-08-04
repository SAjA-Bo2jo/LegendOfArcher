using System.Collections.Generic;
using UnityEngine;
using System.Linq; // LINQ를 사용하기 위해 추가

public class Player : MonoBehaviour
{
    private AnimationHandler animationHandler;

    [Header("플레이어 피격 효과음")] public AudioClip damageSoundClip;
    
    [Header("기본 스탯")]
    [SerializeField] private float maxHealth = 100f;
    private float _health;
    public float Health
    {
        get => _health;
        private set // 외부에서 직접 대입하지 못하도록 private set으로 변경
        {
            _health = Mathf.Clamp(value, 0, MaxHealth); // 0 미만, MaxHealth 초과 방지
                                                        // 체력 변화에 따른 Debug.Log는 TakeDamage나 Heal 내부에서 하는 것이 좋습니다.
                                                        // Debug.Log($"체력 업데이트: {_health:F2}"); // 필요시 여기에 로그 추가
        }
    }
    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

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

    [Header("경험치/레벨 관련 스탯")]
    [SerializeField] private int level = 1;
    public int Level => level;

    [SerializeField] private float experience = 0f;
    public float Experience => experience;

    // 레벨업에 필요한 경험치 배열 (코드에서 자동 생성)
    [SerializeField] private float[] expToNextLevel;

    [Tooltip("경험치 테이블 계산에 사용될 기본 경험치량 (레벨1->2).")]
    [SerializeField] private float baseExperienceForLevelUp = 100f;
    [Tooltip("경험치 테이블 계산에 사용될 지수 계수.")]
    [SerializeField] private float expCoefficient = 1.2f; // 사용자 요청 계수 1.2
    [Tooltip("최대 도달 가능한 레벨.")]
    [SerializeField] private const int MAX_PLAYER_LEVEL = 40; // 최대 레벨 40

    // --- 능력 관리 필드 ---
    public Dictionary<GameObject, Abillity> activeAbilities = new Dictionary<GameObject, Abillity>();

    [Header("능력 합성 레시피")]
    public List<AbilityRecipe> abilityRecipes;

    void Awake()
    {
        animationHandler = GetComponentInChildren<AnimationHandler>();
        if (animationHandler == null)
        {
            Debug.LogError("Player 스크립트: AnimationHandler 컴포넌트를 자식에서 찾을 수 없습니다! 메인 스프라이트 오브젝트에 있는지 확인하세요.");
        }

        // 경험치 테이블 생성
        GenerateExpTable();

        RecalculateStats();
        _health = maxHealth;

        if (abilityRecipes == null || abilityRecipes.Count == 0)
        {
            Debug.LogWarning("Player: Ability Recipes가 할당되지 않았습니다. 합성 기능이 작동하지 않을 수 있습니다.");
        }
    }

    /// 지수함수를 적용하여 레벨업에 필요한 경험치 테이블을 생성합니다.
    private void GenerateExpTable()
    {
        expToNextLevel = new float[MAX_PLAYER_LEVEL - 1];

        for (int i = 0; i < MAX_PLAYER_LEVEL - 1; i++)
        {
            expToNextLevel[i] = baseExperienceForLevelUp * Mathf.Pow(expCoefficient, i);
            Debug.Log($"Lv.{i + 1} -> Lv.{i + 2} 필요 경험치: {expToNextLevel[i]:F2}");
        }
        Debug.Log($"경험치 테이블 생성 완료 (총 {expToNextLevel.Length} 레벨 구간).");
    }

    /// <summary>
    /// 플레이어의 모든 스탯을 기본값으로 재설정하고, 활성화된 능력의 효과를 다시 적용합니다.
    /// 능력 획득/제거 시 또는 스탯에 영향을 주는 아이템 변경 시 호출됩니다.
    /// </summary>
    public void RecalculateStats()
    {
        MaxHealth = maxHealth;
        MoveSpeed = baseMoveSpeed;
        AttackDamage = baseAttackDamage;
        AttackRange = baseAttackRange;
        AttackSize = baseAttackSize;
        CriticalRate = baseCriticalRate;
        ProjectileSpeed = baseProjectileSpeed;
        AttackSpeedMultiplier = 100f;
        Defense = baseDefense;

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
            Abillity ability = entry.Value;
            if (ability is FireArrow fireArrow)
            {
                if (fireArrow.TryActivateFireArrow(regularArrowGO, regularArrowScript))
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
        if (level >= MAX_PLAYER_LEVEL) // 최대 레벨에 도달했는지 먼저 확인
        {
            Debug.Log("최대 레벨에 도달했습니다. 더 이상 경험치를 획득할 수 없습니다.");
            return;
        }

        experience += expAmount;
        Debug.Log($"경험치 획득: {expAmount}. 현재 경험치: {experience:F2} / {expToNextLevel[level - 1]:F2}");

        // 현재 레벨이 최대 레벨 미만이고, 다음 레벨업에 필요한 경험치를 충족했는지 확인
        // 이 부분은 그대로 두어서 레벨업 조건이 충족되면 LevelUp을 호출하게 합니다.
        if (level < MAX_PLAYER_LEVEL && experience >= expToNextLevel[level - 1])
        {
            LevelUp();
        }
    }

    /// <summary>
    /// 플레이어를 레벨업 시키고 관련 스탯을 증가시킵니다.
    /// </summary>
    private void LevelUp()
    {
        // 현재 레벨업에 필요한 경험치량을 가져옵니다.
        float requiredExpForCurrentLevel = expToNextLevel[level - 1];

        // 초과 경험치를 계산하고 다음 레벨로 이월합니다.
        experience -= requiredExpForCurrentLevel; // 필요 경험치를 빼고 남은 경험치가 다음 레벨로 이월됩니다.

        level++; // 레벨 증가

        Debug.Log($"레벨업! 현재 레벨: {level}. 이월된 경험치: {experience:F2}");

        RecalculateStats(); // 증가된 스탯 및 능력 효과 재적용

        // 레벨업 시 어빌리티 선택창 표시
        AbilitySelectionManager abilityManager = FindObjectOfType<AbilitySelectionManager>();
        if (abilityManager != null)
        {
            abilityManager.ShowAbilitySelection();
        }
        else
        {
            Debug.LogWarning("AbilitySelectionManager를 씬에서 찾을 수 없습니다. 어빌리티 선택창을 표시할 수 없습니다.");
        }

        // 레벨업 후에도 여전히 다음 레벨업 조건을 충족하는지 다시 확인합니다.
        // 이는 한 번에 여러 레벨이 오를 수 있는 경우 (예: 매우 많은 경험치를 한 번에 획득)를 처리하기 위함입니다.
        if (level < MAX_PLAYER_LEVEL && experience >= expToNextLevel[level - 1])
        {
            LevelUp(); // 중첩 레벨업을 위해 재귀적으로 호출 (최대 레벨까지 계속)
        }
        else if (level >= MAX_PLAYER_LEVEL)
        {
            // 최대 레벨에 도달하면 경험치를 0으로 설정합니다. (더 이상 레벨업 불가)
            experience = 0;
            Debug.Log("최대 레벨에 도달하여 더 이상 레벨업할 수 없습니다.");
        }
    }

    /// 플레이어에게 피해를 입히는 외부 호출용 메서드.
    /// 이 메서드를 통해 플레이어에게 데미지를 줄 수 있습니다.
    /// </summary>
    /// <param name="damageAmount">받을 피해량.</param>
    /// <param name="attacker">피해를 입힌 GameObject (선택 사항).</param>
    public void TakeDamage(float damageAmount, GameObject attacker = null) // attacker 매개변수 추가, 기본값 null
    {
        if (animationHandler != null)
        {
            animationHandler.Hurt();
            
            if (damageSoundClip != null)
                SoundManager.Instance.PlaySoundEffect(damageSoundClip, Vector3.zero);
        }
        else
        {
            Debug.LogWarning("Player.TakeDamage: AnimationHandler가 할당되지 않아 피해 애니메이션을 재생할 수 없습니다.");
        }

        float finalDamage = Mathf.Max(0, damageAmount - this.Defense);

        // _health 대신 Health 프로퍼티의 set 접근자를 사용하도록 변경
        Health -= finalDamage; // Health 프로퍼티를 통해 체력 감소

        Debug.Log($"피해를 받았습니다: {finalDamage:F2}. 남은 체력: {Health:F2}");

        if (Health <= 0) // Health 프로퍼티 사용
        {
            Death(attacker); // 사망 시 공격자 정보 전달
        }
    }

    /// <summary>
    /// 플레이어 사망 처리 로직.
    /// </summary>
    /// <param name="killer">플레이어를 사망하게 만든 GameObject (선택 사항).</param>
    private void Death(GameObject killer = null) // killer 매개변수 추가, 기본값 null
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

        // 게임 오버 처리, UI 표시 등
        // Time.timeScale = 0f; // 게임 일시 정지 (예시)
        StageManager.Instance.PlayerDie();
        GameManager.Instance.monsterImage = killer.GetComponent<SpriteRenderer>().sprite;
    }

    /// <param name="healAmount">회복할 체력 양.</param>
    public void Heal(float healAmount)
    {
        Health = Mathf.Min(MaxHealth, Health + healAmount);
        Debug.Log($"체력 회복: {healAmount:F2}. 현재 체력: {Health:F2}");
    }
}