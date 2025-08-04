using UnityEngine;

public class IncreaseMaxHealth : Ability
{
    // ������ ������ �ִ� ü�·� (��: 10, 20, 30, 40, 50)
    // ApplyEffect���� CurrentLevel�� ����Ͽ� �� �������� ����մϴ�.
    [SerializeField] private float healthIncreasePerLevel = 10f; // �� ������ �߰��� �⺻ ü�·�

    // �ɷ� ȹ��/������ �� ȸ���� ü�·�
    [SerializeField] private float healAmountOnAcquire = 10f;

    [SerializeField] private Sprite giantsHeartIconSprite; // �ν����Ϳ��� �Ҵ��� ������ ��������Ʈ �ʵ�

    void Awake()
    {
        AbilityName = "������ ����";
        MaxLevel = 5; // ����: �ִ� ���� 5
        InitializeAbility(this.gameObject);
        AbilityIcon = giantsHeartIconSprite;
        UpdateDescription(); // ���� �ʱ�ȭ
    }

    // �ɷ��� ȹ���ϰų� ������ �� �� ȣ��
    public override void OnAcquire(Player playerInstance)
    {
        base.OnAcquire(playerInstance); // �θ� Ŭ������ OnAcquire ȣ�� (CurrentLevel ���� �� player ���� ����)

        if (player != null)
        {
            // ü�� ���� ȿ���� ����� �� (RecalculateStats -> ApplyEffect ȣ�� ��),
            // �÷��̾� ü���� ȸ���մϴ�. �� ȸ������ ���ο� �ִ� ü���� �����ؾ� �մϴ�.
            player.Heal(healAmountOnAcquire);
            Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} ȹ��/��ȭ: ü�� {healAmountOnAcquire} ȸ�� �õ���.");
        }
        UpdateDescription();
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} ȹ��/��ȭ �Ϸ�: {Description}");
    }

    // �� �ɷ��� ���� ȿ���� �����ϴ� �޼���
    // Player.RecalculateStats()�� ���� ȣ��˴ϴ�.
    public override void ApplyEffect()
    {
        if (player == null)
        {
            Debug.LogWarning($"[{AbilityName}] Player ������ �����ϴ�. ȿ���� ������ �� �����ϴ�.");
            return;
        }

        // Player�� RecalculateStats()�� ȣ��� �� Player.Health�� �⺻������ �ʱ�ȭ�� ��,
        // ��� Ȱ��ȭ�� �ɷ��� ApplyEffect�� ȣ��˴ϴ�.
        // ����, ���� ������ �ش��ϴ� �� �������� Health�� �����ָ� �˴ϴ�.
        // ���� ���, Lv1�̸� +10, Lv2�̸� +20 (�� ���� ������)
        player.Health += healthIncreasePerLevel * CurrentLevel;

        UpdateDescription(); // ȿ�� ���� �� ���� ������Ʈ
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} ȿ�� ����: �÷��̾� �ִ� ü�� {healthIncreasePerLevel * CurrentLevel} ������. (�� {player.Health})");
    }

    public override void OnRemove()
    {
        base.OnRemove(); // �θ� Ŭ������ OnRemove ȣ�� (CurrentLevel �ʱ�ȭ)
        Debug.Log($"[{AbilityName}] ȿ�� ���ŵ�.");
        // �ɷ��� ���ŵ� �� Player.RecalculateStats()�� �ٽ� ȣ��ǹǷ�,
        // ���⼭ �������� ü���� �� �ʿ�� �����ϴ�.
    }

    // �ɷ� ������ ���� ������ ���� ������Ʈ
    private void UpdateDescription()
    {
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel)
        {
            // ���� ������ ���� �� ü�� ������ ���
            float totalHealthIncrease = healthIncreasePerLevel * CurrentLevel;
            Description = $"�ִ� ü�� {totalHealthIncrease} ���� �� �ɷ� ȹ��/������ �� ü�� {healAmountOnAcquire} ȸ��.";
        }
        else
        {
            Description = "�ִ� ü���� ������Ű�� ü���� ȸ���մϴ�.";
        }
    }
}