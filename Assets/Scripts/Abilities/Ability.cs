using System.Linq;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    protected Player player; // �ɷ��� ����� �÷��̾� ����
    protected AnimationHandler animationHandler; // �ִϸ��̼� �ڵ鷯 (���� ����)
    protected GameObject target; // ���� Ÿ�� (�Ϻ� �ɷ¿��� �ʿ�)

    public float lastAttackTime = -999f; // ������ ���� �ð� (Ȱ ���� ���� �ɷ¿��� ���)

    public int CurrentLevel { get; protected set; } = 0; // ���� �ɷ� ����
    public int MaxLevel { get; protected set; } = 5; // �ִ� ����

    public GameObject AbilityPrefab { get; private set; } // �� �ɷ��� ������ ���� ������

    public string AbilityName { get; protected set; } // �ɷ� �̸�
    public string Description { get; protected set; } // �ɷ� ����
    public Sprite AbilityIcon { get; protected set; } // �ɷ� ������ (UI��)

    public virtual void InitializeAbility(GameObject prefab)
    {
        AbilityPrefab = prefab;
    }

    public virtual void OnAcquire(Player playerInstance)
    {
        this.player = playerInstance; // �÷��̾� �ν��Ͻ� �Ҵ�
        CurrentLevel++; // ���� ����
        ApplyEffect(); // ȿ�� ���� (���� Ŭ�������� ������)
    }

    public virtual void OnRemove()
    {
        CurrentLevel = 0; // ���� �ʱ�ȭ
        // ���� �� ���� ���� ������ �ʿ��ϴٸ� ���⿡ �߰� (RecalculateStats()�� ó���ϴ� ��� ����)
    }

    // �� �ɷ��� ���� ȿ���� �����ϴ� �޼��� (���� Ŭ�������� �ݵ�� ����)
    public abstract void ApplyEffect();

    // ���������� �� �ɷ� Ŭ�������� Update ������ ���� �� �ֽ��ϴ�.
    public virtual void UpdateAbility() { }

    // Utility: Ÿ�� ã�� (�Ϻ� Ability���� �ʿ��� �� ����)
    // Bow���� ����ϹǷ�, Bow�� �� �޼��带 �������ϰų�,
    // Bow���� FindTarget()�� ���� ȣ���ϴ� ��� Player.GetNearestEnemy()�� ����� �� �ֽ��ϴ�.
    // ���⼭�� Bow�� FindTarget()�� ��ü������ �����ϹǷ� �� �޼���� Ability�� ���� �ɷµ���
    // Ÿ���� �ʿ��� �� ����ϵ��� ���ܵӴϴ�.
    protected GameObject FindTarget()
    {
        if (player == null) return null; // Player ������Ʈ�� ������ Ÿ�� Ž�� �Ұ�

        // �÷��̾��� ��ġ�� �������� ���� ����� ���� ã���ϴ�.
        GameObject target = GameObject
            .FindGameObjectsWithTag("Enemy")
            .Where(enemy => enemy != null && enemy.activeInHierarchy && enemy.transform != null) // null üũ �� Ȱ�� ���� üũ
            .Where(enemy => Vector3.Distance(enemy.transform.position, player.transform.position) < player.AttackRange) // �÷��̾� ���� ���� �� �� ���͸�
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, player.transform.position)) // �Ÿ��� ����
            .FirstOrDefault(); // ���� ����� �� ����
        return target;
    }
}