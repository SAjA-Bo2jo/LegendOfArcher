using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAttack : IEnemyAttack
{
    private float lastAttackTime = 0f;                                  // ��Ÿ�� ����

    public bool CanAttack(EnemyController controller)
    {
        return Time.time > lastAttackTime + 1f;                         // ��Ÿ�Ӻ��� �ð��� �� ������ ���� ����
    }

    public void Attack(EnemyController controller)
    {
        lastAttackTime = Time.time;                                     // ���� �ð� ���

        // �߻�ü ����

        // �÷��̾� �������� �߻�
    }
}
