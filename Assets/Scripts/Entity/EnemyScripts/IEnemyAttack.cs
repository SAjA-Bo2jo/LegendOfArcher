using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyAttack
{
    bool CanAttack(EnemyController controller);     // ������ �� �ִ� �� Ȯ��

    void Attack(EnemyController controller);        // ���� ����
}
