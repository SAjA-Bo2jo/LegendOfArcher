using UnityEngine;

public class IncreaseMaxHealth : Abillity
{
    // ������ ������ �ִ� ü�·� (��: 10, 20, 30, 40, 50)
    // ApplyEffect���� CurrentLevel�� ����Ͽ� �� �������� ����մϴ�.
    [SerializeField] private float healthIncreasePerLevel = 10f;
    // �ɷ� ȹ��/������ �� ȸ���� ü�·�
    [SerializeField] private float healAmountOnAcquire = 10f;

    void Awake()
    {
        AbilityName = "�ִ� ü�� ����";
        MaxLevel = 5; // �ִ� ���� 5�� ����
        InitializeAbility(this.gameObject); // �� �ɷ��� ������ ������ �Ҵ�
    }

    // �ɷ��� ȹ���ϰų� ������ �� �� ȣ��
    public override void OnAcquire(Player playerInstance)
    {
        base.OnAcquire(playerInstance); // �θ� Ŭ���� OnAcquire ȣ�� (���� ���� �� ApplyEffect ȣ��)

        // �ɷ� ȹ��/������ �� ü�� ȸ��
        if (player != null)
        {
            player.Heal(healAmountOnAcquire);
            Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} ȹ��/��ȭ: ü�� {healAmountOnAcquire} ȸ����.");
        }
        UpdateDescription(); // �ɷ� ���� ������Ʈ
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} ȹ��/��ȭ: {Description}");
    }

    // �ɷ��� ���ŵ� �� ȣ�� (��: �ռ����� ����� ��)
    public override void OnRemove()
    {
        base.OnRemove(); // �θ� Ŭ���� OnRemove ȣ�� (CurrentLevel �ʱ�ȭ)
        // RemoveEffect()�� Player�� RecalculateStats()�� ���� ���������� ó���ǹǷ� ���⼭ ���� ������ �ǵ��� �ʿ�� �����ϴ�.
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

        // Player�� RecalculateStats()�� ȣ��� �� ��� ������ �ʱ�ȭ�� �� �� �޼��尡 ȣ��ǹǷ�,
        // ���� ������ �ش��ϴ� �� �������� �����ָ� �˴ϴ�.
        // ���� ���, Lv1�̸� 10, Lv2�̸� 20, Lv3�̸� 30�� ���մϴ�.
        player.MaxHealth += healthIncreasePerLevel * CurrentLevel;

        UpdateDescription(); // ���� ������Ʈ
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} ȿ�� ����: �ִ� ü�� {healthIncreasePerLevel * CurrentLevel} ����.");
    }

    // �� �ɷ��� ȿ���� �����ϴ� �޼���
    public override void RemoveEffect()
    {
        // Player�� RecalculateStats()�� ������ �ʱ�ȭ�ϰ� ��� Ȱ��ȭ�� �ɷ��� ApplyEffect�� �ٽ� ȣ���ϹǷ�,
        // �� RemoveEffect������ Ư���� ������ �ǵ��� �ʿ䰡 �����ϴ�.
        // ���� RecalculateStats()�� ���� ����� ���⼭ player.MaxHealth -= healthIncreasePerLevel * CurrentLevel; �� ���� �ǵ����� �մϴ�.
    }

    // �ɷ� ������ ���� ������ ���� ������Ʈ
    private void UpdateDescription()
    {
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel)
        {
            Description = $"�ִ� ü�� {healthIncreasePerLevel * CurrentLevel} ���� �� �ɷ� ȹ��/������ �� ü�� {healAmountOnAcquire} ȸ��.";
        }
        else
        {
            Description = "�ִ� ü���� ������Ű�� ü���� ȸ���մϴ�.";
        }
    }
}