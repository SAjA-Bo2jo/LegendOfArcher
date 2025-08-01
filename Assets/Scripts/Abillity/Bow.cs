using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 오브젝트 풀 매니저를 사용하기 위해 추가
// using ObjectPoolManager;

public class Bow : Abillity
{
    [SerializeField] private Animator bowAnimator;
    [SerializeField] private Transform weaponPivot;  // 회전 중심
    [SerializeField] private float radius = 40f;     // 중심에서 떨어진 거리
    private PlayerController playerController;
    [SerializeField] private Transform firePoint; // Bow 스크립트에 추가

    // 오브젝트 풀링을 위한 상수 키
    private const string ARROW_POOL_KEY = "Arrow";


    protected void Start()
    {
        // 부모 오브젝트에서 Player 컴포넌트 가져오기
        player = GetComponentInParent<Player>();

        // 자신에게 붙은 AnimationHandler 가져오기
        animationHandler = GetComponent<AnimationHandler>();

        // 이 줄을 추가하여 PlayerController 참조를 가져옵니다.
        // PlayerController가 Player와 같은 GameObject 또는 부모에 있다고 가정합니다.
        playerController = GetComponentInParent<PlayerController>();
    }

    private void LateUpdate()
    {
        // playerController가 null이 아니어야 합니다.
        if (player == null || weaponPivot == null || playerController == null) return;

        // 타겟이 존재할 때만 활의 위치와 회전을 업데이트합니다.
        if (target != null)
        {
            // 방향 벡터를 각도로 변환
            float angle = Mathf.Atan2(playerController.LookDirection.y, playerController.LookDirection.x);

            // 활의 위치 계산 (WeaponPivot 기준)
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            transform.position = weaponPivot.position + offset;

            // 활의 회전도 바라보는 방향으로 설정
            float angleDeg = angle * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angleDeg);
        }
        // 타겟이 없으면 활을 weaponPivot 위치에 유지합니다.
        else
        {
            transform.position = weaponPivot.position;
            transform.rotation = Quaternion.identity;
        }
    }

    protected void Update()
    {
        target = FindTarget();  // 매 프레임마다 대상 갱신
        TryAttack();            // 공격 시도
    }

    protected GameObject FindTarget()
    {
        // 플레이어 없으면 null 반환
        if (player == null) return null;

        // Enemy 태그를 가진 오브젝트들 중 조건에 맞는 가장 가까운 대상 찾기
        GameObject target = GameObject
            .FindGameObjectsWithTag("Enemy")
            .Where(enemy => enemy != null && enemy.activeInHierarchy && enemy.transform != null)
            .Where(enemy => Vector3.Distance(enemy.transform.position, transform.position) < player.AttackRange)
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position))
            .FirstOrDefault();
        return target;
    }

    protected float AttackDelay()
    {
        // 기본 공격 속도
        float attackSpeed = player.AttackSpeed;

        // 공격 속도 배율을 100 기준 비율로 변환
        float multiplier = player.AttackSpeedMultiplier / 100f;

        // 총 공격 속도 계산
        float totalAttackSpeed = attackSpeed * multiplier;

        // 딜레이 계산
        float delay = 1f / totalAttackSpeed;

        return delay;
    }

    protected void TryAttack()
    {
        if (target == null) return; // 타겟 없으면 공격 안 함

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();  // 플레이어의 Rigidbody2D를 통해 이동 중인지 확인
        if (rb != null && rb.velocity.magnitude > 0.01f)
            return; // 이동 중이면 공격 안 함

        float delay = AttackDelay();
        Debug.Log($"Calculated attack delay: {delay}");
        Debug.Log($"Current Time: {Time.time}, Last Attack Time: {lastAttackTime}");

        if (Time.time >= lastAttackTime + delay)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    protected void PerformAttack()
    {
        if (target == null) return;

        // 애니메이션 실행
        if (animationHandler != null)
        {
            animationHandler.Attack();
        }

        // 1. 오브젝트 풀에서 화살을 가져옵니다.
        var arrowObj = ObjectPoolManager.Instance.Get(ARROW_POOL_KEY);

        if (arrowObj == null)
        {
            Debug.LogError("Failed to get arrow from object pool.");
            return;
        }

        var arrow = arrowObj.GetComponent<Arrow>();
        if (arrow == null)
        {
            Debug.LogError("Arrow component not found on pooled object.");
            return;
        }

        // 2. 화살의 위치와 회전을 설정합니다.
        // 화살의 발사 방향을 활(Bow)이 현재 바라보는 방향으로 설정
        Vector3 finalLaunchDirection = transform.right;
        // 화살의 초기 회전을 계산
        float angle = Mathf.Atan2(finalLaunchDirection.y, finalLaunchDirection.x) * Mathf.Rad2Deg;
        Quaternion arrowInitialRotation = Quaternion.Euler(0, 0, angle);

        arrowObj.transform.position = firePoint.position;
        arrowObj.transform.rotation = arrowInitialRotation;

        // 3. 플레이어 스탯을 넘겨서 세팅
        arrow.Setup(
            damage: player.AttackDamage,
            size: player.AttackSize,
            critRate: player.CriticalRate,
            speed: player.AttackSpeed * (player.AttackSpeedMultiplier / 100f)
        );

        // 4. 생성된 화살에 최종 발사 방향을 전달하여 물리적인 힘을 가합니다.
        arrow.LaunchTowards(finalLaunchDirection); // 여기에는 '방향' 벡터를 넘겨야 합니다.
    }
}
