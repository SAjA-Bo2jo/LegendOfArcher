using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    // 애니메이션 제어를 위한 파라미터 해시값들
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
            Debug.LogError("AnimationHandler: Animator 컴포넌트를 이 GameObject 또는 그 자식에서 찾을 수 없습니다!");
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
            Debug.Log("[AnimationHandler] Hurt 호출됨: HurtTrigger를 호출했습니다.");
        }
        if (animator == null)
        {
            Debug.Log("애니메이터가 없음");
        }
    }

    public void Attack()
    {
        if (animator != null)
        {
            animator.SetTrigger(AttackTrigger);
            Debug.Log("[AnimationHandler] Attack 호출됨: AttackTrigger를 호출했습니다.");
        }
    }

    public void Death()
    {
        if (animator != null)
        {
            animator.SetBool(IsDied, true);
            Debug.Log("[AnimationHandler] Death 호출됨: IsDied를 true로 설정했습니다.");
        }
    }
}