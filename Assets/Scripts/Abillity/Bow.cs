using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bow : Abillity
{
    [SerializeField] private Animator bowAnimator;
    [SerializeField] private Transform weaponPivot;  // ȸ�� �߽�
    [SerializeField] private float radius = 40f;     // �߽ɿ��� ������ �Ÿ�
    private PlayerController playerController; // �� ������ ���� �Ҵ�Ǿ�� �մϴ�!
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint; // Bow ��ũ��Ʈ�� �߰�


    protected void Start()
    {
        // �θ� ������Ʈ���� Player ������Ʈ ��������
        player = GetComponentInParent<Player>();

        // �ڽſ��� ���� AnimationHandler ��������
        animationHandler = GetComponent<AnimationHandler>();

        // �� ���� �߰��Ͽ� PlayerController ������ �����ɴϴ�.
        // PlayerController�� Player�� ���� GameObject �Ǵ� �θ� �ִٰ� �����մϴ�.
        playerController = GetComponentInParent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("Bow ��ũ��Ʈ�� �θ𿡼� PlayerController�� ã�� �� �����ϴ�!", this);
        }
    }

    private void LateUpdate()
    {
        // playerController�� null�� �ƴϾ�� �մϴ�.
        if (player == null || weaponPivot == null || playerController == null) return;

        // �ٶ󺸴� ������ 0�̸� ȸ�� ����
        if (playerController.LookDirection == Vector2.zero) return;

        // ���� ���͸� ������ ��ȯ
        float angle = Mathf.Atan2(playerController.LookDirection.y, playerController.LookDirection.x);

        // Ȱ�� ��ġ ��� (WeaponPivot ����)
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
        transform.position = weaponPivot.position + offset;

        // Ȱ�� ȸ���� �ٶ󺸴� �������� ����
        float angleDeg = angle * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angleDeg);
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

        if (target != null)
        {
            Debug.Log($"Target found: {target.name} at distance: {Vector3.Distance(target.transform.position, transform.position)}");
        }
        else
        {
            Debug.Log("No target found in range.");
        }
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
            Debug.Log("Attack condition met: Performing attack!");
            PerformAttack();
            lastAttackTime = Time.time;
        }
        else
        {
            Debug.Log("Attack delay not finished.");
        }
    }
    // Bow.cs

    protected void PerformAttack()
    {
        if (target == null) return;

        // �ִϸ��̼� ����
        if (animationHandler != null)
        {
            animationHandler.Attack();
        }

        // 1. ȭ���� �߻� ������ Ȱ(Bow)�� ���� �ٶ󺸴� �������� �����մϴ�.
        //    ������ LateUpdate���� playerController.LookDirection�� ���� Ȱ�� ȸ���������Ƿ�
        //    Ȱ�� ���� '������' ���� (transform.right)�� �� �÷��̾ �ٶ󺸴� �����Դϴ�.
        Vector3 finalLaunchDirection = transform.right;
        // ���� Ȱ ��������Ʈ�� '��'�� ���ϵ��� �׷��� �ִٸ� transform.up�� ����ϼ���.

        // 2. �� �߻� ������ �������� ȭ���� �ʱ� ȸ���� ����մϴ�.
        //    Ȱ�� ����� �����ϰ� ȭ���� ������ ������ �ٶ󺸵��� �մϴ�.
        float angle = Mathf.Atan2(finalLaunchDirection.y, finalLaunchDirection.x) * Mathf.Rad2Deg;
        Quaternion arrowInitialRotation = Quaternion.Euler(0, 0, angle);

        // 3. ȭ���� FirePoint�� ��ġ�� ���� ȸ������ �����մϴ�.
        //    firePoint�� �÷��̾��� �ڽ��̹Ƿ� �÷��̾ �������� �ùٸ� ��ġ�� ����ŵ�ϴ�.
        var arrowObj = Instantiate(arrowPrefab, firePoint.position, arrowInitialRotation);
        var arrow = arrowObj.GetComponent<Arrow>();

        // Player ������ �Ѱܼ� ����
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