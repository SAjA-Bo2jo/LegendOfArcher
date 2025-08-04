using UnityEngine;

public class IncreaseMaxHealth : Ability
{
    // 레벨별 증가할 최대 체력량 (예: 10, 20, 30, 40, 50)
    // ApplyEffect에서 CurrentLevel을 사용하여 총 증가량을 계산합니다.
    [SerializeField] private float healthIncreasePerLevel = 10f; // 각 레벨당 추가될 기본 체력량

    // 능력 획득/레벨업 시 회복될 체력량
    [SerializeField] private float healAmountOnAcquire = 10f;

    [SerializeField] private Sprite giantsHeartIconSprite; // 인스펙터에서 할당할 아이콘 스프라이트 필드

    void Awake()
    {
        AbilityName = "거인의 심장";
        MaxLevel = 5; // 예시: 최대 레벨 5
        InitializeAbility(this.gameObject);
        AbilityIcon = giantsHeartIconSprite;
        UpdateDescription(); // 설명 초기화
    }

    // 능력을 획득하거나 레벨업 할 때 호출
    public override void OnAcquire(Player playerInstance)
    {
        base.OnAcquire(playerInstance); // 부모 클래스의 OnAcquire 호출 (CurrentLevel 증가 및 player 참조 설정)

        if (player != null)
        {
            // 체력 증가 효과가 적용된 후 (RecalculateStats -> ApplyEffect 호출 후),
            // 플레이어 체력을 회복합니다. 이 회복량은 새로운 최대 체력을 존중해야 합니다.
            player.Heal(healAmountOnAcquire);
            Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} 획득/강화: 체력 {healAmountOnAcquire} 회복 시도됨.");
        }
        UpdateDescription();
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} 획득/강화 완료: {Description}");
    }

    // 각 능력의 실제 효과를 적용하는 메서드
    // Player.RecalculateStats()에 의해 호출됩니다.
    public override void ApplyEffect()
    {
        if (player == null)
        {
            Debug.LogWarning($"[{AbilityName}] Player 참조가 없습니다. 효과를 적용할 수 없습니다.");
            return;
        }

        // Player의 RecalculateStats()가 호출될 때 Player.Health가 기본값으로 초기화된 후,
        // 모든 활성화된 능력의 ApplyEffect가 호출됩니다.
        // 따라서, 현재 레벨에 해당하는 총 증가량을 Health에 더해주면 됩니다.
        // 예를 들어, Lv1이면 +10, Lv2이면 +20 (총 누적 증가량)
        player.Health += healthIncreasePerLevel * CurrentLevel;

        UpdateDescription(); // 효과 적용 후 설명 업데이트
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} 효과 적용: 플레이어 최대 체력 {healthIncreasePerLevel * CurrentLevel} 증가됨. (총 {player.Health})");
    }

    public override void OnRemove()
    {
        base.OnRemove(); // 부모 클래스의 OnRemove 호출 (CurrentLevel 초기화)
        Debug.Log($"[{AbilityName}] 효과 제거됨.");
        // 능력이 제거된 후 Player.RecalculateStats()가 다시 호출되므로,
        // 여기서 수동으로 체력을 뺄 필요는 없습니다.
    }

    // 능력 설명을 현재 레벨에 맞춰 업데이트
    private void UpdateDescription()
    {
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel)
        {
            // 현재 레벨에 대한 총 체력 증가량 계산
            float totalHealthIncrease = healthIncreasePerLevel * CurrentLevel;
            Description = $"최대 체력 {totalHealthIncrease} 증가 및 능력 획득/레벨업 시 체력 {healAmountOnAcquire} 회복.";
        }
        else
        {
            Description = "최대 체력을 증가시키고 체력을 회복합니다.";
        }
    }
}