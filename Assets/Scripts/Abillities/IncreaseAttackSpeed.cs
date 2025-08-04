using UnityEngine;

public class IncreaseAttackSpeed : Abillity
{
    // ������ ���� �ӵ� ������ (��: 20%, 40%, 60%, 80%, 100% (����))
    [SerializeField] private float[] attackSpeedIncreasePercentages = { 20f, 40f, 60f, 80f, 100f };

    [SerializeField] private Sprite rapidfireIcon; // <--- �ν����Ϳ��� �Ҵ��� ������ ��������Ʈ �ʵ�

    // �� �ɷ����� ���� ���� �÷��̾�� ����� ���� �ӵ� ������ (���� �������� ���̸� ����)
    private float currentAppliedBoost = 0f;

    void Awake()
    {
        AbilityName = "��� ����";
        MaxLevel = attackSpeedIncreasePercentages.Length; // �迭 ũ�⿡ ���� �ִ� ���� ����

        // �� �ɷ� �������� �ڱ� �ڽ����� �Ҵ� (�ν����Ϳ��� ���� �Ҵ� ��� �ڵ��)
        InitializeAbility(this.gameObject);
        AbilityIcon = rapidfireIcon; // <--- �� ���� �߰��մϴ�.
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

    public override void OnRemove()
    {
        base.OnRemove(); // �θ� Ŭ���� OnRemove ȣ�� (CurrentLevel �ʱ�ȭ)
        Debug.Log($"[{AbilityName}] ȿ�� ���ŵ�.");
    }
}