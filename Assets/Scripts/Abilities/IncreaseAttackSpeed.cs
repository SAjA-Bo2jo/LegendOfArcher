using UnityEngine;

public class IncreaseAttackSpeed : Ability
{
    // ������ ���� �ӵ� ������ (��: 20%, 40%, 60%, 80%, 100% (����))
    // �� ������ ���� AttackSpeed�� ������ '�ʴ� ���� Ƚ��' ���������� �ؼ��մϴ�.
    [SerializeField] private float[] attackSpeedIncreasePerLevel = { 0.2f, 0.4f, 0.6f, 0.8f, 1.0f }; // ����: 0.2�� �ʴ� ���� Ƚ�� +0.2

    [SerializeField] private Sprite rapidfireIcon; // �ν����Ϳ��� �Ҵ��� ������ ��������Ʈ �ʵ�

    // �ɷ��� ȹ���ϰų� ������ �� �� ȣ��
    public override void OnAcquire(Player playerInstance)
    {
        base.OnAcquire(playerInstance); // �θ� Ŭ���� OnAcquire ȣ�� (���� ����)
        UpdateDescription(); // �ɷ� ���� ������Ʈ
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} ȹ��/��ȭ: {Description}");
    }

    // �� �ɷ��� Ȱ��ȭ�� �� �÷��̾� ���ȿ� ������ �ݴϴ�.
    // Player.RecalculateStats()�� ���� ȣ��˴ϴ�.
    public override void ApplyEffect()
    {
        if (player == null) return;

        // Player.RecalculateStats()���� Player.AttackSpeed�� �ʱ�ȭ�� �� �� �޼��尡 ȣ��ǹǷ�,
        // ���� ������ ȿ���� "����"�� �ʿ� ���� ���� ������ �ش��ϴ� ȿ���� "�����ָ�" �˴ϴ�.
        // �̰��� RecalculateStats ������ �����Դϴ�.

        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel)
        {
            float currentBoost = attackSpeedIncreasePerLevel[CurrentLevel - 1];
            player.AttackSpeed += currentBoost; // �÷��̾��� AttackSpeed�� ���� ������ ����

            UpdateDescription(); // ���� ������Ʈ
            Debug.Log($"[{AbilityName}] Lv.{CurrentLevel}: ���� �ӵ� +{currentBoost:F2} (�÷��̾� AttackSpeed={player.AttackSpeed:F2})");
        }
        else if (CurrentLevel == 0)
        {
            // �ɷ��� ���� ȹ����� �ʾҰų� �ʱ�ȭ�� ����
            UpdateDescription();
        }
    }

    public override void OnRemove()
    {
        base.OnRemove(); // �θ� Ŭ���� OnRemove ȣ�� (CurrentLevel �ʱ�ȭ)
        Debug.Log($"[{AbilityName}] ȿ�� ���ŵ�.");
        // ���� �� Player.RecalculateStats()�� �ٽ� ȣ��ǹǷ�,
        // ���⼭ ���� player.AttackSpeed�� �� �ʿ�� �����ϴ�.
        // RecalculateStats�� ��� activeAbilities�� ��ȸ�ϸ� �ٽ� ����ϱ� �����Դϴ�.
    }

    // �ɷ� ������ ���� ������ ���� ������Ʈ
    private void UpdateDescription()
    {
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel)
        {
            float currentBoost = attackSpeedIncreasePerLevel[CurrentLevel - 1];
            Description = $"���� �ӵ��� {currentBoost:F2} ��ŭ ������ŵ�ϴ�. (���� +{currentBoost:F2}�ʴ� ���� Ƚ��)";
        }
        else
        {
            Description = "���� �ӵ��� ������Ű�� �ɷ��Դϴ�.";
        }
    }
}