using UnityEngine;

// 어빌리티의 메타데이터를 인스펙터에서 편집할 수 있도록 합니다.
[CreateAssetMenu(fileName = "Sniper Slingshot", menuName = "Ability/Sniper Slingshot")]
public class SniperSlingshot : Ability
{
    // 저격 새총 어빌리티의 효과
    [SerializeField] private float damageMultiplier = 2f; // 데미지 증가 배율
    [SerializeField] private float rangeIncrease = 10f; // 추가 사정거리 증가량

    public override void ApplyEffect()
    {
        // 합성 어빌리티는 보통 단일 레벨이므로, 레벨에 따른 증가 로직은 추가하지 않았습니다.
        // 필요에 따라 레벨별 효과를 다르게 구현할 수 있습니다.
        player.AttackDamage *= damageMultiplier;
        player.AttackRange += rangeIncrease;
    }
}