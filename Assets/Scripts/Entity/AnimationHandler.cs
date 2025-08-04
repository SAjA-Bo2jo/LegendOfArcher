using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("IsMove");
    private static readonly int IsHurt = Animator.StringToHash("IsHurt");
    private static int IsAttack = Animator.StringToHash("IsAttack"); // IsAttack�� ���� SetTrigger�� ���������, ���⼭�� SetBool�� �Ǿ��ֳ׿�.
    private static readonly int IsDied = Animator.StringToHash("IsDied");
    protected Animator animator;

    protected virtual void Awake()
    {
        // �� ��ũ��Ʈ�� ������ GameObject�� �ڽ� �Ǵ� ���ο��� Animator ������Ʈ�� ã���ϴ�.
        // AnimationHandler�� ���� ��������Ʈ GameObject�� �ְ�, Animator�� �ű⿡ �ִٸ� GetComponent<Animator>()�� ����������,
        // GetComponentInChildren<Animator>()�� Ȥ�� Animator�� �� �ٸ� �ڽĿ� ���� ��쿡�� ã���ֹǷ� �����մϴ�.
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("AnimationHandler: Animator ������Ʈ�� �� GameObject �Ǵ� �� �ڽĿ��� ã�� �� �����ϴ�! �ùٸ� GameObject�� Animator�� AnimationHandler�� �Բ� �ִ��� Ȯ���ϼ���.");
        }
    }

    // ���� �ִϸ��̼� �� ���� �ð� ����� ���õ� �޼��� (�̸��� InvincibilityEnd�ε� SetBool(IsHurt, false)�� ��)
    public void InvincibilityEnd()
    {
        if (animator != null)
        {
            animator.SetBool(IsHurt, false);
            Debug.Log("[AnimationHandler] InvincibilityEnd ȣ���: IsHurt�� false�� �����߽��ϴ�.");
        }
        else
        {
            // �� ���� Animator�� null�� �� Awake���� �̹� ��µǾ��� ���ɼ��� �����ϴ�.
            // Debug.LogWarning("AnimationHandler: Animator�� ���� InvincibilityEnd�� ������ �� �����ϴ�.");
        }
    }

    // �̵� �ִϸ��̼��� �����ϴ� �޼���
    public void Move(Vector2 obj)
    {
        if (animator != null)
        {
            animator.SetBool(IsMoving, obj.magnitude > .5f);
        }
        // else { Debug.LogWarning("AnimationHandler: Animator�� ���� Move �ִϸ��̼��� ������ �� �����ϴ�."); }
    }

    // ���� �ִϸ��̼��� ����ϴ� �޼���
    public void Hurt()
    {
        if (animator != null)
        {
            animator.SetBool(IsHurt, true);
            Debug.Log("[AnimationHandler] Hurt ȣ���: IsHurt�� true�� �����߽��ϴ�.");
        }
        // else { Debug.LogWarning("AnimationHandler: Animator�� ���� Hurt �ִϸ��̼��� ����� �� �����ϴ�."); }
    }

    // �� �޼��带 �ִϸ��̼� �̺�Ʈ�� ȣ���մϴ�. (�ַ� Hurt �ִϸ��̼� ���� ��)
    public void ResetHurtAnimation()
    {
        if (animator != null)
        {
            animator.SetBool(IsHurt, false);
            Debug.Log("[AnimationHandler] ResetHurtAnimation ȣ���: IsHurt�� false�� �����߽��ϴ�.");
            // �ʿ��ϴٸ� Idle ���·� ���ư��� ���� �ٸ� �Ҹ��� ������ �� �ֽ��ϴ�.
            // ��: �������� ���� ���¶�� IsMoving�� false�� ����
            // animator.SetBool(IsMoving, false);
        }
        // else { Debug.LogWarning("AnimationHandler: Animator�� ���� ResetHurtAnimation�� ������ �� �����ϴ�."); }
    }

    // ���� �ִϸ��̼��� ����ϴ� �޼���
    public void Attack()
    {
        if (animator != null)
        {
            animator.SetBool(IsAttack, true); // �Ϲ������� Attack�� Trigger�� �����մϴ� (SetTrigger("AttackTrigger"))
            Debug.Log("[AnimationHandler] Attack ȣ���: IsAttack�� true�� �����߽��ϴ�.");
        }
        // else { Debug.LogWarning("AnimationHandler: Animator�� ���� Attack �ִϸ��̼��� ����� �� �����ϴ�."); }
    }

    // ��� �ִϸ��̼��� ����ϴ� �޼���
    public void Death()
    {
        if (animator != null)
        {
            animator.SetBool(IsDied, true);
            Debug.Log("[AnimationHandler] Death ȣ���: IsDied�� true�� �����߽��ϴ�.");
        }
        // else { Debug.LogWarning("AnimationHandler: Animator�� ���� Death �ִϸ��̼��� ����� �� �����ϴ�."); }
    }
}