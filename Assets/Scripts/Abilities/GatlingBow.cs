using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GatlingBow : Ability // Ability Ŭ������ ��ӹ޽��ϴ�.
{
    [Header("��Ʋ�� ���� �ɷ� ����")]
    [SerializeField] private Sprite gatlingBowAbilityIcon; // �ɷ� ���� UI�� ������
    [SerializeField] private int arrowsPerShot = 3; // �� ���� �߻�� ȭ�� ��
    [SerializeField] private float spreadAngle = 10f; // ȭ���� ������ ����
    [SerializeField] private float fireIntervalBetweenArrows = 0.05f; // �ٿ��� �߻� �� ȭ�� �� �ð� ����

    [Header("���� ����")]
    [SerializeField] private float attackSpeedBonus = 0.5f; // �÷��̾��� AttackSpeed�� ������ ���ʽ�

    // ��Ʋ�� ȭ�� ������Ʈ Ǯ Ű
    private const string GATLING_ARROW_POOL_KEY = "GatlingArrow";

    // �ɷ��� ȹ���ϰų� ������ �� �� ȣ��
    public override void OnAcquire(Player playerInstance)
    {
        base.OnAcquire(playerInstance); // �θ� Ŭ���� OnAcquire ȣ�� (���� ����)
        // this.player�� Ability Ŭ�������� �̹� �Ҵ��
        UpdateDescription(); // �ɷ� ���� ������Ʈ
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} ȹ��/��ȭ: {Description}");
    }

    // �ɷ��� ���ŵ� �� ȣ��
    public override void OnRemove()
    {
        base.OnRemove();
        Debug.Log($"[{AbilityName}] ȿ�� ���ŵ�.");
    }

    // �� �ɷ��� Ȱ��ȭ�� �� �÷��̾� ���ȿ� ������ �ݴϴ�.
    public override void ApplyEffect()
    {
        if (player != null)
        {
            player.AttackSpeed += attackSpeedBonus; // �÷��̾��� �⺻ ���� �ӵ��� ���ʽ� �߰�
            // Player.RecalculateStats()���� �� ������ ���� MaxAttackSpeed�� �ݿ��˴ϴ�.
        }
        UpdateDescription();
    }

    // �ɷ� ������ ���� ������ ���� ������Ʈ
    private void UpdateDescription()
    {
        if (CurrentLevel > 0)
        {
            description = $"�÷��̾��� ���� �ӵ��� {attackSpeedBonus:F1} ������Ű��, �� ���� �� {arrowsPerShot}���� Ư�� ȭ���� �߻��մϴ�.";
        }
        else
        {
            description = "��Ʋ�� ���� �ɷ� (ȹ�� ���)";
        }
    }

    /// <summary>
    /// �Ϲ� ȭ�� ��� ��Ʋ�� ȭ�� �߻縦 �õ��մϴ�. Player ��ũ��Ʈ���� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="regularArrowGO">�߻��Ϸ��� �Ϲ� ȭ�� GameObject (Ǯ�� ��ȯ�˴ϴ�)</param>
    /// <param name="regularArrowScript">�߻��Ϸ��� �Ϲ� ȭ�� Arrow ������Ʈ</param>
    /// <param name="playerInstance">���� �÷��̾� �ν��Ͻ� (���� ������)</param>
    /// <returns>��Ʋ�� ȭ���� �߻�Ǿ����� true, �ƴϸ� false</returns>
    public bool TryActivateGatlingArrow(GameObject regularArrowGO, Arrow regularArrowScript, Player playerInstance)
    {
        // GatlingBow �ɷ��� �ɵ������� �ߵ��Ǵ� ���̹Ƿ�, CurrentLevel�� Ȯ���ϸ� �˴ϴ�.
        if (CurrentLevel > 0) // �ɷ��� Ȱ��ȭ(ȹ��)�Ǿ� �ִٸ�
        {
            Debug.Log($"GatlingBow: ��Ʋ�� ���� �ߵ�!");

            // ���� �Ϲ� ȭ���� Ǯ�� ��ȯ
            ObjectPoolManager.Instance.Return("Arrow", regularArrowGO);

            // �ٿ��� ȭ�� �߻� �ڷ�ƾ ����
            // ���� ȭ���� ��ġ�� ȸ���� ������� �մϴ�.
            StartCoroutine(FireMultipleGatlingArrows(regularArrowGO.transform.position, regularArrowGO.transform.rotation, regularArrowScript.transform.localScale, playerInstance));

            return true; // ��Ʋ�� ȭ�� �߻� ����
        }
        return false; // ��Ʋ�� ���� �ɷ� ���� ������
    }

    // ���� ���� ��Ʋ�� ȭ���� �߻��ϴ� �ڷ�ƾ
    private IEnumerator FireMultipleGatlingArrows(Vector3 startPosition, Quaternion startRotation, Vector3 startScale, Player playerInstance)
    {
        // Player�� LookDirection (���� Ÿ�� ����)�� ����Ͽ� ȭ�� �߻� ������ ����
        // PlayerController�� Player�� ����̹Ƿ�, playerInstance.GetComponent<PlayerController>()�� ������ �� �ֽ��ϴ�.
        PlayerController playerController = playerInstance.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("GatlingBow: PlayerController�� ã�� �� �����ϴ�.");
            yield break;
        }

        Vector3 baseDirection = playerController.LookDirection; // �÷��̾ ���� �ٶ󺸴� ���� (Bow�� Ÿ���� ���ϰ� ȸ����Ű�� ����)
        float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;

        for (int i = 0; i < arrowsPerShot; i++)
        {
            // �� ȭ���� �߻� ���⿡ ������ ���� ����
            float minSpread = -spreadAngle / 2f;
            float maxSpread = spreadAngle / 2f;
            float randomSpread = Random.Range(minSpread, maxSpread);

            Quaternion rotation = Quaternion.Euler(0, 0, baseAngle + randomSpread);
            Vector3 launchDirection = rotation * Vector3.right; // Z�� ȸ�� �� X�� ����

            // ��Ʋ�� ȭ���� Ǯ���� �����ɴϴ�.
            GameObject gatlingArrowGO = ObjectPoolManager.Instance.Get(GATLING_ARROW_POOL_KEY);
            if (gatlingArrowGO == null)
            {
                Debug.LogError($"GatlingBow: ������Ʈ Ǯ���� '{GATLING_ARROW_POOL_KEY}'�� �������� ���߽��ϴ�.");
                break; // �� �̻� ȭ���� �߻��� �� ����
            }

            // ��Ʋ�� ȭ���� Arrow ������Ʈ�� �����ɴϴ�.
            Arrow gatlingArrowScript = gatlingArrowGO.GetComponent<Arrow>();
            if (gatlingArrowScript == null)
            {
                Debug.LogError("GatlingBow: ��Ʋ�� ȭ�� Prefab�� Arrow ��ũ��Ʈ�� �����ϴ�!");
                ObjectPoolManager.Instance.Return(GATLING_ARROW_POOL_KEY, gatlingArrowGO);
                continue;
            }

            // ���� ���� Ȱ��ȭ
            Rigidbody2D gatlingArrowRb = gatlingArrowGO.GetComponent<Rigidbody2D>();
            if (gatlingArrowRb != null)
            {
                gatlingArrowRb.isKinematic = false;
                gatlingArrowRb.simulated = true;
            }

            // ȭ���� ��ġ, ȸ��, �������� ����
            // ���� ��ġ�� Bow�� FirePoint�� �����ϰ� ����ϴ� ���� �����ϴ�.
            // ���⼭�� regularArrowGO.transform.position (��, Bow�� FirePoint ��ġ)�� �״�� ����մϴ�.
            gatlingArrowGO.transform.position = startPosition;
            gatlingArrowGO.transform.rotation = rotation;
            gatlingArrowScript.transform.localScale = startScale; // �Ϲ� ȭ���� ũ�� ����
            gatlingArrowGO.SetActive(true);

            // ��Ʋ�� ȭ���� ������ �����մϴ�. (�÷��̾� ���� ���)
            gatlingArrowScript.Setup(
                damage: playerInstance.AttackDamage,
                size: playerInstance.AttackSize,
                critRate: playerInstance.CriticalRate,
                speed: playerInstance.ProjectileSpeed
            );
            gatlingArrowScript.LaunchTowards(launchDirection); // ȭ�� �߻�

            yield return new WaitForSeconds(fireIntervalBetweenArrows); // ���� ȭ�� �߻���� ���
        }
    }
}