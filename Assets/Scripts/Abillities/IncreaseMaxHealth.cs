using UnityEngine;

public class IncreaseMaxHealth : Abillity
{
    // 레벨별 증가할 최대 체력량 (예: 10, 20, 30, 40, 50)
    // ApplyEffect에서 CurrentLevel을 사용하여 총 증가량을 계산합니다.
    [SerializeField] private float healthIncreasePerLevel = 10f;
    // 능력 획득/레벨업 시 회복될 체력량
    [SerializeField] private float healAmountOnAcquire = 10f;

    void Awake()
    {
        AbilityName = "최대 체력 증가";
        MaxLevel = 5; // 최대 레벨 5로 설정
        InitializeAbility(this.gameObject); // 이 능력의 프리팹 정보를 할당
    }

    // 능력을 획득하거나 레벨업 할 때 호출
    public override void OnAcquire(Player playerInstance)
    {
        base.OnAcquire(playerInstance); // 부모 클래스 OnAcquire 호출 (레벨 증가 및 ApplyEffect 호출)

        // 능력 획득/레벨업 시 체력 회복
        if (player != null)
        {
            player.Heal(healAmountOnAcquire);
            Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} 획득/강화: 체력 {healAmountOnAcquire} 회복됨.");
        }
        UpdateDescription(); // 능력 설명 업데이트
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} 획득/강화: {Description}");
    }

    // 능력이 제거될 때 호출 (예: 합성으로 사라질 때)
    public override void OnRemove()
    {
        base.OnRemove(); // 부모 클래스 OnRemove 호출 (CurrentLevel 초기화)
        // RemoveEffect()는 Player의 RecalculateStats()에 의해 간접적으로 처리되므로 여기서 직접 스탯을 되돌릴 필요는 없습니다.
        Debug.Log($"[{AbilityName}] 효과 제거됨.");
    }

    // 각 능력의 실제 효과를 적용하는 메서드
    public override void ApplyEffect()
    {
        if (player == null)
        {
            Debug.LogWarning($"[{AbilityName}] Player 참조가 없습니다. 효과를 적용할 수 없습니다.");
            return;
        }

        // Player의 RecalculateStats()가 호출될 때 모든 스탯이 초기화된 후 이 메서드가 호출되므로,
        // 현재 레벨에 해당하는 총 증가량을 더해주면 됩니다.
        // 예를 들어, Lv1이면 10, Lv2이면 20, Lv3이면 30을 더합니다.
        player.MaxHealth += healthIncreasePerLevel * CurrentLevel;

        UpdateDescription(); // 설명 업데이트
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} 효과 적용: 최대 체력 {healthIncreasePerLevel * CurrentLevel} 증가.");
    }

    // 각 능력의 효과를 제거하는 메서드
    public override void RemoveEffect()
    {
        // Player의 RecalculateStats()가 스탯을 초기화하고 모든 활성화된 능력의 ApplyEffect를 다시 호출하므로,
        // 이 RemoveEffect에서는 특별히 스탯을 되돌릴 필요가 없습니다.
        // 만약 RecalculateStats()가 없는 경우라면 여기서 player.MaxHealth -= healthIncreasePerLevel * CurrentLevel; 와 같이 되돌려야 합니다.
    }

    // 능력 설명을 현재 레벨에 맞춰 업데이트
    private void UpdateDescription()
    {
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel)
        {
            Description = $"최대 체력 {healthIncreasePerLevel * CurrentLevel} 증가 및 능력 획득/레벨업 시 체력 {healAmountOnAcquire} 회복.";
        }
        else
        {
            Description = "최대 체력을 증가시키고 체력을 회복합니다.";
        }
    }
}