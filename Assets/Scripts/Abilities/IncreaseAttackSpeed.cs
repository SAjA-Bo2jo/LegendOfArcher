using UnityEngine;

public class IncreaseAttackSpeed : Ability
{
    // 레벨별 공격 속도 증가량 (예: 20%, 40%, 60%, 80%, 100% (총합))
    // 이 값들은 최종 AttackSpeed에 더해질 '초당 공격 횟수' 증가량으로 해석합니다.
    [SerializeField] private float[] attackSpeedIncreasePerLevel = { 0.2f, 0.4f, 0.6f, 0.8f, 1.0f }; // 예시: 0.2는 초당 공격 횟수 +0.2

    [SerializeField] private Sprite rapidfireIcon; // 인스펙터에서 할당할 아이콘 스프라이트 필드

    // 능력을 획득하거나 레벨업 할 때 호출
    public override void OnAcquire(Player playerInstance)
    {
        base.OnAcquire(playerInstance); // 부모 클래스 OnAcquire 호출 (레벨 증가)
        UpdateDescription(); // 능력 설명 업데이트
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} 획득/강화: {Description}");
    }

    // 이 능력이 활성화될 때 플레이어 스탯에 영향을 줍니다.
    // Player.RecalculateStats()에 의해 호출됩니다.
    public override void ApplyEffect()
    {
        if (player == null) return;

        // Player.RecalculateStats()에서 Player.AttackSpeed가 초기화된 후 이 메서드가 호출되므로,
        // 이전 레벨의 효과를 "제거"할 필요 없이 현재 레벨에 해당하는 효과를 "더해주면" 됩니다.
        // 이것이 RecalculateStats 패턴의 장점입니다.

        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel)
        {
            float currentBoost = attackSpeedIncreasePerLevel[CurrentLevel - 1];
            player.AttackSpeed += currentBoost; // 플레이어의 AttackSpeed에 직접 증가량 적용

            UpdateDescription(); // 설명 업데이트
            Debug.Log($"[{AbilityName}] Lv.{CurrentLevel}: 공격 속도 +{currentBoost:F2} (플레이어 AttackSpeed={player.AttackSpeed:F2})");
        }
        else if (CurrentLevel == 0)
        {
            // 능력이 아직 획득되지 않았거나 초기화된 상태
            UpdateDescription();
        }
    }

    public override void OnRemove()
    {
        base.OnRemove(); // 부모 클래스 OnRemove 호출 (CurrentLevel 초기화)
        Debug.Log($"[{AbilityName}] 효과 제거됨.");
        // 제거 시 Player.RecalculateStats()가 다시 호출되므로,
        // 여기서 직접 player.AttackSpeed를 뺄 필요는 없습니다.
        // RecalculateStats가 모든 activeAbilities를 순회하며 다시 계산하기 때문입니다.
    }

    // 능력 설명을 현재 레벨에 맞춰 업데이트
    private void UpdateDescription()
    {
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel)
        {
            float currentBoost = attackSpeedIncreasePerLevel[CurrentLevel - 1];
            Description = $"공격 속도를 {currentBoost:F2} 만큼 증가시킵니다. (현재 +{currentBoost:F2}초당 공격 횟수)";
        }
        else
        {
            Description = "공격 속도를 증가시키는 능력입니다.";
        }
    }
}