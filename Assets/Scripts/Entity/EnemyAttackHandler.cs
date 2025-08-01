using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHandler : MonoBehaviour
{
    [Header("Temporary Attack Stat")]                           // 임시 수동 스탯 입력
    [SerializeField] protected float attackDamage = 5f;         // attackDamage : 공격 데미지
    [SerializeField] protected float attackRange = 1f;          // attackRange : 공격 사거리
    [SerializeField] protected float attackCooldown = 1.5f;     // attackCooldawn : 공격 쿨타임

    // 능력치 프로퍼티 -> 나중에 StatManager나 DB에서 끌고오면 변경
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
    protected Transform target;                                 // Target : 공격 대상 (보통 플레이어)

    private float lastAttackTime = float.MinValue;              // lastAttackTime : 마지막 공격 시간 -> 쿨타임에 응용
    protected bool isAttacking = false;                         // isAttacking : 현재 공격 중인지 여부

    protected virtual void Awake()
    {

    }

    public virtual void SetTarget (Transform newTarget)         // EnemyController에서 플레이어를 대상으로 설정 -> 이를 받아와서 똑같이 대상 설정
    {
        target = newTarget;
    }

    public virtual bool CanAttack()                             // 공격 가능 여부 확인하는 메서드 -> 조건문에 넣어서 쓰기
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (target == null) return false;
        if (animationHandler != null && animationHandler.IsDead) return false;
        if (isAttacking) return false;
        if (Time.time - lastAttackTime < attackDamage) return false;
        if (distanceToTarget > AttackRange) return false;

        return true;
    }

    public virtual void DoAttack()                              // 실제 공격 메서드
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
