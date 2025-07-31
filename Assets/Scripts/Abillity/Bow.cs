using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bow : Abillity
{
    protected override void Awake()
    {
        player = GetComponent<Player>();                        // 플레이어 컴포넌트 가져오기
        animationHandler = GetComponent<AnimationHandler>();    // 애니메이션 핸들러 가져오기
    }

    protected override void Update()
    {
        target = FindTarget();                                  // 매 프레임마다 대상 갱신
        TryAttack();                                            // 공격 시도
    }


    protected override GameObject FindTarget()  // 공격 대상 탐색
    {
        GameObject target = GameObject
            .FindGameObjectsWithTag("Enemy")                                                   // "Enemy" 태그를 가진 오브젝트들 중
            .Where(enemy => enemy.activeInHierarchy)                                           // 활성화된 것만 필터링
            .Where(enemy => Vector3.Distance(enemy.transform.position, transform.position) < player.AttackRange) // 사정거리 이내
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position))  // 가장 가까운 순으로 정렬
            .FirstOrDefault();                                                                 // 가장 가까운 대상 선택

        return target;
    }

    protected override float AttackDelay()   // 공격 딜레이 계산 (공격속도 기반), 공격속도가 높을수록 딜레이는 짧아짐
    {
        float attackSpeed = player.AttackSpeed;                     // 기본 공격속도
        float multiplier = player.AttackSpeedMultiplier / 100f;     // 공격속도 배율 (퍼센트를 1.x로 변환)
        float totalAttackSpeed = attackSpeed * multiplier;          // 최종 공격속도
        float delay = 1f / totalAttackSpeed;                        // 공격속도를 딜레이(초)로 변환

        return delay;
    }


    protected override void TryAttack()  // 공격을 시도하는 함수
    {
        if (target == null) return;                           // 대상 없으면 무시

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();  // Rigidbody2D를 통해 플레이어의 정지 여부 확인
        if (rb != null && rb.velocity.magnitude > 0.01f)
            return;                                           // 플레이어가 이동 중이면 공격하지 않음

        float delay = AttackDelay();                          // 현재 공격 딜레이 계산

        if (Time.time >= lastAttackTime + delay)              // 현재 시간(Time.time)이 마지막 공격 시간 + 딜레이를 초과했는지 확인
        {
            PerformAttack();                                  // 공격 실행
            lastAttackTime = Time.time;                       // 마지막 공격 시간 갱신
        }
    }

    protected override void PerformAttack()      // 실제 공격 로직 수행 (이 부분을 상속해서 각 능력마다 다르게 구현 가능)
    {
        Debug.Log("공격 실행됨! 대상: " + target.name);
        // 여기서 실제 투사체 생성이나 데미지 계산 등을 구현

    }
}
