using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("IsMove");
    private static readonly int IsHurt = Animator.StringToHash("IsHurt");
    private static int IsAttack = Animator.StringToHash("IsAttack"); // IsAttack은 보통 SetTrigger를 사용하지만, 여기서는 SetBool로 되어있네요.
    private static readonly int IsDied = Animator.StringToHash("IsDied");
    protected Animator animator;

    protected virtual void Awake()
    {
        // 이 스크립트가 부착된 GameObject의 자식 또는 본인에서 Animator 컴포넌트를 찾습니다.
        // AnimationHandler가 메인 스프라이트 GameObject에 있고, Animator도 거기에 있다면 GetComponent<Animator>()도 가능하지만,
        // GetComponentInChildren<Animator>()는 혹시 Animator가 또 다른 자식에 있을 경우에도 찾아주므로 안전합니다.
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("AnimationHandler: Animator 컴포넌트를 이 GameObject 또는 그 자식에서 찾을 수 없습니다! 올바른 GameObject에 Animator와 AnimationHandler가 함께 있는지 확인하세요.");
        }
    }

    // 피해 애니메이션 후 무적 시간 종료와 관련된 메서드 (이름이 InvincibilityEnd인데 SetBool(IsHurt, false)만 함)
    public void InvincibilityEnd()
    {
        if (animator != null)
        {
            animator.SetBool(IsHurt, false);
            Debug.Log("[AnimationHandler] InvincibilityEnd 호출됨: IsHurt를 false로 설정했습니다.");
        }
        else
        {
            // 이 경고는 Animator가 null일 때 Awake에서 이미 출력되었을 가능성이 높습니다.
            // Debug.LogWarning("AnimationHandler: Animator가 없어 InvincibilityEnd를 실행할 수 없습니다.");
        }
    }

    // 이동 애니메이션을 제어하는 메서드
    public void Move(Vector2 obj)
    {
        if (animator != null)
        {
            animator.SetBool(IsMoving, obj.magnitude > .5f);
        }
        // else { Debug.LogWarning("AnimationHandler: Animator가 없어 Move 애니메이션을 제어할 수 없습니다."); }
    }

    // 피해 애니메이션을 재생하는 메서드
    public void Hurt()
    {
        if (animator != null)
        {
            animator.SetBool(IsHurt, true);
            Debug.Log("[AnimationHandler] Hurt 호출됨: IsHurt를 true로 설정했습니다.");
        }
        // else { Debug.LogWarning("AnimationHandler: Animator가 없어 Hurt 애니메이션을 재생할 수 없습니다."); }
    }

    // 이 메서드를 애니메이션 이벤트로 호출합니다. (주로 Hurt 애니메이션 종료 시)
    public void ResetHurtAnimation()
    {
        if (animator != null)
        {
            animator.SetBool(IsHurt, false);
            Debug.Log("[AnimationHandler] ResetHurtAnimation 호출됨: IsHurt를 false로 설정했습니다.");
            // 필요하다면 Idle 상태로 돌아가기 위한 다른 불리언도 조작할 수 있습니다.
            // 예: 움직임이 없는 상태라면 IsMoving도 false로 설정
            // animator.SetBool(IsMoving, false);
        }
        // else { Debug.LogWarning("AnimationHandler: Animator가 없어 ResetHurtAnimation을 실행할 수 없습니다."); }
    }

    // 공격 애니메이션을 재생하는 메서드
    public void Attack()
    {
        if (animator != null)
        {
            animator.SetBool(IsAttack, true); // 일반적으로 Attack은 Trigger로 설정합니다 (SetTrigger("AttackTrigger"))
            Debug.Log("[AnimationHandler] Attack 호출됨: IsAttack을 true로 설정했습니다.");
        }
        // else { Debug.LogWarning("AnimationHandler: Animator가 없어 Attack 애니메이션을 재생할 수 없습니다."); }
    }

    // 사망 애니메이션을 재생하는 메서드
    public void Death()
    {
        if (animator != null)
        {
            animator.SetBool(IsDied, true);
            Debug.Log("[AnimationHandler] Death 호출됨: IsDied를 true로 설정했습니다.");
        }
        // else { Debug.LogWarning("AnimationHandler: Animator가 없어 Death 애니메이션을 재생할 수 없습니다."); }
    }
}