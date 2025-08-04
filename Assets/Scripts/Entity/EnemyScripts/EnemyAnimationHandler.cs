using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationHandler : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("IsMove");     // IsMoving : bool IsMove 값
    private static readonly int IsDamage = Animator.StringToHash("IsDamage");   // IsDamage : 트리거 IsDamage
    private static readonly int Die = Animator.StringToHash("Die");             // Die : 트리거 Die
    private static readonly int Attack = Animator.StringToHash("Attack");       // Attack : 트리거 Attack

    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    private bool isDead = false;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public virtual void Move(Vector2 moveVector)                                // 이동 애니메이션 재생 관리
    {
        if (isDead) return;

        bool isMoving = moveVector.magnitude > 0.1f;                            // 일정 수준 이상의 움직임 크기가 있으면 애니메이션 재생
        animator.SetBool(IsMoving, isMoving);
    }

    public virtual void HandleDirection(Vector2 direction)                      // 방향(flipX) 결정 관리
    {
        if (spriteRenderer == null || direction.magnitude < 0.1f) return;
        
        spriteRenderer.flipX = direction.x < 0f;                                // 각도가 -면 왼쪽, 양수면 오른쪽 보게 함.
    }

    public virtual void Damage()                                                // 데미지 받았을 때 애니메이션 재생 관리
    {
        if (isDead) return;
        
        animator.SetTrigger(IsDamage);
    }

    public virtual void AttackTarget()                                          // 공격 애니메이션 재생 관리
    {
        if (isDead) return;

        animator.SetTrigger(Attack);                                            // Attack 트리거 켜서 공격 애니메이션 재생
    }

    public virtual void Death()                                                 // 적 처치 애니메이션 재생 관리
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger(Die);                                               // Die 트리거 켜서 Death 애니메이션 재생함
    }
}
