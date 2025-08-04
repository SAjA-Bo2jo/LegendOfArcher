using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour
{
    // === 플레이어 스탯 ===
    [Header("Player Stats")]
    public float MaxHealth = 100f; // 플레이어의 '최대 체력'을 의미 (어빌리티에 의해 증가 가능)
    public float AttackDamage = 10f;
    public float Defense = 0f; // <-- 방어력 스탯 추가
    public float AttackSpeed = 1f; // 기본 초당 공격 횟수
    public float MaxAttackSpeed { get; private set; } // 최종 공격 속도 (AttackSpeed와 어빌리티 보너스 반영)
    public float AttackRange = 5f;
    public float AttackSize = 1f;
    public float ProjectileSpeed = 10f;
    public float CriticalRate = 0.1f;

    // === 플레이어 현재 상태 ===
    public float Health { get; private set; } // 현재 체력
    [SerializeField] private float invincibilityDuration = 0.5f; // 무적 시간 (피격 후)
    private float invincibilityEndTime = 0f; // 무적 시간이 끝나는 시간

    // === 능력 관리 ===
    public Dictionary<GameObject, Ability> activeAbilities = new Dictionary<GameObject, Ability>();

    // === 기타 참조 ===
    private PlayerController playerController;
    private AnimationHandler animationHandler;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animationHandler = GetComponent<AnimationHandler>();

        if (playerController == null)
        {
            Debug.LogError("Player: PlayerController 컴포넌트를 찾을 수 없습니다.");
        }
        if (animationHandler == null)
        {
            Debug.LogError("Player: AnimationHandler 컴포넌트를 찾을 수 없습니다.");
        }
    }

    void Start()
    {
        RecalculateStats();
        Health = MaxHealth; // 시작 시 현재 체력을 최대 체력으로 설정
    }

    void Update()
    {
        if (Time.time >= invincibilityEndTime && IsInvincible())
        {
            animationHandler?.InvincibilityEnd();
        }
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
        MaxHealth = 100f; // 기본 최대 체력
        AttackDamage = 10f;
        Defense = 0f; // <-- 기본 방어력 초기화
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
                ability.ApplyEffect(); // 각 능력의 ApplyEffect 호출하여 플레이어 스탯을 수정
            }
        }

        // === 최종 스탯 계산 ===
        MaxAttackSpeed = AttackSpeed;

        // 최대 체력이 변경되었을 때 현재 체력이 새로운 최대 체력을 초과하지 않도록 조정
        Health = Mathf.Min(Health, MaxHealth);

        Debug.Log($"Player Stats Recalculated: MaxHealth={MaxHealth}, CurrentHealth={Health}, Damage={AttackDamage}, Defense={Defense}, Speed={MaxAttackSpeed}, Range={AttackRange}");
    }

    /// <summary>
    /// 플레이어가 데미지를 받도록 합니다.
    /// </summary>
    /// <param name="damageAmount">받을 데미지량</param>
    /// <param name="damageSource">데미지를 준 오브젝트 (선택 사항)</param>
    public void TakeDamage(float damageAmount, GameObject damageSource = null)
    {
        if (IsInvincible() || damageAmount <= 0)
        {
            Debug.Log($"Player 무적 상태이거나 데미지 0. 데미지 적용 안됨. (소스: {damageSource?.name})");
            return;
        }

        // 데미지 계산 로직에 방어력 적용
        float finalDamage = damageAmount / (damageAmount + Defense); // <-- 방어력 적용

        Health -= finalDamage;
        Health = Mathf.Max(Health, 0f);

        Debug.Log($"플레이어가 {damageAmount} 데미지를 받았습니다. (방어력 적용 후 최종 데미지: {finalDamage}) (소스: {damageSource?.name}) 현재 체력: {Health}");

        animationHandler?.Hurt();
        invincibilityEndTime = Time.time + invincibilityDuration;

        if (Health <= 0f)
        {
            Death(damageSource);
        }
    }

    /// <summary>
    /// 플레이어의 체력을 회복시킵니다.
    /// </summary>
    /// <param name="amount">회복량</param>
    public void Heal(float amount)
    {
        if (amount <= 0) return;

        Health += amount;
        Health = Mathf.Min(Health, MaxHealth);

        Debug.Log($"플레이어 체력 {amount} 회복. 현재 체력: {Health}");
    }

    /// <summary>
    /// 플레이어가 무적 상태인지 확인합니다.
    /// </summary>
    public bool IsInvincible()
    {
        return Time.time < invincibilityEndTime;
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
    /// 플레이어 사망 처리
    /// </summary>
    private void Death(GameObject killer = null)
    {
        Debug.Log($"플레이어가 사망했습니다! (킬러: {killer?.name})");
        animationHandler?.Death();
        gameObject.SetActive(false);
    }
}