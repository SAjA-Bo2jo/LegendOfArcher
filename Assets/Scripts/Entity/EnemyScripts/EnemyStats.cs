using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStats
{
    [Header("체력")]
    public float maxHealth = 5f;                                        // maxHealth : 적 개체 최대 체력
    public float healthPoint = 5f;                                      // healthPoint : 체력

    [Header("공격")]
    public float attackDamage = 5f;                                     // attackDamage : 무기 공격력
    public float contactDamage = 1f;                                    // contactDamage : 접촉 공격력
    public float attackCooldown = 3f;                                   // attackCooldown : 공격 쿨타임

    [Header("이동")]
    public float moveSpeed = 2f;                                        // moveSpeed : 이동 속도
    public float detectRange = 15f;                                     // detectRange : 타겟 감지 거리

    [Header("공격 사거리")]
    public float attackRange = 6f;                                      // attackRange : 공격 사거리
    public float optimalDistanceRatio = 0.75f;                          // optimalDistanceRatio : 최적거리 비율 (공격 사거리의 일정 비율)
    public float distanceTolerance = 0.5f;                              // distanceTolerance : 오차 허용 범위
    public bool canRetreat = true;                                      // canRetreat : 후퇴 가능 여부

    public float OptimalDistance => attackRange * optimalDistanceRatio; // OptimalDistance : 최적 거리

    public void StatInitialize()
    {
        healthPoint = maxHealth;
    }

    public bool TakeDamage(float damage)                                // 데미지 계산 + 적 개체 사망 여부 판단
    {
        healthPoint -= damage;
        return healthPoint <= 0;
    }
}
