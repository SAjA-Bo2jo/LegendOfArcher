using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAttack : IEnemyAttack
{
    private float lastAttackTime = 0f;                                  // 쿨타임 변수

    public bool CanAttack(EnemyController controller)
    {
        return Time.time > lastAttackTime + 1f;                         // 쿨타임보다 시간이 더 지나야 공격 가능
    }

    public void Attack(EnemyController controller)
    {
        lastAttackTime = Time.time;                                     // 공격 시간 기록

        // 발사체 생성

        // 플레이어 방향으로 발사
    }
}
