using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAttack : IEnemyAttack
{
    private float lastAttackTime = 0f;                                          // ��Ÿ�� ����
        
    public bool CanAttack(EnemyController controller)
    {
        return Time.time > lastAttackTime + controller.Stats.attackCooldown;    // ��Ÿ�Ӻ��� �ð��� �� ������ ���� ����
    }

    public void Attack(EnemyController controller)
    {
        lastAttackTime = Time.time;

        CreateProjectile(controller);

        Debug.Log($"{controller.name} ���Ÿ�");
    }

    private void CreateProjectile(EnemyController controller)
    {
        GameObject arrow = ObjectPoolManager.Instance.Get("EnemyArrow");
        if (arrow == null)
            Debug.Log("key \"EnemyArrow\" is not set in Object Pool");

        Vector2 spawnPosition = (Vector2)controller.transform.position + controller.DirectionToTarget() * 0.5f;
        arrow.transform.position = spawnPosition;
        
        EnemyArrow arrowScript = arrow.GetComponent<EnemyArrow>();
        if (arrowScript != null)
        {
            arrowScript.Initialize(controller.Stats.attackDamage, controller.DirectionToTarget());
        }
        else
        {
            Debug.LogWarning("Can not find \"EnemyArrow.cs\" conpontent from EnemyArrow prefab!");
        }
                
    }
}
