using UnityEngine;

public class FireArrow : Abillity
{
    // ������ �䱸 ���� Ƚ�� (��: 3, 2, 2, 1, 1)
    [SerializeField] private int[] attackCountForFireArrowPerLevel = { 3, 2, 2, 1, 1 };
    // ������ ��ȭ�� ������ ���� (��: 1.5, 1.7, 2.0, 2.2, 2.5)
    [SerializeField] private float[] damageMultiplierPerLevel = { 1.5f, 1.7f, 2.0f, 2.2f, 2.5f };

    private int currentAttackCount = 0; // ���� ���� Ƚ�� ī����

    // ��ȭ�� ������Ʈ Ǯ Ű (ObjectPoolManager���� ����� �̸�)
    private const string FIRE_ARROW_POOL_KEY = "FireArrow";

    void Awake()
    {
        AbilityName = "��ȭ��";
        MaxLevel = attackCountForFireArrowPerLevel.Length;
        InitializeAbility(this.gameObject);
    }

    // �ɷ��� ȹ���ϰų� ������ �� �� ȣ��
    public override void OnAcquire(Player playerInstance)
    {
        base.OnAcquire(playerInstance); // �θ� Ŭ���� OnAcquire ȣ�� (���� ����)
        currentAttackCount = 0; // �ɷ� ȹ��/������ �� ī��Ʈ �ʱ�ȭ
        UpdateDescription(); // �ɷ� ���� ������Ʈ
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} ȹ��/��ȭ: {Description}");
    }

    // �ɷ��� ���ŵ� �� ȣ��
    public override void OnRemove()
    {
        base.OnRemove();
        Debug.Log($"[{AbilityName}] ȿ�� ���ŵ�.");
    }

    // �� �ɷ��� ���� ������ �ƴ� ���Ǻ� �ߵ��̹Ƿ� ApplyEffect������ Ư���� ���� ���� ����
    public override void ApplyEffect()
    {
        // �ַ� UpdateDescription()�� ȣ���Ͽ� UI�� ǥ�õ� ������ ������Ʈ
        UpdateDescription();
    }

    // ���� �� Ư���� ���� ���� ����
    public override void RemoveEffect()
    {
        // Ư���� �ؾ� �� �� ����
    }

    // �ɷ� ������ ���� ������ ���� ������Ʈ
    private void UpdateDescription()
    {
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel)
        {
            Description = $"�� {attackCountForFireArrowPerLevel[CurrentLevel - 1]}��° ���� �� ��ȭ�� �߻� (������ {damageMultiplierPerLevel[CurrentLevel - 1]}��)";
        }
        else
        {
            Description = "��ȭ�� �ɷ� (ȹ�� ���)";
        }
    }

    /// <summary>
    /// �Ϲ� ȭ�� ��� ��ȭ�� �߻縦 �õ��մϴ�. Bow ��ũ��Ʈ���� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="regularArrowGO">�߻��Ϸ��� �Ϲ� ȭ�� GameObject</param>
    /// <param name="regularArrowScript">�߻��Ϸ��� �Ϲ� ȭ�� Arrow ������Ʈ</param>
    /// <returns>��ȭ���� �߻�Ǿ����� true, �ƴϸ� false</returns>
    public bool TryActivateFireArrow(GameObject regularArrowGO, Arrow regularArrowScript)
    {
        currentAttackCount++;
        // ���� ������ ��ȿ�ϰ�, ���� ī��Ʈ�� �䱸 Ƚ���� �������� ��
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel &&
            currentAttackCount >= attackCountForFireArrowPerLevel[CurrentLevel - 1])
        {
            currentAttackCount = 0; // ī��Ʈ �ʱ�ȭ

            Debug.Log($"��ȭ�� �ߵ�! (Lv.{CurrentLevel})");

            // ���� �Ϲ� ȭ���� Ǯ�� ��ȯ
            ObjectPoolManager.Instance.Return("Arrow", regularArrowGO); // "Arrow"�� �Ϲ� ȭ���� Ǯ Ű��� ����

            // ��ȭ���� Ǯ���� �����ɴϴ�.
            GameObject fireArrowGO = ObjectPoolManager.Instance.Get(FIRE_ARROW_POOL_KEY);
            if (fireArrowGO == null)
            {
                Debug.LogError($"��ȭ�� ������Ʈ Ǯ���� '{FIRE_ARROW_POOL_KEY}'�� �������� ���߽��ϴ�.");
                return false;
            }

            // ��ȭ���� Arrow ������Ʈ�� �����ɴϴ�.
            Arrow fireArrowScript = fireArrowGO.GetComponent<Arrow>();
            if (fireArrowScript == null)
            {
                Debug.LogError("��ȭ�� Prefab�� Arrow ��ũ��Ʈ�� �����ϴ�!");
                ObjectPoolManager.Instance.Return(FIRE_ARROW_POOL_KEY, fireArrowGO);
                return false;
            }

            // ��ȭ���� ��ġ, ȸ��, �������� �Ϲ� ȭ��� �����ϰ� ����
            fireArrowGO.transform.position = regularArrowGO.transform.position;
            fireArrowGO.transform.rotation = regularArrowGO.transform.rotation;
            fireArrowScript.transform.localScale = regularArrowScript.transform.localScale; // ũ�� ����

            // ��ȭ���� ������ �����մϴ�. (�÷��̾� ���� ��� + ������ ����)
            fireArrowScript.Setup(
                damage: player.AttackDamage * damageMultiplierPerLevel[CurrentLevel - 1],
                size: player.AttackSize,
                critRate: player.CriticalRate,
                speed: player.ProjectileSpeed
            );

            // Rigidbody Ȱ��ȭ �� �߻�
            Rigidbody2D fireArrowRb = fireArrowGO.GetComponent<Rigidbody2D>();
            if (fireArrowRb != null)
            {
                fireArrowRb.isKinematic = false;
                fireArrowRb.simulated = true;
            }
            fireArrowScript.LaunchTowards(fireArrowGO.transform.right); // Ȱ�� �������� �߻�

            return true; // ��ȭ�� �߻� ����
        }
        return false; // ��ȭ�� �߻� ���� ������
    }
}