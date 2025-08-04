using UnityEngine;

public class FireArrow : Ability
{
    [SerializeField] private Sprite fireArrowIcon;
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
        // ��ȭ���� ���� ���� ���溸�ٴ� ���Ǻ� �ߵ� �ɷ��̹Ƿ� �� �κ��� ����Ӵϴ�.
        UpdateDescription(); // ���� ������Ʈ
    }

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
    /// �Ϲ� ȭ�� ��� ��ȭ�� �߻縦 �õ��մϴ�. Player ��ũ��Ʈ���� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="regularArrowGO">�߻��Ϸ��� �Ϲ� ȭ�� GameObject</param>
    /// <param name="regularArrowScript">�߻��Ϸ��� �Ϲ� ȭ�� Arrow ������Ʈ</param>
    /// <returns>��ȭ���� �߻�Ǿ����� true, �ƴϸ� false</returns>
    public bool TryActivateFireArrow(GameObject regularArrowGO, Arrow regularArrowScript)
    {
        currentAttackCount++;
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel &&
            currentAttackCount >= attackCountForFireArrowPerLevel[CurrentLevel - 1])
        {
            currentAttackCount = 0;

            Debug.Log($"FireArrow: ��ȭ�� �ߵ�! (Lv.{CurrentLevel})");

            ObjectPoolManager.Instance.Return("Arrow", regularArrowGO); // ���� �Ϲ� ȭ�� Ǯ ��ȯ

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

            // FireArrow �ɷ��� �÷��̾��� ��ġ�� ������ ����Ͽ� ȭ���� �߻��ؾ� �մϴ�.
            // Bow�� FirePoint�� ���� ������ �� �����Ƿ�, �÷��̾� ��ġ�� �÷��̾ �ٶ󺸴� ������ ����ϰų�
            // �ƴϸ� Bow���� �Ѱܹ��� regularArrowGO�� ��ġ�� ȸ���� Ȱ���ؾ� �մϴ�.
            // ���⼭�� regularArrowGO�� ��ġ�� rotation�� Ȱ���մϴ�.
            fireArrowGO.transform.position = regularArrowGO.transform.position;
            fireArrowGO.transform.rotation = regularArrowGO.transform.rotation;
            fireArrowScript.transform.localScale = regularArrowScript.transform.localScale;

            Rigidbody2D fireArrowRb = fireArrowGO.GetComponent<Rigidbody2D>();
            if (fireArrowRb != null)
            {
                fireArrowRb.isKinematic = false;
                fireArrowRb.simulated = true;
            }

            // �÷��̾� ���� (this.player�� OnAcquire���� �Ҵ��)
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
            fireArrowScript.LaunchTowards(fireArrowGO.transform.right); // ȭ�� Prefab�� ���� X�� �������� �߻�

            return true;
        }
        return false;
    }
}