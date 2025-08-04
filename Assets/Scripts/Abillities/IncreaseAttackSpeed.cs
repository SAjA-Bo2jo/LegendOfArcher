using UnityEngine;

public class IncreaseAttackSpeed : Abillity
{
    // ������ ���� �ӵ� ������ (��: 20%, 40%, 60%, 80%, 100% (����))
    [SerializeField] private float[] attackSpeedIncreasePercentages = { 20f, 40f, 60f, 80f, 100f };

    // �� �ɷ����� ���� ���� �÷��̾�� ����� ���� �ӵ� ������ (���� �������� ���̸� ����)
    private float currentAppliedBoost = 0f;

    void Awake()
    {
        AbilityName = "���� �ӵ� ����";
        MaxLevel = attackSpeedIncreasePercentages.Length; // �迭 ũ�⿡ ���� �ִ� ���� ����

        // �� �ɷ� �������� �ڱ� �ڽ����� �Ҵ� (�ν����Ϳ��� ���� �Ҵ� ��� �ڵ��)
        InitializeAbility(this.gameObject);
    }

    // ������ �� ȿ�� ����
    public override void ApplyEffect()
    {
        if (player == null) return;

        // ���� ������ ȿ���� �����մϴ�. (���� ����� ����)
        if (currentAppliedBoost > 0)
        {
            player.AttackSpeedMultiplier -= currentAppliedBoost;
        }

        // ���� ������ �ش��ϴ� �� �������� ����մϴ�.
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel)
        {
            currentAppliedBoost = attackSpeedIncreasePercentages[CurrentLevel - 1];
            player.AttackSpeedMultiplier += currentAppliedBoost;
            Description = $"���� �ӵ� {currentAppliedBoost}% ���� (Lv.{CurrentLevel})";
            Debug.Log($"[{AbilityName}] Lv.{CurrentLevel}: ���� �ӵ� {currentAppliedBoost}% ���� (�÷��̾� �� {player.AttackSpeedMultiplier}%)");
        }
    }

    // �ɷ��� ���ŵ� �� ȿ���� �ǵ����ϴ�. (�ռ� ������ ����� ��)
    public override void RemoveEffect()
    {
        if (player == null) return;

        // ����Ǿ� �ִ� ���� �ӵ� �������� �ǵ����ϴ�.
        if (currentAppliedBoost > 0)
        {
            player.AttackSpeedMultiplier -= currentAppliedBoost;
            currentAppliedBoost = 0f;
            Debug.Log($"[{AbilityName}] ȿ�� ����: ���� �ӵ� {player.AttackSpeedMultiplier}%�� ����.");
        }
    }
}