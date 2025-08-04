using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyAttack : IEnemyAttack
{
    public bool CanAttack(EnemyController controller)
    {
        return true;
    }

    public void Attack(EnemyController controller)
    {

    }
}
