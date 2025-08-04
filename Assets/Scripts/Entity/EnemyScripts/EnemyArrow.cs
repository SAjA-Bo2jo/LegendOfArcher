using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
    private float damage = 0f;
    private float speed = 5f;
    private Vector2 direction;
    private Rigidbody2D rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float attackDamage, Vector2 targetDirection)
    {
        damage = attackDamage;
        direction = targetDirection.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (rigidbody != null)
        {
            rigidbody.velocity = direction * speed;
        }

        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            /*
            ResourceController resourceController = collision.GetComponent<ResourceController>();

            if (resourceController != null)
            {
                bool damageApplied = resourceController.ChangeHealth(-damage);
                if (damageApplied)
                {
                    BaseController playerController = collision.GetComponent<BaseController>();
                    if (playerController != null)
                    {
                        playerController.ApplyKnockback(transform, 2f, 0.3f);
                    }

                    Debug.Log($"화살, 데미지 {damage}");
                }
            }
            */

            Player player = collision.GetComponent<Player>();

            if (player != null)
            {
                player.TakeDamage(damage, this.gameObject);
                BaseController controller = player.GetComponent<BaseController>();
                if (controller != null)
                {
                    controller.ApplyKnockback(transform, 2f, 0.3f);
                }

                Debug.Log($"[EnemyArrow] {gameObject.name}이 플레이어에게 {damage} 피해를 입힘");
            }
        }

        if (collision.CompareTag("LowObject"))
            return;

        Destroy(gameObject);
    }
}
