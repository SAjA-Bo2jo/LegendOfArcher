using UnityEngine;

public class IncreaseAttackSpeed : Abillity
{
    // 레벨별 공격 속도 증가량 (예: 20%, 40%, 60%, 80%, 100% (총합))
    [SerializeField] private float[] attackSpeedIncreasePercentages = { 20f, 40f, 60f, 80f, 100f };

    // 이 능력으로 인해 현재 플레이어에게 적용된 공격 속도 증가량 (이전 레벨과의 차이를 관리)
    private float currentAppliedBoost = 0f;

    void Awake()
    {
        AbilityName = "공격 속도 증가";
        MaxLevel = attackSpeedIncreasePercentages.Length; // 배열 크기에 따라 최대 레벨 설정

        // 이 능력 프리팹을 자기 자신으로 할당 (인스펙터에서 수동 할당 대신 코드로)
        InitializeAbility(this.gameObject);
    }

    // 레벨업 시 효과 적용
    public override void ApplyEffect()
    {
        if (player == null) return;

        // 이전 레벨의 효과를 제거합니다. (누적 계산을 위해)
        if (currentAppliedBoost > 0)
        {
            player.AttackSpeedMultiplier -= currentAppliedBoost;
        }

        // 현재 레벨에 해당하는 총 증가량을 계산합니다.
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel)
        {
            currentAppliedBoost = attackSpeedIncreasePercentages[CurrentLevel - 1];
            player.AttackSpeedMultiplier += currentAppliedBoost;
            Description = $"공격 속도 {currentAppliedBoost}% 증가 (Lv.{CurrentLevel})";
            Debug.Log($"[{AbilityName}] Lv.{CurrentLevel}: 공격 속도 {currentAppliedBoost}% 증가 (플레이어 총 {player.AttackSpeedMultiplier}%)");
        }
    }

    // 능력이 제거될 때 효과를 되돌립니다. (합성 등으로 사라질 때)
    public override void RemoveEffect()
    {
        if (player == null) return;

        // 적용되어 있던 공격 속도 증가량을 되돌립니다.
        if (currentAppliedBoost > 0)
        {
            player.AttackSpeedMultiplier -= currentAppliedBoost;
            currentAppliedBoost = 0f;
            Debug.Log($"[{AbilityName}] 효과 제거: 공격 속도 {player.AttackSpeedMultiplier}%로 복원.");
        }
    }
}