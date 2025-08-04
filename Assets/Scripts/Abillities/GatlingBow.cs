// GatlingBow.cs
using UnityEngine;

public class GatlingBow : Abillity
{
    [SerializeField] private float gatlingBowAttackSpeedBonus = 200f; // 개틀링 보우의 추가 공격 속도 보너스
    [SerializeField] private Sprite gatlingBowIcon;
    void Awake()
    {
        AbilityName = "개틀링 보우"; // 능력 이름 설정
        MaxLevel = 1; // 합성 능력은 일반적으로 단일 레벨 (최대 레벨)
        Description = $"궁극의 고속 연사력! 공격 속도가 {gatlingBowAttackSpeedBonus}% 추가 증가합니다."; // 설명

        // 여기에 개틀링 보우 아이콘 스프라이트 필드를 추가하고 할당해야 합니다.

        AbilityIcon = gatlingBowIcon;
    }

    public override void ApplyEffect()
    {
        // 이 능력을 획득하면 플레이어의 공격 속도 배율을 크게 증가시킵니다.
        // (이전에 불화살, 고속연사에서 적용된 배율은 이미 제거되었거나 다른 방식으로 관리될 것입니다.)
        if (player == null) return;

        // Player의 AttackSpeedMultiplier는 누적되므로, 여기에 직접 추가합니다.
        // 이전 능력들(불화살, 고속연사)이 제거되면서 그들의 효과도 RecalculateStats()에 의해 초기화되거나 제거되었을 것입니다.
        player.AttackSpeedMultiplier += gatlingBowAttackSpeedBonus;
        Debug.Log($"[{AbilityName}] 효과 적용: 공격 속도 {gatlingBowAttackSpeedBonus}% 증가 (총 {player.AttackSpeedMultiplier}%)");
    }

    public override void OnRemove()
    {
        // 합성 능력은 일반적으로 게임 도중 제거되지 않으므로, 이 메서드는 비워두거나 필요시 로직을 추가합니다.
        // 예를 들어, 게임 오버 시 모든 능력 초기화 로직 등.
        Debug.Log($"[{AbilityName}] 효과 제거됨 (합성 능력은 일반적으로 제거되지 않습니다).");
        // player.AttackSpeedMultiplier -= gatlingBowAttackSpeedBonus; // 만약 제거 시 효과를 되돌린다면
    }
}