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
            ResourceController rc = other.GetComponent<ResourceController>();
            if (rc != null)
            {
                rc.ChangeHealth(-damage);
                Debug.Log("대검 공격! 피해: " + damage);
            }
        }
    }
}