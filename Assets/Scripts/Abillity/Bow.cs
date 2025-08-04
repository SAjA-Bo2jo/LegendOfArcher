using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bow : Abillity
{
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private float radius = 0.3f;
    private PlayerController playerController;
    [SerializeField] private Transform firePoint;

    public const string ARROW_POOL_KEY = "Arrow";
    public const string FIRE_ARROW_POOL_KEY = "FireArrow";

    [Header("활 공격 설정")]
    private bool isAttack = false; // 현재 공격 애니메이션이 진행 중인지 나타냅니다.

    private GameObject loadedArrowGO; // 현재 활에 장전된 화살 GameObject
    private Arrow loadedArrowScript; // 현재 활에 장전된 화살 스크립트

    // 애니메이션 속도 제어를 위해 Animator 컴포넌트 참조
    private Animator animator;
    // 활의 스프라이트를 제어하기 위해 SpriteRenderer 컴포넌트 참조
    private SpriteRenderer spriteRenderer;

    protected void Start()
    {
        // 부모 오브젝트 (Player)에서 Player와 PlayerController 컴포넌트를 가져옵니다.
        player = GetComponentInParent<Player>();
        playerController = GetComponentInParent<PlayerController>();

        // 활 오브젝트 자체에서 AnimationHandler 컴포넌트를 가져와 할당합니다.
        // Bow는 Abillity를 상속받았고 Abillity 내부에 animationHandler 필드가 있으므로,
        // 이 필드를 활용하여 Bow의 AnimationHandler를 참조합니다.
        animationHandler = GetComponent<AnimationHandler>();
        if (animationHandler == null)
        {
            Debug.LogError("Bow 스크립트에 AnimationHandler 컴포넌트가 없습니다! Bow 오브젝트에 있는지 확인하세요.");
        }

        // 스크린샷에서 확인된 Animator 컴포넌트를 가져옵니다.
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Bow 스크립트에 Animator 컴포넌트가 없습니다! Player > WeaponPivot > Bow GameObject에 Animator가 있는지 확인하세요.");
        }

        // 활의 스프라이트를 제어하기 위해 SpriteRenderer 컴포넌트를 가져옵니다.
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Bow 스크립트에 SpriteRenderer 컴포넌트가 없습니다!");
        }

        // 화살이 발사될 위치를 확인합니다.
        if (firePoint == null)
        {
            Debug.LogError("Bow 스크립트에 Fire Point가 할당되지 않았습니다! 화살 발사 위치를 지정해주세요.");
        }
    }

    // 오브젝트의 위치와 회전을 업데이트합니다. (주로 PlayerController의 LookDirection에 따라)
    private void LateUpdate()
    {
        if (player == null || weaponPivot == null || playerController == null) return;

        // 플레이어의 시선 방향에 따라 활의 위치를 원형으로 배치합니다.
        float angle = Mathf.Atan2(playerController.LookDirection.y, playerController.LookDirection.x);
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
        transform.position = weaponPivot.position + offset;

        // 활의 회전을 플레이어 시선 방향에 맞춥니다.
        float angleDeg = angle * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angleDeg);

        // 애니메이션이 재생되는 동안 장전된 화살을 활 끝에 고정합니다.
        if (loadedArrowGO != null)
        {
            loadedArrowGO.transform.position = firePoint.position;
            loadedArrowGO.transform.rotation = transform.rotation;
        }
    }

    // 매 프레임마다 공격 로직을 업데이트합니다.
    protected void Update()
    {
        // 매 프레임 타겟을 찾습니다.
        target = FindTarget();

        // 타겟이 없거나 플레이어가 움직일 경우 공격을 중단하고 활 스프라이트를 비활성화합니다.
        if (target == null || (playerController != null && playerController.IsMoving))
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false; // 활 스프라이트 비활성화
            }
            StopAttack(); // 공격 사이클 중지 및 장전된 화살 반환, 애니메이션 리셋
        }
        else // 타겟이 있고 플레이어가 움직이지 않을 경우 공격을 시도하고 활 스프라이트를 활성화합니다.
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true; // 활 스프라이트 활성화
            }
            TryAttack(); // 공격 시도
        }
    }

    // 가장 가까운 적을 타겟으로 설정합니다.
    protected GameObject FindTarget()
    {
        if (player == null) return null; // Player 컴포넌트가 없으면 타겟 탐색 불가

        GameObject target = GameObject
            .FindGameObjectsWithTag("Enemy")
            .Where(enemy => enemy != null && enemy.activeInHierarchy && enemy.transform != null) // null 체크 및 활성 상태 체크
            .Where(enemy => Vector3.Distance(enemy.transform.position, transform.position) < player.AttackRange) // 공격 범위 내 적 필터링
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position)) // 거리순 정렬
            .FirstOrDefault(); // 가장 가까운 적 선택
        return target;
    }

    // 공격 딜레이 시간을 계산합니다.
    protected float AttackDelay()
    {
        float totalAttackSpeed = player.MaxAttackSpeed;

        if (totalAttackSpeed <= 0.01f) totalAttackSpeed = 0.01f; // 0으로 나누는 것을 방지

        float delay = 1f / totalAttackSpeed; // 초당 공격 횟수를 딜레이로 변환
        return delay;
    }

    // 공격을 시도합니다.
    protected void TryAttack()
    {
        if (isAttack) return; // 이미 공격 중이면 스킵 (중복 공격 방지)

        // 플레이어가 움직이거나 타겟이 없으면 공격 시도하지 않음
        if ((playerController != null && playerController.IsMoving) || target == null)
        {
            // Debug.Log("Bow: 플레이어가 움직이거나 타겟이 없어 공격을 시도하지 않습니다."); // 너무 자주 출력될 수 있어 주석 처리
            return;
        }

        // 다음 공격까지의 딜레이가 지났는지 확인
        float delay = AttackDelay();
        if (Time.time >= lastAttackTime + delay)
        {
            StartCoroutine(PerformAttackCycle()); // 공격 사이클 코루틴 시작
            lastAttackTime = Time.time; // 마지막 공격 시간 업데이트
        }
    }

    // 공격 애니메이션 및 발사 과정을 관리하는 코루틴
    private IEnumerator PerformAttackCycle()
    {
        isAttack = true; // 공격 시작 상태로 설정

        // 코루틴 시작 직전에 플레이어 이동 또는 타겟 상실 여부 다시 확인
        if ((playerController != null && playerController.IsMoving) || target == null)
        {
            Debug.Log("Bow: PerformAttackCycle 시작 직전에 조건 미달. 공격을 취소합니다.");
            StopAttack();
            yield break; // 코루틴 즉시 종료
        }

        if (player == null)
        {
            Debug.LogError("Bow: Player 컴포넌트를 찾을 수 없습니다. 공격을 중단합니다.");
            StopAttack();
            yield break;
        }

        // 활 공격 애니메이션 재생 요청
        if (animationHandler != null)
        {
            animationHandler.Attack();
        }
        else
        {
            Debug.LogWarning("Bow: AnimationHandler가 할당되지 않아 공격 애니메이션을 재생할 수 없습니다.");
            StopAttack();
            yield break;
        }

        // 애니메이션 시작과 동시에 화살을 미리 장전합니다.
        LoadArrow();

        // 코루틴은 여기서 대기하지 않고, 애니메이션 이벤트에 의해
        // FireLoadedArrowFromAnimationEvent()가 호출되기를 기다립니다.
        // FireLoadedArrowFromAnimationEvent()에서 isAttack을 false로 설정하고
        // loadedArrowGO를 null로 만들어 공격 사이클을 완료시킬 것입니다.
        // 따라서 이 코루틴은 FireLoadedArrowFromAnimationEvent() 호출 전까지는
        // 다른 작업을 하지 않고 단순히 상태를 설정하고 화살을 장전하는 역할만 합니다.
        // 필요한 경우, 애니메이션이 너무 길어져 이벤트가 호출되지 않는 상황을 대비한
        // 타임아웃 로직 (yield return new WaitForSeconds(maxAnimationDuration);)을 추가할 수 있습니다.
    }

    // 화살을 오브젝트 풀에서 가져와 활에 장전하는 함수
    private void LoadArrow()
    {
        // 이전에 장전된 화살이 있다면 풀로 반환하고 초기화합니다.
        if (loadedArrowGO != null)
        {
            Debug.LogWarning("Bow: 새로운 화살을 장전하기 전에 이전에 장전된 화살을 풀로 반환합니다.");
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
        }

        // 오브젝트 풀에서 일반 화살을 가져옵니다.
        loadedArrowGO = ObjectPoolManager.Instance.Get(ARROW_POOL_KEY);
        if (loadedArrowGO == null)
        {
            Debug.LogError($"Bow: 오브젝트 풀에서 '{ARROW_POOL_KEY}'를 가져오지 못했습니다. 화살 장전 실패.");
            StopAttack(); // 실패 시 공격 사이클 중단
            return;
        }

        // 화살 스크립트 컴포넌트를 가져옵니다.
        loadedArrowScript = loadedArrowGO.GetComponent<Arrow>();
        if (loadedArrowScript == null)
        {
            Debug.LogError("Bow: 화살 Prefab에 Arrow 스크립트가 없습니다! 화살 장전 실패.");
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO); // 가져온 화살 반환
            loadedArrowGO = null;
            loadedArrowScript = null;
            StopAttack(); // 실패 시 공격 사이클 중단
            return;
        }

        // 장전된 화살의 물리 설정을 비활성화하여 활에 고정되도록 합니다.
        Rigidbody2D arrowRb = loadedArrowScript.GetComponent<Rigidbody2D>();
        if (arrowRb != null)
        {
            arrowRb.isKinematic = true; // 물리 영향 받지 않도록
            arrowRb.simulated = false; // 물리 시뮬레이션 비활성화
            arrowRb.velocity = Vector2.zero;
            arrowRb.angularVelocity = 0;
        }

        // 화살의 위치와 회전을 활 끝에 맞추고 스케일을 조정합니다.
        loadedArrowGO.transform.position = firePoint.position;
        loadedArrowGO.transform.rotation = transform.rotation;
        loadedArrowScript.transform.localScale = Vector3.one * player.AttackSize;
        loadedArrowGO.SetActive(true); // 화살 활성화
        Debug.Log("Bow: 화살 장전 완료!");
    }

    // 현재 진행 중인 공격을 중단하고 모든 관련 상태를 초기화합니다.
    public void StopAttack()
    {
        StopAllCoroutines(); // 활 관련 모든 코루틴 중지
        isAttack = false; // 공격 중 아님으로 설정

        // Animator의 공격 상태를 리셋합니다.
        if (animator != null)
        {
            animator.SetBool("IsAttack", false);
        }

        // 장전된 화살이 있다면 오브젝트 풀로 반환합니다.
        if (loadedArrowGO != null)
        {
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
            Debug.Log("Bow: 장전된 화살을 오브젝트 풀로 반환했습니다.");
        }
    }

    // 애니메이션 이벤트에 의해 호출되는 화살 발사 메서드
    public void FireLoadedArrowFromAnimationEvent()
    {
        // 발사 직전에 다시 한 번 플레이어 상태와 타겟 여부를 확인합니다.
        if ((playerController != null && playerController.IsMoving) || target == null)
        {
            Debug.Log("Bow: 발사 직전 플레이어 이동 또는 타겟 상실 감지. 화살 발사를 취소하고 공격을 중단합니다.");
            StopAttack();
            return;
        }

        // 장전된 화살이 없으면 경고하고 공격을 중단합니다. (LoadArrow가 제대로 작동하지 않았거나, 이미 발사된 경우)
        if (loadedArrowGO == null)
        {
            Debug.LogWarning("Bow: 장전된 화살이 없어 발사할 수 없습니다. 애니메이션 이벤트 호출 시점에 loadedArrowGO가 null입니다.");
            StopAttack();
            return;
        }

        Debug.Log("Bow: 애니메이션 이벤트에 의해 화살 발사!");
        // 타겟 방향으로 화살을 발사합니다.
        Vector3 finalLaunchDirection = (target.transform.position - firePoint.position).normalized;

        // 특수 화살 능력이 활성화될 수 있는지 시도합니다.
        bool specialArrowFired = player.TryActivateSpecialArrowAbility(loadedArrowGO, loadedArrowScript);

        // 특수 화살이 발사되지 않았다면 일반 화살을 발사합니다.
        if (!specialArrowFired)
        {
            Rigidbody2D arrowRb = loadedArrowScript.GetComponent<Rigidbody2D>();
            if (arrowRb != null)
            {
                arrowRb.isKinematic = false; // 물리 시뮬레이션 다시 활성화
                arrowRb.simulated = true;
            }

            // 화살의 스탯을 플레이어의 현재 스탯에 맞춰 설정합니다.
            loadedArrowScript.Setup(
                damage: player.AttackDamage,
                size: player.AttackSize,
                critRate: player.CriticalRate,
                speed: player.ProjectileSpeed
            );
            loadedArrowScript.LaunchTowards(finalLaunchDirection); // 화살 발사
        }

        // 화살 발사 후 장전된 화살 참조를 초기화하고 공격 상태를 해제합니다.
        loadedArrowGO = null;
        loadedArrowScript = null;
        isAttack = false; // 공격 사이클 완료

        // Animator의 공격 상태를 리셋합니다.
        if (animator != null)
        {
            animator.SetBool("IsAttack", false);
        }
    }

    // Abillity 클래스에서 상속받은 추상 메서드 구현 (필요에 따라 로직 추가)
    public override void ApplyEffect() { }
    public override void RemoveEffect() { }
}