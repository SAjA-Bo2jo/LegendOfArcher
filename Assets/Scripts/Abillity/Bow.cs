using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bow : Abillity
{
    protected override void Awake()
    {
        player = GetComponent<Player>();                        // �÷��̾� ������Ʈ ��������
        animationHandler = GetComponent<AnimationHandler>();    // �ִϸ��̼� �ڵ鷯 ��������
    }

    protected override void Update()
    {
        target = FindTarget();                                  // �� �����Ӹ��� ��� ����
        TryAttack();                                            // ���� �õ�
    }


    protected override GameObject FindTarget()  // ���� ��� Ž��
    {
        GameObject target = GameObject
            .FindGameObjectsWithTag("Enemy")                                                   // "Enemy" �±׸� ���� ������Ʈ�� ��
            .Where(enemy => enemy.activeInHierarchy)                                           // Ȱ��ȭ�� �͸� ���͸�
            .Where(enemy => Vector3.Distance(enemy.transform.position, transform.position) < player.AttackRange) // �����Ÿ� �̳�
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position))  // ���� ����� ������ ����
            .FirstOrDefault();                                                                 // ���� ����� ��� ����

        return target;
    }

    protected override float AttackDelay()   // ���� ������ ��� (���ݼӵ� ���), ���ݼӵ��� �������� �����̴� ª����
    {
        float attackSpeed = player.AttackSpeed;                     // �⺻ ���ݼӵ�
        float multiplier = player.AttackSpeedMultiplier / 100f;     // ���ݼӵ� ���� (�ۼ�Ʈ�� 1.x�� ��ȯ)
        float totalAttackSpeed = attackSpeed * multiplier;          // ���� ���ݼӵ�
        float delay = 1f / totalAttackSpeed;                        // ���ݼӵ��� ������(��)�� ��ȯ

        return delay;
    }


    protected override void TryAttack()  // ������ �õ��ϴ� �Լ�
    {
        if (target == null) return;                           // ��� ������ ����

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();  // Rigidbody2D�� ���� �÷��̾��� ���� ���� Ȯ��
        if (rb != null && rb.velocity.magnitude > 0.01f)
            return;                                           // �÷��̾ �̵� ���̸� �������� ����

        float delay = AttackDelay();                          // ���� ���� ������ ���

        if (Time.time >= lastAttackTime + delay)              // ���� �ð�(Time.time)�� ������ ���� �ð� + �����̸� �ʰ��ߴ��� Ȯ��
        {
            PerformAttack();                                  // ���� ����
            lastAttackTime = Time.time;                       // ������ ���� �ð� ����
        }
    }

    protected override void PerformAttack()      // ���� ���� ���� ���� (�� �κ��� ����ؼ� �� �ɷ¸��� �ٸ��� ���� ����)
    {
        Debug.Log("���� �����! ���: " + target.name);
        // ���⼭ ���� ����ü �����̳� ������ ��� ���� ����

    }
}
