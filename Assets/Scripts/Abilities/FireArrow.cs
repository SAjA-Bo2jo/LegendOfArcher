using UnityEngine;

public class FireArrow : Ability
{
    [SerializeField] private int[] attackCountForFireArrowPerLevel = { 3, 2, 2, 1, 1 };
    [SerializeField] private float[] damageMultiplierPerLevel = { 1.5f, 1.7f, 2.0f, 2.2f, 2.5f };

    private int currentAttackCount = 0;
    private const string FIRE_ARROW_POOL_KEY = "FireArrow";

    public override void OnAcquire(Player playerInstance)
    {
        base.OnAcquire(playerInstance);
        currentAttackCount = 0;
        UpdateDescription();
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} ȹ��/��ȭ: {Description}");
    }

    public override void OnRemove()
    {
        base.OnRemove();
        Debug.Log($"[{AbilityName}] ȿ�� ���ŵ�.");
    }

    public override void ApplyEffect()
    {
        UpdateDescription();
    }

    private void UpdateDescription()
    {
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel)
        {
            description = $"�� {attackCountForFireArrowPerLevel[CurrentLevel - 1]}��° ���� �� ��ȭ�� �߻� (������ {damageMultiplierPerLevel[CurrentLevel - 1]}��)";
        }
        else
        {
            description = "��ȭ�� �ɷ� (ȹ�� ���)";
        }
    }

    /// <summary>
    /// �Ϲ� ȭ�� ��� ��ȭ�� �߻縦 �õ��մϴ�. Player ��ũ��Ʈ���� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="regularArrowGO">�߻��Ϸ��� �Ϲ� ȭ�� GameObject</param>
    /// <param name="regularArrowScript">�߻��Ϸ��� �Ϲ� ȭ�� Arrow ������Ʈ</param>
    /// <returns>��ȭ���� �߻�Ǿ����� true, �ƴϸ� false</returns>
    public bool TryActivateFireArrow(GameObject regularArrowGO, Arrow regularArrowScript)
    {
        // ���� Ƚ���� ������ŵ�ϴ�.
        currentAttackCount++;

        // --- ����� �α� ---
        Debug.Log($"[FireArrow] TryActivateFireArrow ȣ���. ���� ���� Ƚ��: {currentAttackCount}");
        if (CurrentLevel > 0)
        {
            Debug.Log($"[FireArrow] ���� Ȯ��: ���� ����: {CurrentLevel}, �ʿ� ���� Ƚ��: {attackCountForFireArrowPerLevel[CurrentLevel - 1]}");
        }
        // --- ����� �α� �� ---

        // ������ �����Ǿ��� ��쿡�� ��ȭ���� �߻��մϴ�.
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel &&
            currentAttackCount >= attackCountForFireArrowPerLevel[CurrentLevel - 1])
        {
            // ��ȭ�� �߻� ������ �����Ǿ����Ƿ�, ���� ���� Ƚ���� �����մϴ�.
            currentAttackCount = 0;
            Debug.Log($"FireArrow: ��ȭ�� �ߵ�! (Lv.{CurrentLevel})");

            // ���� �Ϲ� ȭ���� Ǯ�� ��ȯ�մϴ�.
            ObjectPoolManager.Instance.Return("Arrow", regularArrowGO);

            // ��ȭ���� ������Ʈ Ǯ���� �����ɴϴ�.
            GameObject fireArrowGO = ObjectPoolManager.Instance.Get(FIRE_ARROW_POOL_KEY);
            if (fireArrowGO == null)
            {
                Debug.LogError($"FireArrow: ��ȭ�� ������Ʈ Ǯ���� '{FIRE_ARROW_POOL_KEY}'�� �������� ���߽��ϴ�.");
                return false;
            }

            Arrow fireArrowScript = fireArrowGO.GetComponent<Arrow>();
            if (fireArrowScript == null)
            {
                Debug.LogError("FireArrow: ��ȭ�� Prefab�� Arrow ��ũ��Ʈ�� �����ϴ�!");
                ObjectPoolManager.Instance.Return(FIRE_ARROW_POOL_KEY, fireArrowGO);
                return false;
            }

            fireArrowGO.transform.position = regularArrowGO.transform.position;
            fireArrowGO.transform.rotation = regularArrowGO.transform.rotation;
            fireArrowScript.transform.localScale = regularArrowScript.transform.localScale;

            Rigidbody2D fireArrowRb = fireArrowGO.GetComponent<Rigidbody2D>();
            if (fireArrowRb != null)
            {
                fireArrowRb.isKinematic = false;
                fireArrowRb.simulated = true;
            }

            if (player == null)
            {
                Debug.LogError("FireArrow: Player ������ null�Դϴ�. ������ ������ �� �����ϴ�.");
                ObjectPoolManager.Instance.Return(FIRE_ARROW_POOL_KEY, fireArrowGO);
                return false;
            }

            fireArrowScript.Setup(
                damage: player.AttackDamage * damageMultiplierPerLevel[CurrentLevel - 1],
                size: player.AttackSize,
                critRate: player.CriticalRate,
                speed: player.ProjectileSpeed
            );
            fireArrowScript.LaunchTowards(fireArrowGO.transform.right);

            return true; // ��ȭ�� �߻翡 ���������Ƿ� true ��ȯ
        }

        // ������ �������� �ʾҴٸ� false�� ��ȯ�մϴ�.
        return false;
    }
}