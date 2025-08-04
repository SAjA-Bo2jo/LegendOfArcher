using UnityEngine;

public class IncreaseCriticalRateAbility : Ability
{
    // ������ ������ ġ��Ÿ Ȯ�� (%)
    [SerializeField] private float criticalRateIncreasePerLevel = 5f;

    // �ɷ��� ȹ���ϰų� ������ �� �� ȣ��
    public override void OnAcquire(Player playerInstance)
    {
        base.OnAcquire(playerInstance);
        UpdateDescription();
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} ȹ��/��ȭ: {Description}");
    }

    // �ɷ��� ���ŵ� �� ȣ��
    public override void OnRemove()
    {
        base.OnRemove(); // �θ� Ŭ���� OnRemove ȣ�� (CurrentLevel �ʱ�ȭ)
        Debug.Log($"[{AbilityName}] ȿ�� ���ŵ�.");
    }

    // �� �ɷ��� ���� ȿ���� �����ϴ� �޼���
    public override void ApplyEffect()
    {
        if (player == null)
        {
            Debug.LogWarning($"[{AbilityName}] Player ������ �����ϴ�. ȿ���� ������ �� �����ϴ�.");
            return;
        }

        // Player�� RecalculateStats()�� ȣ��� �� ��� ������ �⺻������ �ʱ�ȭ�� ��
        // �� �޼��尡 ȣ��ǹǷ�, ���� ������ �ش��ϴ� �� �������� �����ָ� �˴ϴ�.
        // ���� ���, Lv1�̸� 5%, Lv2�̸� 10%�� ���մϴ�.
        player.CriticalRate += criticalRateIncreasePerLevel * CurrentLevel;

        UpdateDescription(); // ���� ������Ʈ
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} ȿ�� ����: ġ��Ÿ Ȯ�� {criticalRateIncreasePerLevel * CurrentLevel}% ����.");
    }

    // �� �ɷ��� ȿ���� �����ϴ� �޼��� (RecalculateStats()�� ó���ϹǷ� ���⼭�� �� ����)

    // �ɷ� ������ ���� ������ ���� ������Ʈ
    private void UpdateDescription()
    {
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel)
        {
            description = $"ġ��Ÿ Ȯ�� {criticalRateIncreasePerLevel * CurrentLevel}% ����.";
        }
        else
        {
            description = "ġ��Ÿ Ȯ���� ������ŵ�ϴ�.";
        }
    }
}