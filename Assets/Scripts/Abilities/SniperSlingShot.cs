using UnityEngine;

// �����Ƽ�� ��Ÿ�����͸� �ν����Ϳ��� ������ �� �ֵ��� �մϴ�.
[CreateAssetMenu(fileName = "Sniper Slingshot", menuName = "Ability/Sniper Slingshot")]
public class SniperSlingshot : Ability
{
    // ���� ���� �����Ƽ�� ȿ��
    [SerializeField] private float damageMultiplier = 2f; // ������ ���� ����
    [SerializeField] private float rangeIncrease = 10f; // �߰� �����Ÿ� ������

    public override void ApplyEffect()
    {
        // �ռ� �����Ƽ�� ���� ���� �����̹Ƿ�, ������ ���� ���� ������ �߰����� �ʾҽ��ϴ�.
        // �ʿ信 ���� ������ ȿ���� �ٸ��� ������ �� �ֽ��ϴ�.
        player.AttackDamage *= damageMultiplier;
        player.AttackRange += rangeIncrease;
    }
}