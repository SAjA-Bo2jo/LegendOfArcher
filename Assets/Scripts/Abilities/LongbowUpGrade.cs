using UnityEngine;

// 어빌리티의 메타데이터를 인스펙터에서 편집할 수 있도록 합니다.
[CreateAssetMenu(fileName = "Longbow Upgrade", menuName = "Ability/Longbow Upgrade")]
public class LongbowUpgrade : Ability
{
    // 장궁 개조 어빌리티의 레벨당 사정거리 증가량
    [SerializeField] private float rangeIncreasePerLevel = 2f;

    /// <summary>
    /// 어빌리티가 활성화될 때 플레이어 스탯을 변경하는 로직입니다.
    /// </summary>
    public override void ApplyEffect()
    {
        // 어빌리티의 현재 레벨에 따라 공격 사정거리를 증가시킵니다.
        player.AttackRange += rangeIncreasePerLevel * CurrentLevel;
    }
}