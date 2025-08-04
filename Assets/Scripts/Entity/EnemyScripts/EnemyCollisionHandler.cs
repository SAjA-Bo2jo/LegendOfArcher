// EnemyCollisionHandler.cs 전체 교체
using UnityEngine;

public class EnemyCollisionHandler : MonoBehaviour
{
    private EnemyController controller;

    private void Awake()
    {
        controller = GetComponent<EnemyController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        controller.ApplyContactDamage(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        controller.ApplyContactDamage(other);
    }
}