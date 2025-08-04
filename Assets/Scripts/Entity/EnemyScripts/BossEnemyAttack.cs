using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyAttack : IEnemyAttack
{
    private float lastAttackTime = 0f;
    private bool isSecondPhase = false;

    public bool CanAttack(EnemyController controller)
    {
        if (controller.Stats.healthPoint <= controller.Stats.maxHealth * 0.5)
        {
            isSecondPhase = true;
        }

        float cooldown = isSecondPhase ? 0.5f : 1f;
        return Time.time > lastAttackTime + cooldown;
    }

    public void Attack(EnemyController controller)
    {

    }
}
