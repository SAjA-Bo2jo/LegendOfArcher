using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Abillity 클래스에 lastAttackTime 변수가 있을 것으로 가정합니다.
public class Bow : Abillity
{
    [SerializeField] private Transform weaponPivot;  // 활이 회전할 중심점 (플레이어의 특정 위치)
    [SerializeField] private float radius = 0.3f;    // weaponPivot에서 활이 떨어져 있는 거리
    private PlayerController playerController;       // 플레이어의 이동 및 바라보는 방향 정보를 가져오기 위함
    [SerializeField] private Transform firePoint;    // 화살이 실제로 생성되어 발사될 위치 (활 끝 부분)

    // 오브젝트 풀링을 위한 상수 키
    public const string ARROW_POOL_KEY = "Arrow"; // 다른 스크립트에서 참조할 수 있도록 public으로 변경

    [Header("활 공격 설정")]
    private bool isAttacking = false;            // 현재 공격(장전 포함) 중인지 나타내는 플래그

    // 현재 활에 매겨진(장전된) 화살 오브젝트 참조
    private GameObject loadedArrowGO;
    private Arrow loadedArrowScript;

    // Start 메서드는 오브젝트가 활성화될 때 한 번 호출됩니다.
    protected void Start()
    {
        // Player 클래스에서 BaseAbillity의 player 필드를 상속받아 사용
        player = GetComponentInParent<Player>();
        animationHandler = GetComponent<AnimationHandler>();
        playerController = GetComponentInParent<PlayerController>(); // PlayerController 참조

        if (firePoint == null)
        {
            Debug.LogError("Bow 스크립트에 Fire Point가 할당되지 않았습니다! 화살 발사 위치를 지정해주세요.");
        }
    }

    // LateUpdate는 모든 Update 함수가 호출된 후 매 프레임 호출됩니다.
    // 여기서는 활의 위치와 회전을 업데이트하여 부드러운 시각적 움직임을 제공합니다.
    private void LateUpdate()
    {
        if (player == null || weaponPivot == null || playerController == null) return;

        // 플레이어가 바라보는 방향으로 활 회전 (PlayerController의 LookDirection 사용)
        float angle = Mathf.Atan2(playerController.LookDirection.y, playerController.LookDirection.x);
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
        transform.position = weaponPivot.position + offset;
        float angleDeg = angle * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angleDeg);

        // 활이 회전할 때 장전된 화살도 함께 회전하도록 업데이트
        if (loadedArrowGO != null)
        {
            loadedArrowGO.transform.position = firePoint.position;
            loadedArrowGO.transform.rotation = transform.rotation;
        }
    }

    protected void Update()
    {
        // PlayerController의 FindTarget()을 사용하는 것이 일관성이 높습니다.
        // 하지만 Bow 자체 로직을 유지하고 싶다면 그대로 두어도 됩니다.
        // 여기서는 기존 Bow의 FindTarget()을 사용한다고 가정합니다.
        target = FindTarget();
        TryAttack();
    }

    // 시야 내 가장 가까운 적을 찾아 반환합니다.
    // (PlayerController의 FindTarget과 중복될 수 있으므로, 설계에 따라 하나만 사용 권장)
    protected GameObject FindTarget()
    {
        if (player == null) return null;

        GameObject target = GameObject
            .FindGameObjectsWithTag("Enemy")
            .Where(enemy => enemy != null && enemy.activeInHierarchy && enemy.transform != null)
            .Where(enemy => Vector3.Distance(enemy.transform.position, transform.position) < player.AttackRange)
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position))
            .FirstOrDefault();
        return target;
    }

    // 플레이어의 공격 속도를 기반으로 공격 지연 시간을 계산합니다.
    protected float AttackDelay()
    {
        // player.MaxAttackSpeed를 사용
        float totalAttackSpeed = player.MaxAttackSpeed;

        if (totalAttackSpeed <= 0.01f) totalAttackSpeed = 0.01f;

        float delay = 1f / totalAttackSpeed;
        return delay;
    }

    // 공격을 시도합니다.
    protected void TryAttack()
    {
        if (target == null) return;
        if (isAttacking) return;

        // 플레이어가 움직이는 중일 때는 공격하지 않음
        if (playerController != null && playerController.IsMoving)
        {
            return;
        }

        float delay = AttackDelay();

        if (Time.time >= lastAttackTime + delay)
        {
            StartCoroutine(PerformAttackWithDelay(delay));
            lastAttackTime = Time.time;
        }
    }

    // 화살 장전 시간을 포함하여 공격을 수행하는 코루틴입니다.
    private IEnumerator PerformAttackWithDelay(float totalAttackDelay)
    {
        isAttacking = true;

        float timeToWaitForArrowLoad = Mathf.Max(0f, totalAttackDelay - 0.2f); // 장전 시간 (딜레이의 일부)
        yield return new WaitForSeconds(timeToWaitForArrowLoad);

        // 오브젝트 풀에서 일반 화살을 가져옵니다.
        loadedArrowGO = ObjectPoolManager.Instance.Get(ARROW_POOL_KEY);

        if (loadedArrowGO == null)
        {
            Debug.LogError($"오브젝트 풀에서 '{ARROW_POOL_KEY}'를 가져오지 못했습니다. 풀에 등록되었는지 확인하세요.");
            isAttacking = false;
            yield break;
        }

        loadedArrowScript = loadedArrowGO.GetComponent<Arrow>();
        if (loadedArrowScript == null)
        {
            Debug.LogError("화살 Prefab에 Arrow 스크립트가 없습니다!");
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
            isAttacking = false;
            yield break;
        }

        // 화살의 Rigidbody를 초기화하고 비활성화
        Rigidbody2D arrowRb = loadedArrowScript.GetComponent<Rigidbody2D>();
        if (arrowRb != null)
        {
            arrowRb.isKinematic = true;
            arrowRb.simulated = false;
            arrowRb.velocity = Vector2.zero;
            arrowRb.angularVelocity = 0;
        }

        // 화살의 위치와 회전을 활의 firePoint에 맞춥니다.
        loadedArrowGO.transform.position = firePoint.position;
        loadedArrowGO.transform.rotation = transform.rotation;
        loadedArrowScript.transform.localScale = Vector3.one * player.AttackSize; // 플레이어 스탯에 따른 크기

        if (animationHandler != null)
        {
            animationHandler.Attack(); // 플레이어 공격 애니메이션 트리거
        }

        // 나머지 공격 딜레이 시간 대기
        yield return new WaitForSeconds(totalAttackDelay - timeToWaitForArrowLoad);

        FireLoadedArrow(); // 실제 화살 발사 로직 호출

        // 화살 관련 참조 초기화 (FireLoadedArrow 내부에서 이미 처리될 수 있음)
        loadedArrowGO = null;
        loadedArrowScript = null;

        isAttacking = false;
    }

    // 장전된 화살을 실제 발사하는 로직입니다.
    private void FireLoadedArrow()
    {
        if (target == null || loadedArrowScript == null || loadedArrowGO == null)
        {
            if (loadedArrowGO != null)
            {
                ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            }
            loadedArrowGO = null;
            loadedArrowScript = null;
            return;
        }

        // 플레이어에게 특수 화살 능력을 발동할 수 있는지 물어봅니다.
        // 플레이어에 FireArrowAbility와 같은 능력이 활성화되어 있다면, 이 메서드가 true를 반환하고 특수 화살을 발사할 것입니다.
        bool specialArrowFired = player.TryActivateSpecialArrowAbility(loadedArrowGO, loadedArrowScript);

        // 특수 화살이 발사되지 않았다면, 일반 화살을 발사합니다.
        if (!specialArrowFired)
        {
            Rigidbody2D arrowRb = loadedArrowScript.GetComponent<Rigidbody2D>();
            if (arrowRb != null)
            {
                arrowRb.isKinematic = false;
                arrowRb.simulated = true;
            }

            loadedArrowScript.Setup(
                damage: player.AttackDamage,
                size: player.AttackSize,
                critRate: player.CriticalRate,
                speed: player.ProjectileSpeed
            );

            Vector3 finalLaunchDirection = transform.right; // 활의 정면 방향으로 발사
            loadedArrowScript.LaunchTowards(finalLaunchDirection);
        }

        // loadedArrowGO와 loadedArrowScript는 다음 공격을 위해 초기화됩니다.
        // 특수 화살이 발사되었을 경우, loadedArrowGO는 이미 FireArrowAbility 내부에서 풀로 반환되었을 것입니다.
        loadedArrowGO = null;
        loadedArrowScript = null;
    }
}