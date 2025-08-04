// GatlingBow.cs
using UnityEngine;

public class GatlingBow : Abillity
{
    [SerializeField] private float gatlingBowAttackSpeedBonus = 200f; // ��Ʋ�� ������ �߰� ���� �ӵ� ���ʽ�
    [SerializeField] private Sprite gatlingBowIcon;
    void Awake()
    {
        AbilityName = "��Ʋ�� ����"; // �ɷ� �̸� ����
        MaxLevel = 1; // �ռ� �ɷ��� �Ϲ������� ���� ���� (�ִ� ����)
        Description = $"�ñ��� ��� �����! ���� �ӵ��� {gatlingBowAttackSpeedBonus}% �߰� �����մϴ�."; // ����

        // ���⿡ ��Ʋ�� ���� ������ ��������Ʈ �ʵ带 �߰��ϰ� �Ҵ��ؾ� �մϴ�.

        AbilityIcon = gatlingBowIcon;
    }

    public override void ApplyEffect()
    {
        // �� �ɷ��� ȹ���ϸ� �÷��̾��� ���� �ӵ� ������ ũ�� ������ŵ�ϴ�.
        // (������ ��ȭ��, ��ӿ��翡�� ����� ������ �̹� ���ŵǾ��ų� �ٸ� ������� ������ ���Դϴ�.)
        if (player == null) return;

        // Player�� AttackSpeedMultiplier�� �����ǹǷ�, ���⿡ ���� �߰��մϴ�.
        // ���� �ɷµ�(��ȭ��, ��ӿ���)�� ���ŵǸ鼭 �׵��� ȿ���� RecalculateStats()�� ���� �ʱ�ȭ�ǰų� ���ŵǾ��� ���Դϴ�.
        player.AttackSpeedMultiplier += gatlingBowAttackSpeedBonus;
        Debug.Log($"[{AbilityName}] ȿ�� ����: ���� �ӵ� {gatlingBowAttackSpeedBonus}% ���� (�� {player.AttackSpeedMultiplier}%)");
    }

    public override void OnRemove()
    {
        // �ռ� �ɷ��� �Ϲ������� ���� ���� ���ŵ��� �����Ƿ�, �� �޼���� ����ΰų� �ʿ�� ������ �߰��մϴ�.
        // ���� ���, ���� ���� �� ��� �ɷ� �ʱ�ȭ ���� ��.
        Debug.Log($"[{AbilityName}] ȿ�� ���ŵ� (�ռ� �ɷ��� �Ϲ������� ���ŵ��� �ʽ��ϴ�).");
        // player.AttackSpeedMultiplier -= gatlingBowAttackSpeedBonus; // ���� ���� �� ȿ���� �ǵ����ٸ�
    }
}