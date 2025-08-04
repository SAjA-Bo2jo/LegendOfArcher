using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationHandler : MonoBehaviour
{
    private static readonly int IsMove = Animator.StringToHash("IsMove");     // IsMoving : bool IsMove ��
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

        // Debug.Log($"[Anim] Move called: vector={moveVector}, isMoving={isMoving}");

        animator.SetBool(IsMove, isMoving);
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

    public void ResetState()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Animator is missing in EnemyAnimationHandler!");
                return;
            }
        }

        /*
        animator.enabled = false;   // force to stop animator
        animator.enabled = true;    // initiate animator
        */

        animator.Rebind();
        animator.Update(0f);
        
        animator.SetBool("IsMove", false);

        isDead = false;
    }
}
