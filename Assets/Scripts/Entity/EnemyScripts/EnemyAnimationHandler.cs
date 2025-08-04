using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationHandler : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("IsMove");     // IsMoving : bool IsMove ��
    private static readonly int IsDamage = Animator.StringToHash("IsDamage");   // IsDamage : Ʈ���� IsDamage
    private static readonly int Die = Animator.StringToHash("Die");             // Die : Ʈ���� Die
    private static readonly int Attack = Animator.StringToHash("Attack");       // Attack : Ʈ���� Attack

    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    private bool isDead = false;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public virtual void Move(Vector2 moveVector)                                // �̵� �ִϸ��̼� ��� ����
    {
        if (isDead) return;

        bool isMoving = moveVector.magnitude > 0.1f;                            // ���� ���� �̻��� ������ ũ�Ⱑ ������ �ִϸ��̼� ���
        animator.SetBool(IsMoving, isMoving);
    }

    public virtual void HandleDirection(Vector2 direction)                      // ����(flipX) ���� ����
    {
        if (spriteRenderer == null || direction.magnitude < 0.1f) return;
        
        spriteRenderer.flipX = direction.x < 0f;                                // ������ -�� ����, ����� ������ ���� ��.
    }

    public virtual void Damage()                                                // ������ �޾��� �� �ִϸ��̼� ��� ����
    {
        if (isDead) return;
        
        animator.SetTrigger(IsDamage);
    }

    public virtual void AttackTarget()                                          // ���� �ִϸ��̼� ��� ����
    {
        if (isDead) return;

        animator.SetTrigger(Attack);                                            // Attack Ʈ���� �Ѽ� ���� �ִϸ��̼� ���
    }

    public virtual void Death()                                                 // �� óġ �ִϸ��̼� ��� ����
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger(Die);                                               // Die Ʈ���� �Ѽ� Death �ִϸ��̼� �����
    }
}
