using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionHandler : MonoBehaviour
{
    [SerializeField] private float contactDamage = 5f;                          // contactDamage : 충돌 공격력

    private EnemyAnimationHandler enemyAnimationHandler;

    private void Awake()
    {
        enemyAnimationHandler = GetComponentInParent<EnemyAnimationHandler>();

        if (enemyAnimationHandler == null )
        {
            Debug.Log($"{gameObject.name}: EnemyAnimationHandler 없음");
        }
    }

    private void OnTriggerEnter2D(Collider2D playerCollider)                    // 충돌 시 실행 메서드 (플레이어에게 충돌 데미지 적용)
    {
        if (playerCollider.CompareTag("Player")) return;                        // 충돌 물체가 Player 맞는 지 확인    
        if (enemyAnimationHandler != null &&
            enemyAnimationHandler.IsDead) return;                               // 적 개체 사망 상태인 지 확인

        ApplyContactDamageToCollider(playerCollider);                           // 충돌 데미지 적용
        Debug.Log($"{transform.parent.name}이 플레이어에게 접촉, {contactDamage} 데미지");
    }

    private void ApplyContactDamageToCollider(Collider2D Collider)               // 접촉 데미지를 주는 메서드
    {
        ResourceController Rescource = Collider.GetComponent<ResourceController>();
        if (Rescource != null)
        {
            Rescource.ChangeHealth(-contactDamage);
        }
        else
        {
            Debug.LogWarning($"Player에 ResourceController 없음 : {Collider.name}");
        }

        AnimationHandler Animation = Collider.GetComponent<AnimationHandler>();
        if ( Animation != null )
        {
            Animation.Damage();
        }
        else
        {
            Debug.LogWarning($"Player에 AnimationHandler 없음 : {Collider.name}");
        }
    }
}
