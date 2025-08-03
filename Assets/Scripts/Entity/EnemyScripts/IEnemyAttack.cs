using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyAttack
{
    bool CanAttack(EnemyController controller);     // 공격할 수 있는 지 확인

    void Attack(EnemyController controller);        // 실제 공격
}
