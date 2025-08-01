using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ������Ʈ Ǯ �Ŵ����� ����ϱ� ���� �߰�
// using ObjectPoolManager;

public class Bow : Abillity
{
    [SerializeField] private Animator bowAnimator;
    [SerializeField] private Transform weaponPivot;  // ȸ�� �߽�
    [SerializeField] private float radius = 40f;     // �߽ɿ��� ������ �Ÿ�
    private PlayerController playerController;
    [SerializeField] private Transform firePoint; // Bow ��ũ��Ʈ�� �߰�

    // ������Ʈ Ǯ���� ���� ��� Ű
    private const string ARROW_POOL_KEY = "Arrow";


    protected void Start()
    {
        // �θ� ������Ʈ���� Player ������Ʈ ��������
        player = GetComponentInParent<Player>();

        // �ڽſ��� ���� AnimationHandler ��������
        animationHandler = GetComponent<AnimationHandler>();

        // �� ���� �߰��Ͽ� PlayerController ������ �����ɴϴ�.
        // PlayerController�� Player�� ���� GameObject �Ǵ� �θ� �ִٰ� �����մϴ�.
        playerController = GetComponentInParent<PlayerController>();
    }

    private void LateUpdate()
    {
        // playerController�� null�� �ƴϾ�� �մϴ�.
        if (player == null || weaponPivot == null || playerController == null) return;

        // Ÿ���� ������ ���� Ȱ�� ��ġ�� ȸ���� ������Ʈ�մϴ�.
        if (target != null)
        {
            // ���� ���͸� ������ ��ȯ
            float angle = Mathf.Atan2(playerController.LookDirection.y, playerController.LookDirection.x);

            // Ȱ�� ��ġ ��� (WeaponPivot ����)
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            transform.position = weaponPivot.position + offset;

            // Ȱ�� ȸ���� �ٶ󺸴� �������� ����
            float angleDeg = angle * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angleDeg);
        }
        // Ÿ���� ������ Ȱ�� weaponPivot ��ġ�� �����մϴ�.
        else
        {
            transform.position = weaponPivot.position;
            transform.rotation = Quaternion.identity;
        }
    }

    protected void Update()
    {
        target = FindTarget();  // �� �����Ӹ��� ��� ����
        TryAttack();            // ���� �õ�
    }

    protected GameObject FindTarget()
    {
        // �÷��̾� ������ null ��ȯ
        if (player == null) return null;

        // Enemy �±׸� ���� ������Ʈ�� �� ���ǿ� �´� ���� ����� ��� ã��
        GameObject target = GameObject
            .FindGameObjectsWithTag("Enemy")
            .Where(enemy => enemy != null && enemy.activeInHierarchy && enemy.transform != null)
            .Where(enemy => Vector3.Distance(enemy.transform.position, transform.position) < player.AttackRange)
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position))
            .FirstOrDefault();
        return target;
    }

    protected float AttackDelay()
    {
        // �⺻ ���� �ӵ�
        float attackSpeed = player.AttackSpeed;

        // ���� �ӵ� ������ 100 ���� ������ ��ȯ
        float multiplier = player.AttackSpeedMultiplier / 100f;

        // �� ���� �ӵ� ���
        float totalAttackSpeed = attackSpeed * multiplier;

        // ������ ���
        float delay = 1f / totalAttackSpeed;

        return delay;
    }

    protected void TryAttack()
    {
        if (target == null) return; // Ÿ�� ������ ���� �� ��

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();  // �÷��̾��� Rigidbody2D�� ���� �̵� ������ Ȯ��
        if (rb != null && rb.velocity.magnitude > 0.01f)
            return; // �̵� ���̸� ���� �� ��

        float delay = AttackDelay();
        Debug.Log($"Calculated attack delay: {delay}");
        Debug.Log($"Current Time: {Time.time}, Last Attack Time: {lastAttackTime}");

        if (Time.time >= lastAttackTime + delay)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    protected void PerformAttack()
    {
        if (target == null) return;

        // �ִϸ��̼� ����
        if (animationHandler != null)
        {
            animationHandler.Attack();
        }

        // 1. ������Ʈ Ǯ���� ȭ���� �����ɴϴ�.
        var arrowObj = ObjectPoolManager.Instance.Get(ARROW_POOL_KEY);

        if (arrowObj == null)
        {
            Debug.LogError("Failed to get arrow from object pool.");
            return;
        }

        var arrow = arrowObj.GetComponent<Arrow>();
        if (arrow == null)
        {
            Debug.LogError("Arrow component not found on pooled object.");
            return;
        }

        // 2. ȭ���� ��ġ�� ȸ���� �����մϴ�.
        // ȭ���� �߻� ������ Ȱ(Bow)�� ���� �ٶ󺸴� �������� ����
        Vector3 finalLaunchDirection = transform.right;
        // ȭ���� �ʱ� ȸ���� ���
        float angle = Mathf.Atan2(finalLaunchDirection.y, finalLaunchDirection.x) * Mathf.Rad2Deg;
        Quaternion arrowInitialRotation = Quaternion.Euler(0, 0, angle);

        arrowObj.transform.position = firePoint.position;
        arrowObj.transform.rotation = arrowInitialRotation;

        // 3. �÷��̾� ������ �Ѱܼ� ����
        arrow.Setup(
            damage: player.AttackDamage,
            size: player.AttackSize,
            critRate: player.CriticalRate,
            speed: player.AttackSpeed * (player.AttackSpeedMultiplier / 100f)
        );

        // 4. ������ ȭ�쿡 ���� �߻� ������ �����Ͽ� �������� ���� ���մϴ�.
        arrow.LaunchTowards(finalLaunchDirection); // ���⿡�� '����' ���͸� �Ѱܾ� �մϴ�.
    }
}
