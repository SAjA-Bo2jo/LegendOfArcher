using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyAttack : IEnemyAttack
{
    private float lastAttackTime = 0f;
    private bool isAttacking = false;

    public bool CanAttack(EnemyController controller)
    {
        return !isAttacking && Time.time > lastAttackTime + 1.5f;
    }

    public void Attack(EnemyController controller)
    {
        lastAttackTime = Time.time;
        isAttacking = true;

        controller.StartCoroutine(SwordSlash(controller));
    }

    private System.Collections.IEnumerator SwordSlash(EnemyController controller)
    {

    }

    private EnemyMeleeWeaponAttack GetSwordDamageHandler(EnemyController controller)
    {
        Transform weaponPivot = controller.transform.Find("WeaponPivot");
        if (weaponPivot != null)
        {
            Transform sword = weaponPivot.Find("Sword");
            if (sword != null)
            {
                return sword.GetComponent<EnemyMeleeWeaponAttack>();
            }
        }
        return null;
    }
}
