using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseController
{
    private Transform target;

    [SerializeField] private float detectRange = 15f;                       // detectRange : 적의 플레이어 감지 거리

    [Header("Temporary setting")]                                           // 임시 세팅값. 사정거리, 적정 거리 비율은 나중에 stat 클래스나 몬스터 DB로 대체될 예정임
    [SerializeField] private float attackRange = 6f;                        // attackRange : 적의 사정거리
    [SerializeField] private float optimalDistanceRatio = 0.75f;            // optimalDistanceRatio : 적이 접근하는 거리의 비율
    [SerializeField] private float distanceTolerance = 0.5f;                // distanceTolerance : 거리 허용 오차
    [SerializeField] private bool canRetreat = true;                        // canRetreat : 적이 후퇴할 수 있는지 여부

    private float OptimalDistance => attackRange * optimalDistanceRatio;    // OptimalDistance : 계산된 최적거리

    public void Init(Transform target)                                      // 추적 대상 정하는 메서드
    {
        this.target = target;
    }
    
    protected float DistanceToTarget()                                      // 플레이어 ~ 적 거리 구하는 메서드
    {
        return Vector2.Distance(transform.position, target.position);
    }

    protected Vector2 DirectionToTarget()                                   // 적 -> 플레이어 단위벡터 구함
    {
        return (target.position - transform.position).normalized;
    }

    public bool IsInAttackRange()                                           // 공격 사거리 내에 플레이어 있는 지 확인
    {
        if (target == null) return false;
        return DistanceToTarget() <= attackRange;
    }

    protected override void HandleInput()                                   // 적 개체 추적 AI
    {
        if (target == null)                                         // 추적 대상(플레이어) 없을 경우
        {
            moveDirection = Vector2.zero;                           // -> 이동 중지
            lookDirection = Vector2.zero;                           // -> 회정 중지
            return;
        }

        float distance = DistanceToTarget();       
        Vector2 direction = DirectionToTarget();

        lookDirection = direction;

        if (distance <= detectRange)                                // 추적 범위 내일 경우
        {
            float optimalDist = OptimalDistance;

            if (distance >  optimalDist + distanceTolerance)        // 경우 1. 플레이어가 너무 멀리 있을 경우
            {
                moveDirection = direction;                          // 플레이어 방향으로 이동
            }
            else if (distance < optimalDist - distanceTolerance)    // 경우 2. 플레이어가 너무 가까이 있을 경우
            {
                if (canRetreat)                                     
                {
                    moveDirection -= direction;                     // 후퇴 가능 개체는 후퇴
                }
                else
                {
                    moveDirection = Vector2.zero;                   // 후퇴 불가능 시 정지
                }
            }
            else
            {
                moveDirection = Vector2.zero ;                      // 경우3. 최적거리 범위 내일 경우
            }
        }
        else { moveDirection = Vector2.zero ; }                     // 추적 범위 외일 경우 : 정지
    }
}
