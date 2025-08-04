using UnityEngine;

public abstract class Abillity : MonoBehaviour
{
    // �ɷ��� ����� �÷��̾� ����
    protected Player player;
    // �ִϸ��̼� �ڵ鷯 (���� ����, �ʿ�� ���� Ŭ�������� ���)
    protected AnimationHandler animationHandler;
    // ���� Ÿ�� (�Ϻ� �ɷ¿��� �ʿ��� �� ����)
    protected GameObject target;

    // ������ ���� �ð� (Bow ��ũ��Ʈ���� ���)
    public float lastAttackTime = -999f;

    // ���� �ɷ� ����
    public int CurrentLevel { get; protected set; } = 0;
    // �ִ� ����
    public int MaxLevel { get; protected set; } = 5;

    // �� �ɷ��� Ȱ��ȭ�� �� ������ ������ (�ɷ� ���� UI ��� Ȱ��)
    public GameObject AbilityPrefab { get; private set; }

    // �� �ɷ¿� ���� ����, ������ ���� ���� ������
    public string AbilityName { get; protected set; }
    public string Description { get; protected set; }
    public Sprite AbilityIcon { get; protected set; } // <--- �� ���� �߰��մϴ�.

    // �ɷ��� ������ �� ȣ��Ǹ�, �� �ɷ��� ������ ������ �Ҵ��մϴ�.
    public virtual void InitializeAbility(GameObject prefab)
    {
        AbilityPrefab = prefab;
    }

    // �ɷ��� ȹ���ϰų� ������ �� �� ȣ��� �޼���
    public virtual void OnAcquire(Player playerInstance)
    {
        this.player = playerInstance; // �÷��̾� �ν��Ͻ� �Ҵ�
        CurrentLevel++; // ���� ����
        ApplyEffect(); // ȿ�� ����
    }

    // �ɷ��� ���ŵ� �� (��: �ռ����� ����� ��) ȣ��� �޼���
    public virtual void OnRemove()
    {
        CurrentLevel = 0; // ���� �ʱ�ȭ
    }

    // �� �ɷ��� ���� ȿ���� �����ϴ� �޼��� (���� Ŭ�������� �ݵ�� ����)
    public abstract void ApplyEffect();
    // �� �ɷ��� ȿ���� �����ϴ� �޼��� (���� Ŭ�������� �ݵ�� ����)

    // ���������� �� �ɷ� Ŭ�������� Update ������ ���� �� �ֽ��ϴ�.
    // ������, Player �Ǵ� PlayerController���� �� �޼��带 ȣ�����־�� �մϴ�.
    // �Ǵ� �� Abillity�� MonoBehaviour�� ����� GameObject�� �ٿ��� ��ü Update�� ����� ���� �ֽ��ϴ�.
    // �� ���ÿ����� MonoBehaviour�� ��ӹ����Ƿ� ��ü Update�� ����� �� �ֽ��ϴ�.
    public virtual void UpdateAbility() { }
}