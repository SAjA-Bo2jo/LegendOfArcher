using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public enum EnemyBehaviorType
{
    Melee,
    Ranged,
    Boss
}

public class EnemyController : BaseController
{
    [Header("능력치")]
    [SerializeField] private EnemyStats stats;
    public EnemyStats Stats { get { return stats; } }

    [Header("유닛 타입")]
    [SerializeField] private EnemyBehaviorType behaviorType;    // 해당 유닛 타입 인스펙터 입력

    private Transform target;
    private EnemyAnimationHandler _animation;
    private IEnemyAttack EnemyAttack;

    bool isDead = false;
    public bool IsDead => isDead;



    // 생명주기 함수
    protected override void Awake()
    {
        base.Awake();

        switch (behaviorType)                                   // -> 인스펙터 값 바탕으로 유닛 타입 판단
        {
            case EnemyBehaviorType.Melee:
                EnemyAttack = new MeleeEnemyAttack();
                break;

            case EnemyBehaviorType.Ranged:
                EnemyAttack = new RangedEnemyAttack();
                break;

            case EnemyBehaviorType.Boss:
                EnemyAttack = new BossEnemyAttack();
                break;
        } 
    }

    protected override void Start()
    {
        base.Start();

        stats.StatInitialize();

        Init(StageManager.Instance._Player.transform);
        _animation = GetComponent<EnemyAnimationHandler>();
    }

    public void Init(Transform target)                                      // 초기화 시 추적 대상 정하는 메서드
    {
        this.target = target;
    }



    // 유틸리티 메서드
    protected float DistanceToTarget()                                      // DistanceToTarget 메서드 : 플레이어 ~ 적 거리 구하는 메서드
    {
        return Vector2.Distance(transform.position, target.position);
    }

    protected Vector2 DirectionToTarget()                                   // DirectionToTarget 메서드 : 적 -> 플레이어 단위벡터 구함
    {
        return (target.position - transform.position).normalized;
    }

    public bool IsInAttackRange()                                           // IsInAttackRange 메서드 : 공격 사거리 내에 플레이어 있는 지 확인
    {
        if (target == null) return false;
        return DistanceToTarget() <= stats.attackRange;
    }



    // HandleInput 메서드 : 적 개체의 타겟(플레이어) 추적 시스템
    protected override void HandleInput()
    {
        if (target == null || stats.healthPoint <= 0)                       // 추적 대상(플레이어) 없거나 적 개체가 죽었을 경우
        {
            moveDirection = Vector2.zero;                                   // -> 이동 중지
            lookDirection = Vector2.zero;                                   // -> 회정 중지
            return;
        }

        float distance = DistanceToTarget();       
        Vector2 direction = DirectionToTarget();

        lookDirection = direction;                                          // 적 개체는 타겟을 계속 바라봄

        if (distance <= stats.detectRange)                                  // 추적 범위 내일 경우
        {
            float optimalDist = stats.OptimalDistance;

            if (distance >  optimalDist + stats.distanceTolerance)          // 경우 1. 플레이어가 너무 멀리 있을 경우
            {
                moveDirection = direction;                                  // 플레이어 방향으로 이동
            }
            else if (distance < optimalDist - stats.distanceTolerance)      // 경우 2. 플레이어가 너무 가까이 있을 경우
            {
                if (stats.canRetreat)                                     
                {
                    moveDirection -= direction;                             // 후퇴 가능 개체는 후퇴
                }
                else
                {
                    moveDirection = Vector2.zero;                           // 후퇴 불가능 시 정지
                }
            }
            else
            {
                moveDirection = Vector2.zero ;                              // 경우3. 최적거리 범위 내일 경우
            }
        }
        else 
        { 
            moveDirection = Vector2.zero;                                   // 추적 범위 외일 경우 : 정지
        }

        if (IsInAttackRange() && EnemyAttack.CanAttack(this))               // 이동 끝 -> 사거리 내에 타겟 있으면 공격
        {
            EnemyAttack.Attack(this);
        }
    }



    // 적 개체 사망 처리 관련 메서드
    public void GetDamage(float dmg)                                        // GetDamage 메서드 : 적 개체 피격 + 사망 판정 처리
    {
        Debug.Log("데미지가 들어갔습니다" + dmg);
        
        isDead = stats.TakeDamage(dmg);

        if (isDead)
        {
            HandleDeath();
        }
        else
        {
            _animation.Death();
        }
    }

    private void HandleDeath()                                              // HandleDeath 메서드 : 적 개체 사망 처리
    {
        _animation.Damage();                                                // 애니메이션 실행

        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()                                    // Death 애니메이션 재생 완료 후 오브젝트 삭제
    {
        yield return new WaitForSeconds(0.4f);

        StageManager.Instance.RemoveMonsterFromList(gameObject);

        EnemyPoolObject poolObject = GetComponent<EnemyPoolObject>();
        if (poolObject != null)
        {
            poolObject.ReturnToPool();                                      // Pool로 죽은 개체 돌려보냄
        }
        else
        {
            Debug.Log($"{gameObject.name}: EnemyPoolObject 없음 Destroy로 대체");
            Destroy(gameObject);
        }
    }



    // 접촉 상대에게 데미지 주는 메서드
    public void ApplyContactDamage(Collider2D collider)
    {
        float damage = stats.contactDamage;

        ResourceController resourceController = collider.GetComponent<ResourceController>();
        if (resourceController != null)
        {
            resourceController.ChangeHealth(-damage);
        }
    }
}
