using UnityEngine;

// �����Ƽ�� ��Ÿ�����͸� �ν����Ϳ��� ������ �� �ֵ��� �մϴ�.
[CreateAssetMenu(fileName = "Longbow Upgrade", menuName = "Ability/Longbow Upgrade")]
public class LongbowUpgrade : Ability
{
    // ��� ���� �����Ƽ�� ������ �����Ÿ� ������
    [SerializeField] private float rangeIncreasePerLevel = 2f;

    /// <summary>
    /// �����Ƽ�� Ȱ��ȭ�� �� �÷��̾� ������ �����ϴ� �����Դϴ�.
    /// </summary>
    public override void ApplyEffect()
    {
        // �����Ƽ�� ���� ������ ���� ���� �����Ÿ��� ������ŵ�ϴ�.
        player.AttackRange += rangeIncreasePerLevel * CurrentLevel;
    }
}