using UnityEngine;

public class IncreaseMaxHealth : Abillity
{
    // ������ ������ �ִ� ü�·� (��: 10, 20, 30, 40, 50)
    // ApplyEffect���� CurrentLevel�� ����Ͽ� �� �������� ����մϴ�.
    [SerializeField] private float healthIncreasePerLevel = 10f;

    // �ɷ� ȹ��/������ �� ȸ���� ü�·�
    [SerializeField] private float healAmountOnAcquire = 10f;

    [SerializeField] private Sprite giantsHeartIconSprite; // <--- �ν����Ϳ��� �Ҵ��� ������ ��������Ʈ �ʵ�

    void Awake()
    {
        AbilityName = "������ ����"; // "�ִ� ü�� ����" ��� ��ü���� �̸�
        MaxLevel = 5;
        InitializeAbility(this.gameObject);
        AbilityIcon = giantsHeartIconSprite; // <--- ������ �Ҵ�
    }

    // �ɷ��� ȹ���ϰų� ������ �� �� ȣ��
    public override void OnAcquire(Player playerInstance)
    {
        base.OnAcquire(playerInstance);
        if (player != null)
        {
            player.Heal(healAmountOnAcquire);
            Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} ȹ��/��ȭ: ü�� {healAmountOnAcquire} ȸ����.");
        }
        UpdateDescription();
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} ȹ��/��ȭ: {Description}");
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