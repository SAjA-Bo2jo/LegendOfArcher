using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAttack : IEnemyAttack
{
    private float lastAttackTime = 0f;                                          // 쿨타임 변수

    public bool CanAttack(EnemyController controller)
    {
        return Time.time > lastAttackTime + controller.Stats.attackCooldown;    // 쿨타임보다 시간이 더 지나야 공격 가능
    }

    public void Attack(EnemyController controller)
    {
        lastAttackTime = Time.time;

        CreateProjectile(controller);

        Debug.Log($"{controller.name} 원거리");
    }

    private void CreateProjectile(EnemyController controller)
    {
        GameObject arrowPrefab = controller.ArrowPrefab;

        if (arrowPrefab != null )
        {
            Vector2 spawnPosition = (Vector2)controller.transform.position + controller.DirectionToTarget() * 0.5f;

            GameObject arrow = Object.Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);

            EnemyArrow arrowScript = arrow.GetComponent<EnemyArrow>();
            if (arrowScript != null)
            {
                //arrowScript.Initialize(controller.Stats.attackDamage, controller.DirectionToTarget());
            }
        }
        else
        {
            Debug.Log($"{controller.name} ArrowPrefab 설정 X");
        }
    }
}
