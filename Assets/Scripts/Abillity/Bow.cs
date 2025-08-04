using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Abillity Ŭ������ lastAttackTime ������ ���� ������ �����մϴ�.
public class Bow : Abillity
{
    [SerializeField] private Transform weaponPivot;  // Ȱ�� ȸ���� �߽��� (�÷��̾��� Ư�� ��ġ)
    [SerializeField] private float radius = 0.3f;    // weaponPivot���� Ȱ�� ������ �ִ� �Ÿ�
    private PlayerController playerController;       // �÷��̾��� �̵� �� �ٶ󺸴� ���� ������ �������� ����
    [SerializeField] private Transform firePoint;    // ȭ���� ������ �����Ǿ� �߻�� ��ġ (Ȱ �� �κ�)

    // ������Ʈ Ǯ���� ���� ��� Ű
    public const string ARROW_POOL_KEY = "Arrow"; // �ٸ� ��ũ��Ʈ���� ������ �� �ֵ��� public���� ����

    [Header("Ȱ ���� ����")]
    private bool isAttacking = false;            // ���� ����(���� ����) ������ ��Ÿ���� �÷���

    // ���� Ȱ�� �Ű���(������) ȭ�� ������Ʈ ����
    private GameObject loadedArrowGO;
    private Arrow loadedArrowScript;

    // Start �޼���� ������Ʈ�� Ȱ��ȭ�� �� �� �� ȣ��˴ϴ�.
    protected void Start()
    {
        // Player Ŭ�������� BaseAbillity�� player �ʵ带 ��ӹ޾� ���
        player = GetComponentInParent<Player>();
        animationHandler = GetComponent<AnimationHandler>();
        playerController = GetComponentInParent<PlayerController>(); // PlayerController ����

        if (firePoint == null)
        {
            Debug.LogError("Bow ��ũ��Ʈ�� Fire Point�� �Ҵ���� �ʾҽ��ϴ�! ȭ�� �߻� ��ġ�� �������ּ���.");
        }
    }

    // LateUpdate�� ��� Update �Լ��� ȣ��� �� �� ������ ȣ��˴ϴ�.
    // ���⼭�� Ȱ�� ��ġ�� ȸ���� ������Ʈ�Ͽ� �ε巯�� �ð��� �������� �����մϴ�.
    private void LateUpdate()
    {
        if (player == null || weaponPivot == null || playerController == null) return;

        // �÷��̾ �ٶ󺸴� �������� Ȱ ȸ�� (PlayerController�� LookDirection ���)
        float angle = Mathf.Atan2(playerController.LookDirection.y, playerController.LookDirection.x);
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
        transform.position = weaponPivot.position + offset;
        float angleDeg = angle * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angleDeg);

        // Ȱ�� ȸ���� �� ������ ȭ�쵵 �Բ� ȸ���ϵ��� ������Ʈ
        if (loadedArrowGO != null)
        {
            loadedArrowGO.transform.position = firePoint.position;
            loadedArrowGO.transform.rotation = transform.rotation;
        }
    }

    protected void Update()
    {
        // PlayerController�� FindTarget()�� ����ϴ� ���� �ϰ����� �����ϴ�.
        // ������ Bow ��ü ������ �����ϰ� �ʹٸ� �״�� �ξ �˴ϴ�.
        // ���⼭�� ���� Bow�� FindTarget()�� ����Ѵٰ� �����մϴ�.
        target = FindTarget();
        TryAttack();
    }

    // �þ� �� ���� ����� ���� ã�� ��ȯ�մϴ�.
    // (PlayerController�� FindTarget�� �ߺ��� �� �����Ƿ�, ���迡 ���� �ϳ��� ��� ����)
    protected GameObject FindTarget()
    {
        if (player == null) return null;

        GameObject target = GameObject
            .FindGameObjectsWithTag("Enemy")
            .Where(enemy => enemy != null && enemy.activeInHierarchy && enemy.transform != null)
            .Where(enemy => Vector3.Distance(enemy.transform.position, transform.position) < player.AttackRange)
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position))
            .FirstOrDefault();
        return target;
    }

    // �÷��̾��� ���� �ӵ��� ������� ���� ���� �ð��� ����մϴ�.
    protected float AttackDelay()
    {
        // player.MaxAttackSpeed�� ���
        float totalAttackSpeed = player.MaxAttackSpeed;

        if (totalAttackSpeed <= 0.01f) totalAttackSpeed = 0.01f;

        float delay = 1f / totalAttackSpeed;
        return delay;
    }

    // ������ �õ��մϴ�.
    protected void TryAttack()
    {
        if (target == null) return;
        if (isAttacking) return;

        // �÷��̾ �����̴� ���� ���� �������� ����
        if (playerController != null && playerController.IsMoving)
        {
            return;
        }

        float delay = AttackDelay();

        if (Time.time >= lastAttackTime + delay)
        {
            StartCoroutine(PerformAttackWithDelay(delay));
            lastAttackTime = Time.time;
        }
    }

    // ȭ�� ���� �ð��� �����Ͽ� ������ �����ϴ� �ڷ�ƾ�Դϴ�.
    private IEnumerator PerformAttackWithDelay(float totalAttackDelay)
    {
        isAttacking = true;

        float timeToWaitForArrowLoad = Mathf.Max(0f, totalAttackDelay - 0.2f); // ���� �ð� (�������� �Ϻ�)
        yield return new WaitForSeconds(timeToWaitForArrowLoad);

        // ������Ʈ Ǯ���� �Ϲ� ȭ���� �����ɴϴ�.
        loadedArrowGO = ObjectPoolManager.Instance.Get(ARROW_POOL_KEY);

        if (loadedArrowGO == null)
        {
            Debug.LogError($"������Ʈ Ǯ���� '{ARROW_POOL_KEY}'�� �������� ���߽��ϴ�. Ǯ�� ��ϵǾ����� Ȯ���ϼ���.");
            isAttacking = false;
            yield break;
        }

        loadedArrowScript = loadedArrowGO.GetComponent<Arrow>();
        if (loadedArrowScript == null)
        {
            Debug.LogError("ȭ�� Prefab�� Arrow ��ũ��Ʈ�� �����ϴ�!");
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
            isAttacking = false;
            yield break;
        }

        // ȭ���� Rigidbody�� �ʱ�ȭ�ϰ� ��Ȱ��ȭ
        Rigidbody2D arrowRb = loadedArrowScript.GetComponent<Rigidbody2D>();
        if (arrowRb != null)
        {
            arrowRb.isKinematic = true;
            arrowRb.simulated = false;
            arrowRb.velocity = Vector2.zero;
            arrowRb.angularVelocity = 0;
        }

        // ȭ���� ��ġ�� ȸ���� Ȱ�� firePoint�� ����ϴ�.
        loadedArrowGO.transform.position = firePoint.position;
        loadedArrowGO.transform.rotation = transform.rotation;
        loadedArrowScript.transform.localScale = Vector3.one * player.AttackSize; // �÷��̾� ���ȿ� ���� ũ��

        if (animationHandler != null)
        {
            animationHandler.Attack(); // �÷��̾� ���� �ִϸ��̼� Ʈ����
        }

        // ������ ���� ������ �ð� ���
        yield return new WaitForSeconds(totalAttackDelay - timeToWaitForArrowLoad);

        FireLoadedArrow(); // ���� ȭ�� �߻� ���� ȣ��

        // ȭ�� ���� ���� �ʱ�ȭ (FireLoadedArrow ���ο��� �̹� ó���� �� ����)
        loadedArrowGO = null;
        loadedArrowScript = null;

        isAttacking = false;
    }

    // ������ ȭ���� ���� �߻��ϴ� �����Դϴ�.
    private void FireLoadedArrow()
    {
        if (target == null || loadedArrowScript == null || loadedArrowGO == null)
        {
            if (loadedArrowGO != null)
            {
                ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            }
            loadedArrowGO = null;
            loadedArrowScript = null;
            return;
        }

        // �÷��̾�� Ư�� ȭ�� �ɷ��� �ߵ��� �� �ִ��� ����ϴ�.
        // �÷��̾ FireArrowAbility�� ���� �ɷ��� Ȱ��ȭ�Ǿ� �ִٸ�, �� �޼��尡 true�� ��ȯ�ϰ� Ư�� ȭ���� �߻��� ���Դϴ�.
        bool specialArrowFired = player.TryActivateSpecialArrowAbility(loadedArrowGO, loadedArrowScript);

        // Ư�� ȭ���� �߻���� �ʾҴٸ�, �Ϲ� ȭ���� �߻��մϴ�.
        if (!specialArrowFired)
        {
            Rigidbody2D arrowRb = loadedArrowScript.GetComponent<Rigidbody2D>();
            if (arrowRb != null)
            {
                arrowRb.isKinematic = false;
                arrowRb.simulated = true;
            }

            loadedArrowScript.Setup(
                damage: player.AttackDamage,
                size: player.AttackSize,
                critRate: player.CriticalRate,
                speed: player.ProjectileSpeed
            );

            Vector3 finalLaunchDirection = transform.right; // Ȱ�� ���� �������� �߻�
            loadedArrowScript.LaunchTowards(finalLaunchDirection);
        }

        // loadedArrowGO�� loadedArrowScript�� ���� ������ ���� �ʱ�ȭ�˴ϴ�.
        // Ư�� ȭ���� �߻�Ǿ��� ���, loadedArrowGO�� �̹� FireArrowAbility ���ο��� Ǯ�� ��ȯ�Ǿ��� ���Դϴ�.
        loadedArrowGO = null;
        loadedArrowScript = null;
    }
}