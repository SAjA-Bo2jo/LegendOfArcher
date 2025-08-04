using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour
{
    // === 플레이어 스탯 ===
    [Header("Player Stats")]
    public float MaxHealth = 100f;
    public float AttackDamage = 10f;
    public float Defense = 0f;
    public float MoveSpeed = 5f;
    public float AttackSpeed = 1f;
    public float MaxAttackSpeed { get; private set; }
    public float AttackRange = 5f;
    public float AttackSize = 1f;
    public float ProjectileSpeed = 10f;
    public float CriticalRate = 0.1f;

    // === 플레이어 현재 상태 ===
    private bool isDead = false;
    //private bool isInvincible = false; // 무적 상태를 관리할 새로운 변수
    public float Health { get; private set; }
    [SerializeField] private float invincibilityDuration = 0.5f;
    private float invincibilityEndTime = 0f;

    // === 능력 관리 ===
    public Dictionary<GameObject, Ability> activeAbilities = new Dictionary<GameObject, Ability>();

    // === 합성 능력 관리 ===
    [Header("Ability Recipes")]
    // 이제 MonoBehaviour를 상속받는 AbilityRecipe 컴포넌트를 참조합니다.
    [SerializeField]
    public AbilityRecipe[] abilityRecipes;

    // === 기타 참조 ===
    private PlayerController playerController;
    private AnimationHandler animationHandler;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animationHandler = GetComponentInChildren<AnimationHandler>();
    }

    void Start()
    {
        RecalculateStats();
        Health = MaxHealth;
    }

    void Update()
    {
        //if (Time.time >= invincibilityEndTime && IsInvincible())
        //{
        //    animationHandler?.InvincibilityEnd();
        //}
    }

    /// <summary>
    /// 새로운 능력을 플레이어에게 추가하고 스탯을 재계산합니다.
    /// </summary>
    public void AcquireAbility(GameObject abilityPrefab)
    {
        Ability existingAbility = activeAbilities.Values.FirstOrDefault(a => a.AbilityPrefab == abilityPrefab);

        if (existingAbility != null)
        {
            if (existingAbility.CurrentLevel < existingAbility.MaxLevel)
            {
                existingAbility.OnAcquire(this);
                Debug.Log($"[{existingAbility.AbilityName}] 능력 레벨업! Lv.{existingAbility.CurrentLevel}");
            }
            else
            {
                Debug.Log($"[{existingAbility.AbilityName}] 능력은 이미 최대 레벨입니다.");
            }
        }
        else
        {
            GameObject abilityInstance = Instantiate(abilityPrefab, transform);
            Ability newAbility = abilityInstance.GetComponent<Ability>();

            if (newAbility != null)
            {
                newAbility.InitializeAbility(abilityPrefab);
                newAbility.OnAcquire(this);
                activeAbilities.Add(abilityPrefab, newAbility);
                Debug.Log($"[{newAbility.AbilityName}] 새로운 능력 획득! (Lv.{newAbility.CurrentLevel})");
            }
            else
            {
                Debug.LogError($"AcquireAbility: 선택된 프리팹 {abilityPrefab.name}에 Ability 컴포넌트가 없습니다!");
                Destroy(abilityInstance);
            }
        }
        RecalculateStats();
    }

    /// <summary>
    /// 모든 활성화된 능력들의 효과를 재적용하여 플레이어 스탯을 재계산합니다.
    /// </summary>
    public void RecalculateStats()
    {
        // === 기본 스탯 초기화 ===
        MaxHealth = 100f;
        AttackDamage = 10f;
        Defense = 0f;
        MoveSpeed = 5f;
        AttackSpeed = 1f;
        AttackRange = 5f;
        AttackSize = 1f;
        ProjectileSpeed = 10f;
        CriticalRate = 0.1f;

        // === 모든 활성화된 능력들의 효과 적용 ===
        foreach (var entry in activeAbilities)
        {
            Ability ability = entry.Value;
            if (ability != null)
            {
                ability.ApplyEffect();
            }
        }

        // === 최종 스탯 계산 ===
        MaxAttackSpeed = AttackSpeed;

        // 최대 체력이 변경되었을 때 현재 체력이 새로운 최대 체력을 초과하지 않도록 조정
        Health = Mathf.Min(Health, MaxHealth);

        Debug.Log($"Player Stats Recalculated: MaxHealth={MaxHealth}, CurrentHealth={Health}, Damage={AttackDamage}, Defense={Defense}, MoveSpeed={MoveSpeed}, Speed={MaxAttackSpeed}, Range={AttackRange}");
    }

    //public void SetInvincibleState(bool state)
    //{
    //    isInvincible = state;
    //}
    //public bool IsInvincible()
    //{
    //    return isInvincible;
    //}

    /// <summary>
    /// 플레이어가 데미지를 받도록 합니다.
    /// </summary>
    public void TakeDamage(float damageAmount, GameObject damageSource = null)
    {
        // 무적 상태 체크 코드를 제거했습니다. 이제 사망하거나 데미지 0일 때만 데미지를 무시합니다.
        if (isDead || damageAmount <= 0)
        {
            Debug.Log($"Player 사망했거나 데미지 0. 데미지 적용 안됨. (소스: {damageSource?.name})");
            return;
        }

        float finalDamage = damageAmount / (1f + Defense);
        Health -= finalDamage;
        Health = Mathf.Max(Health, 0f);

        Debug.Log($"플레이어가 {damageAmount} 데미지를 받았습니다. (방어력 적용 후 최종 데미지: {finalDamage}) (소스: {damageSource?.name}) 현재 체력: {Health}");

        animationHandler?.Hurt();

        if (Health <= 0f)
        {
            Death(damageSource);
        }
    }

    /// <summary>
    /// 플레이어 사망 처리
    /// </summary>
    private void Death(GameObject killer = null)
    {
        if (isDead) return;

        Debug.Log($"플레이어가 사망했습니다! (킬러: {killer?.name})");
        isDead = true;
        animationHandler?.Death();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 플레이어의 체력을 회복시킵니다.
    /// </summary>
    public void Heal(float amount)
    {
        if (amount <= 0) return;

        Health += amount;
        Health = Mathf.Min(Health, MaxHealth);

        Debug.Log($"플레이어 체력 {amount} 회복. 현재 체력: {Health}");
    }

    /// <summary>
    /// 플레이어가 보유한 특수 화살 능력이 발동될 수 있는지 확인하고 발동합니다.
    /// </summary>
    public bool TryActivateSpecialArrowAbility(GameObject regularArrowGO, Arrow regularArrowScript)
    {
        foreach (var entry in activeAbilities)
        {
            FireArrow fireArrowAbility = entry.Value as FireArrow;
            if (fireArrowAbility != null)
            {
                if (fireArrowAbility.TryActivateFireArrow(regularArrowGO, regularArrowScript))
                {
                    return true;
                }
            }
        }

        foreach (var entry in activeAbilities)
        {
            GatlingBow gatlingBowAbility = entry.Value as GatlingBow;
            if (gatlingBowAbility != null)
            {
                if (gatlingBowAbility.TryActivateGatlingArrow(regularArrowGO, regularArrowScript, this))
                {
                    return true;
                }
            }
        }

        return false;
    }



    /// <summary>
    /// 현재 플레이어가 보유한 어빌리티로 합성할 수 있는 어빌리티 레시피를 찾습니다.
    /// </summary>
    /// <returns>합성 가능한 최종 어빌리티 프리팹 목록.</returns>
    public List<GameObject> GetCombinableAbilities()
    {
        List<GameObject> combinablePrefabs = new List<GameObject>();
        if (abilityRecipes == null) return combinablePrefabs;

        foreach (var recipe in abilityRecipes)
        {
            if (recipe.CanCombine(this))
            {
                combinablePrefabs.Add(recipe.CombinedAbilityPrefab);
            }
        }
        return combinablePrefabs;
    }

    /// <summary>
    /// 플레이어에게서 특정 어빌리티를 제거합니다.
    /// </summary>
    /// <param name="abilityPrefab">제거할 어빌리티의 프리팹.</param>
    public void RemoveAbility(GameObject abilityPrefab)
    {
        if (activeAbilities.ContainsKey(abilityPrefab))
        {
            Ability abilityInstance = activeAbilities[abilityPrefab];
            abilityInstance.OnRemove();
            Destroy(abilityInstance.gameObject);
            activeAbilities.Remove(abilityPrefab);
            Debug.Log($"[{abilityPrefab.name}] 어빌리티가 제거되었습니다.");
        }
    }
}