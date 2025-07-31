using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Abillity : MonoBehaviour
{
    protected Player player;                                    // �÷��̾� ����
    protected AnimationHandler animationHandler;                // �ִϸ��̼� �ڵ鷯 ���� (���� �� Ȱ�� ����)
    protected GameObject target;                                  // ���� ���
    protected float lastAttackTime = 0f;                          // ������ ���� �ð� �����

    protected virtual void Awake()
    {
        player = GetComponent<Player>();                        // �÷��̾� ������Ʈ ��������
        animationHandler = GetComponent<AnimationHandler>();    // �ִϸ��̼� �ڵ鷯 ��������
    }

    protected virtual void Update()
    {
        target = FindTarget();                                  // �� �����Ӹ��� ��� ����
        TryAttack();                                            // ���� �õ�
    }


    protected virtual GameObject FindTarget()  // ���� ��� Ž��
    {
        GameObject target = GameObject
            .FindGameObjectsWithTag("Enemy")                                                   // "Enemy" �±׸� ���� ������Ʈ�� ��
            .Where(enemy => enemy.activeInHierarchy)                                           // Ȱ��ȭ�� �͸� ���͸�
            .Where(enemy => Vector3.Distance(enemy.transform.position, transform.position) < player.AttackRange) // �����Ÿ� �̳�
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position))  // ���� ����� ������ ����
            .FirstOrDefault();                                                                 // ���� ����� ��� ����

        return target;
    }


    protected float DistanceToTarget()    // �÷��̾�� Ÿ�� ���� �Ÿ� ���
    {
        return Vector2.Distance(player.transform.position, target.transform.position);
    }
 

    protected virtual float AttackDelay()   // ���� ������ ��� (���ݼӵ� ���), ���ݼӵ��� �������� �����̴� ª����
    {
        float attackSpeed = player.AttackSpeed;                     // �⺻ ���ݼӵ�
        float multiplier = player.AttackSpeedMultiplier / 100f;     // ���ݼӵ� ���� (�ۼ�Ʈ�� 1.x�� ��ȯ)
        float totalAttackSpeed = attackSpeed * multiplier;          // ���� ���ݼӵ�
        float delay = 1f / totalAttackSpeed;                        // ���ݼӵ��� ������(��)�� ��ȯ

        return delay;
    }


    protected virtual void TryAttack()  // ������ �õ��ϴ� �Լ�
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


    protected virtual void PerformAttack()      // ���� ���� ���� ���� (�� �κ��� ����ؼ� �� �ɷ¸��� �ٸ��� ���� ����)
    {
        Debug.Log("���� �����! ���: " + target.name);
        // ���⼭ ���� ����ü �����̳� ������ ��� ���� ����
    }
}
