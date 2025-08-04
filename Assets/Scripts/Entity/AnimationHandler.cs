using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    // �ִϸ��̼� ��� ���� �Ķ���� �ؽð���
    private static readonly int IsMoving = Animator.StringToHash("IsMove");
    private static readonly int HurtTrigger = Animator.StringToHash("IsHurt");
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");
    private static readonly int IsDied = Animator.StringToHash("IsDied");
    private static readonly int IsIdle = Animator.StringToHash("IsIdle");

    protected Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("AnimationHandler: Animator ������Ʈ�� �� GameObject �Ǵ� �� �ڽĿ��� ã�� �� �����ϴ�!");
        }
    }
    public void Idle()
    {
        if (animator != null)
        {
            animator.SetTrigger(IsIdle);
        }
    }
    public void Move(Vector2 obj)
    {
        if (animator != null)
        {
            animator.SetBool(IsMoving, obj.magnitude > .5f);
        }
    }
    public void Hurt()
    {
        if (animator != null)
        {
            animator.SetTrigger(HurtTrigger);
            Debug.Log("[AnimationHandler] Hurt ȣ���: HurtTrigger�� ȣ���߽��ϴ�.");
        }
        if (animator == null)
        {
            Debug.Log("�ִϸ����Ͱ� ����");
        }
    }

    public void Attack()
    {
        if (animator != null)
        {
            animator.SetTrigger(AttackTrigger);
            Debug.Log("[AnimationHandler] Attack ȣ���: AttackTrigger�� ȣ���߽��ϴ�.");
        }
    }

    public void Death()
    {
        if (animator != null)
        {
            animator.SetBool(IsDied, true);
            Debug.Log("[AnimationHandler] Death ȣ���: IsDied�� true�� �����߽��ϴ�.");
        }
    }
}