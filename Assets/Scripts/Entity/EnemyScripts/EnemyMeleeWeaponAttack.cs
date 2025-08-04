using System.Collections;
using UnityEngine;

public class EnemyMeleeWeaponAttack : MonoBehaviour
{
    private float damage = 0f;
    private bool isAttacking = false;

    public void StartAttack(float attackDamage)
    {
        damage = attackDamage;
        isAttacking = true;
        StartCoroutine(EndAttackAfterTime());
    }

    private IEnumerator EndAttackAfterTime()
    {
        yield return new WaitForSeconds(0.6f);
        isAttacking = false;
        damage = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAttacking && other.CompareTag("Player"))
        {
            /*
            ResourceController rc = other.GetComponent<ResourceController>();
            if (rc != null)
            {
                rc.ChangeHealth(-damage);
                Debug.Log("��� ����! ����: " + damage);
            }
            */

            Player player = other.GetComponent<Player>();

            if (player == null)
                Debug.LogWarning("[Boss Melee Weapon Attack] can not find \"Player\" component");

            player.TakeDamage(damage, gameObject);
            Debug.Log("Great Sword Attack! Damage: " + damage);

            BaseController controller = other.GetComponentInParent<BaseController>();
            if (controller != null)
            {
                controller.ApplyKnockback(transform, 4f, 0.3f);
            }
        }
    }
}