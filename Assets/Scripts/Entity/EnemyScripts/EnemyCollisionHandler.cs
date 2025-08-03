using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionHandler : MonoBehaviour
{
    private EnemyAnimationHandler enemyAnimationHandler;
    private EnemyController enemyController;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
    }

    private void OnTriggerEnter2D(Collider2D playerCollider)                    // 충돌 감지 메서드
    {
        if (playerCollider.CompareTag("Player"))
        {
            if (enemyController.IsDead) return;

            if (enemyController != null)
            {
                enemyController.ApplyContactDamage(playerCollider);
            }
        }
    }
}
