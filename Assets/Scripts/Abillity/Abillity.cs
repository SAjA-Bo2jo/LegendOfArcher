using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Abillity : MonoBehaviour
{
    protected Player player;                                    // 플레이어 참조
    protected AnimationHandler animationHandler;                // 애니메이션 핸들러 참조 (공격 시 활용 가능)
    protected GameObject target;                                  // 공격 대상
    protected float lastAttackTime = 0f;                          // 마지막 공격 시간 저장용
}
