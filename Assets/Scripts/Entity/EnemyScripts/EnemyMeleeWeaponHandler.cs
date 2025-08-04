using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeWeaponAttack : MonoBehaviour
{
    private float damage = 0f;
    private bool isAttacking = false;
    private EnemyController controller;

    private void Awake()
    {
        controller = GetComponentInParent<EnemyController>();
    }

    public void StartAttack(float attackDamage)
    {
        damage = attackDamage;
        isAttacking = true;
    }

    public void EndAttack()
    {
        isAttacking = false;
        damage = 0f;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAttacking) return;

        if (collision.CompareTag("Player"))
        {
            ResourceController resourceController = collision.GetComponent<ResourceController>();
            
            if (resourceController != null)
            {
                bool damageApplied = resourceController.ChangeHealth(-damage);

                if (damageApplied)
                {
                    BaseController baseController = collision.GetComponent<BaseController>();

                    if (baseController != null)
                    {
                        baseController.ApplyKnockback(controller.transform, 8f, 0.6f);
                    }
                }
            }
        }
    }
}
