using UnityEngine;

public class IncreaseCriticalRateAbility : Abillity
{
    // 레벨당 증가할 치명타 확률 (%)
    [SerializeField] private float criticalRateIncreasePerLevel = 5f;

    void Awake()
    {
        AbilityName = "치명타 확률 증가";
        MaxLevel = 5; // 최대 레벨 5로 설정 (선택 사항, 필요에 따라 조절)
        InitializeAbility(this.gameObject); // 이 능력의 프리팹 정보를 할당
    }

    // 능력을 획득하거나 레벨업 할 때 호출
    public override void OnAcquire(Player playerInstance)
    {
        base.OnAcquire(playerInstance); // 부모 클래스 OnAcquire 호출 (레벨 증가 및 ApplyEffect 호출)
        UpdateDescription(); // 능력 설명 업데이트
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} 획득/강화: {Description}");
    }

    // 능력이 제거될 때 호출
    public override void OnRemove()
    {
        base.OnRemove(); // 부모 클래스 OnRemove 호출 (CurrentLevel 초기화)
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

        // Player의 RecalculateStats()가 호출될 때 모든 스탯이 기본값으로 초기화된 후
        // 이 메서드가 호출되므로, 현재 레벨에 해당하는 총 증가량을 더해주면 됩니다.
        // 예를 들어, Lv1이면 5%, Lv2이면 10%를 더합니다.
        player.CriticalRate += criticalRateIncreasePerLevel * CurrentLevel;

        UpdateDescription(); // 설명 업데이트
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} 효과 적용: 치명타 확률 {criticalRateIncreasePerLevel * CurrentLevel}% 증가.");
    }

    // 각 능력의 효과를 제거하는 메서드 (RecalculateStats()가 처리하므로 여기서는 빈 상태)
    public override void RemoveEffect()
    {
        // Player의 RecalculateStats()가 스탯을 초기화하고 모든 활성화된 능력의 ApplyEffect를 다시 호출하므로,
        // 이 RemoveEffect에서는 특별히 스탯을 되돌릴 필요가 없습니다.
    }

    // 능력 설명을 현재 레벨에 맞춰 업데이트
    private void UpdateDescription()
    {
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel)
        {
            Description = $"치명타 확률 {criticalRateIncreasePerLevel * CurrentLevel}% 증가.";
        }
        else
        {
            Description = "치명타 확률을 증가시킵니다.";
        }
    }
}