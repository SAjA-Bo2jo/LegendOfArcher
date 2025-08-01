using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHandler : MonoBehaviour
{
    [Header("Temporary Attack Stat")]                           // �ӽ� ���� ���� �Է�
    [SerializeField] protected float attackDamage = 5f;         // attackDamage : ���� ������
    [SerializeField] protected float attackRange = 1f;          // attackRange : ���� ��Ÿ�
    [SerializeField] protected float attackCooldown = 1.5f;     // attackCooldawn : ���� ��Ÿ��

    // �ɷ�ġ ������Ƽ -> ���߿� StatManager�� DB���� ������� ����
    public virtual float AttackDamage
    {
        get => attackDamage;
        protected set => attackDamage = value;
    }
    
    public virtual float AttackRange
    {
        get => attackRange;
        protected set => attackRange = value;
    }

    public virtual float AttackCooldown
    {
        get => attackCooldown;
        protected set => attackCooldown = value;
    }

    protected EnemyAnimationHandler animationHandler;
    protected Transform target;                                 // Target : ���� ��� (���� �÷��̾�)

    private float lastAttackTime = float.MinValue;              // lastAttackTime : ������ ���� �ð� -> ��Ÿ�ӿ� ����
    protected bool isAttacking = false;                         // isAttacking : ���� ���� ������ ����

    protected virtual void Awake()
    {

    }

    public virtual void SetTarget (Transform newTarget)         // EnemyController���� �÷��̾ ������� ���� -> �̸� �޾ƿͼ� �Ȱ��� ��� ����
    {
        target = newTarget;
    }

    public virtual bool CanAttack()                             // ���� ���� ���� Ȯ���ϴ� �޼��� -> ���ǹ��� �־ ����
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (target == null) return false;
        if (animationHandler != null && animationHandler.IsDead) return false;
        if (isAttacking) return false;
        if (Time.time - lastAttackTime < attackDamage) return false;
        if (distanceToTarget > AttackRange) return false;

        return true;
    }

    public virtual void DoAttack()                              // ���� ���� �޼���
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        if (animationHandler != null)
        {
            animationHandler.AttackTarget();
        }
    }

    public virtual void OnHitTarget(Collider2D targetCollider)
    {

    }
}
