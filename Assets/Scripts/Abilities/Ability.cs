// Ability.cs
using UnityEngine;

public class Ability : MonoBehaviour
{
    [Header("Base Ability Info")]
    public string AbilityName;
    [TextArea(3, 5)]
    public string Description;
    public Sprite AbilityIcon;
    public GameObject AbilityPrefab;

    // ��� �����Ƽ���� �������� ���� ������
    public Player player; // ���� public���� �����Ͽ� �ܺο��� ���� �Ҵ� ����
    protected GameObject target;
    public int CurrentLevel { get; protected set; }
    public int MaxLevel = 3;

    // Start()�� virtual�� �ξ� �ڽ� Ŭ�������� �������� �� �ְ� �մϴ�.
    protected virtual void Start()
    {
        // �� �̻� GetComponentInParent�� ������� �ʽ��ϴ�.
    }

    // �ܺο��� �÷��̾� ������ �����ϴ� �޼���
    public void SetPlayer(Player playerInstance)
    {
        this.player = playerInstance;
    }

    public void InitializeAbility(GameObject abilityPrefab)
    {
        this.AbilityPrefab = abilityPrefab;
        CurrentLevel = 1;
        Debug.Log($"InitializeAbility ȣ���: {this.AbilityName}, ���� ����: {CurrentLevel}");
    }

    public virtual void OnAcquire(Player player)
    {
        CurrentLevel++;
        Debug.Log($"OnAcquire ȣ���: {this.AbilityName}, ������ ��: {CurrentLevel}");
    }

    public virtual void ApplyEffect()
    {
        // ���� �����Ƽ�� ���� �ٸ� ȿ���� �����ϴ� ����
    }

    public virtual void OnRemove()
    {
        Debug.Log($"OnRemove ȣ���: {this.AbilityName}");
    }
}