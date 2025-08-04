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
    private bool isAttack = false;

    private GameObject loadedArrowGO;
    private Arrow loadedArrowScript;

    // 애니메이션 속도 제어를 위해 Animator 컴포넌트 참조 추가
    private Animator animator;
    // 활의 스프라이트를 제어하기 위해 SpriteRenderer 컴포넌트 참조 추가
    private SpriteRenderer spriteRenderer;

    protected void Start()
    {
        // 부모 오브젝트에서 Player와 PlayerController 컴포넌트를 가져옵니다.
        player = GetComponentInParent<Player>();
        playerController = GetComponentInParent<PlayerController>();

<<<<<<< Updated upstream
        // 활 오브젝트에서 AnimationHandler 컴포넌트를 가져와 할당합니다.
        // 이전에 부모(Player)에서 가져오던 것을 변경합니다.
=======
>>>>>>> Stashed changes
        animationHandler = GetComponent<AnimationHandler>();
        if (animationHandler == null)
        {
            Debug.LogError("Bow 스크립트에 AnimationHandler 컴포넌트가 없습니다! Bow 오브젝트에 있는지 확인하세요.");
        }

<<<<<<< Updated upstream
        // 스크린샷에서 확인된 Animator 컴포넌트를 가져옵니다.
=======
>>>>>>> Stashed changes
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Bow 스크립트에 Animator 컴포넌트가 없습니다! Player > WeaponPivot > Bow에 Animator가 있는지 확인하세요.");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Bow 스크립트에 SpriteRenderer 컴포넌트가 없습니다!");
        }

        if (firePoint == null)
        {
            Debug.LogError("Bow 스크립트에 Fire Point가 할당되지 않았습니다! 화살 발사 위치를 지정해주세요.");
        }
    }

    private void LateUpdate()
    {
        if (player == null || weaponPivot == null || playerController == null) return;

        float angle = Mathf.Atan2(playerController.LookDirection.y, playerController.LookDirection.x);
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
        transform.position = weaponPivot.position + offset;
        float angleDeg = angle * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angleDeg);

        // 애니메이션이 재생되는 동안 장전된 화살을 활 끝에 고정합니다.
        if (loadedArrowGO != null)
        {
            loadedArrowGO.transform.position = firePoint.position;
            loadedArrowGO.transform.rotation = transform.rotation;
        }
    }

    protected void Update()
    {
        // 매 프레임 타겟을 찾고, 공격을 시도합니다.
        target = FindTarget();
        TryAttack();

        // --- 핵심 수정 부분: 타겟이 없거나 플레이어가 움직일 경우 공격 중단 및 화살 반환 ---
        if (target == null || playerController.IsMoving)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }
<<<<<<< Updated upstream
            if (animator != null)
            {
                // 공격 애니메이션을 멈춥니다.
                animator.SetBool("IsAttack", false);
            }
            StopAttack(); // 타겟이 없으면 공격 사이클도 중지
=======
            StopAttack(); // 공격 중단 및 장전된 화살 반환
>>>>>>> Stashed changes
        }
        else
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }
        }
    }

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

    protected float AttackDelay()
    {
        float totalAttackSpeed = player.MaxAttackSpeed;

        if (totalAttackSpeed <= 0.01f) totalAttackSpeed = 0.01f;

        float delay = 1f / totalAttackSpeed;
        return delay;
    }

    protected void TryAttack()
    {
<<<<<<< Updated upstream
        if (isAttack) return; // 이미 공격 중이면 스킵

        if (playerController != null && playerController.IsMoving)
        {
            Debug.Log("Bow: 플레이어가 움직이고 있어 공격을 시도하지 않습니다.");
=======
        if (isAttack) return;

        // 플레이어 움직임 체크를 AttackDelay 안으로 옮기지 않고, 여기서 미리 체크합니다.
        if (playerController != null && playerController.IsMoving)
        {
            // Debug.Log("Bow: 플레이어가 움직이고 있어 공격을 시도하지 않습니다."); // 너무 자주 출력될 수 있어 주석 처리
>>>>>>> Stashed changes
            return;
        }

        if (target == null)
        {
<<<<<<< Updated upstream
            Debug.Log("Bow: 타겟이 없어 공격을 시도하지 않습니다.");
=======
            // Debug.Log("Bow: 타겟이 없어 공격을 시도하지 않습니다."); // 너무 자주 출력될 수 있어 주석 처리
>>>>>>> Stashed changes
            return;
        }

        float delay = AttackDelay();
        if (Time.time >= lastAttackTime + delay)
        {
<<<<<<< Updated upstream
            Debug.Log("Bow: 공격 시작!");
            StartCoroutine(PerformAttackCycle()); // 딜레이는 코루틴 내에서 처리
=======
            StartCoroutine(PerformAttackCycle());
>>>>>>> Stashed changes
            lastAttackTime = Time.time;
        }
    }

    private IEnumerator PerformAttackCycle()
    {
        isAttack = true;

        if (playerController != null && playerController.IsMoving)
        {
            Debug.Log("Bow: PerformAttackCycle 시작 직전에 플레이어 이동 감지. 공격을 취소합니다.");
            StopAttack();
            yield break;
        }

        if (player == null)
        {
            Debug.LogError("Bow: Player 컴포넌트를 찾을 수 없습니다.");
            StopAttack();
            yield break;
        }

        if (animationHandler != null)
        {
            animationHandler.Attack(); // 활 공격 애니메이션 재생
        }
        else
        {
            Debug.LogWarning("Bow: AnimationHandler가 할당되지 않아 공격 애니메이션을 재생할 수 없습니다.");
            StopAttack();
            yield break;
        }

<<<<<<< Updated upstream
        // 애니메이션이 시작되면 바로 화살을 장전
        LoadArrow();

        // 애니메이션 이벤트에 의해 화살이 발사될 때까지 대기
        // isAttack은 Animation Event에서 FireLoadedArrowFromAnimationEvent 호출 후 false로 설정
        // 또는 공격 애니메이션이 끝났는데도 FireLoadedArrowFromAnimationEvent가 호출되지 않았다면 (예: 애니메이션이 중단된 경우)
        // 안전 장치로 일정 시간 후 isAttack을 false로 되돌립니다.
        // 여기서는 애니메이션 이벤트에서 isAttack을 false로 설정하는 것을 기대합니다.
        // 만약 애니메이션 이벤트가 FireLoadedArrowFromAnimationEvent를 호출하지 않는 경우를 대비한 타임아웃 로직이 필요하다면 추가할 수 있습니다.
        // 예를 들어, 최대 공격 딜레이 시간만큼 기다린 후에도 isAttack이 true라면 강제 종료
        // yield return new WaitForSeconds(totalAttackDelay * 2); // 예시: 최대 공격 딜레이의 두 배 시간 후에도 공격 중이라면
        // if (isAttack) { StopAttack(); }

        // 장전된 화살이 발사되거나 공격이 중단될 때까지 기다립니다.
        // Animator의 "IsAttack" 상태가 false가 되거나, loadedArrowGO가 null이 될 때까지 기다릴 수 있습니다.
        // 여기서는 FireLoadedArrowFromAnimationEvent()에서 isAttack을 false로 설정하고 loadedArrowGO를 null로 만들기 때문에
        // 별도의 긴 대기 시간 없이 애니메이션 이벤트의 호출을 기다리면 됩니다.
=======
        LoadArrow(); // 애니메이션 시작과 동시에 화살 장전
>>>>>>> Stashed changes
    }

    // 화살을 오브젝트 풀에서 가져와 활에 장전하는 함수
    private void LoadArrow()
    {
<<<<<<< Updated upstream
        if (loadedArrowGO != null)
        {
            // 이미 장전된 화살이 있다면 반환하고 새로 장전
=======
        // --- 핵심 수정 부분: 이미 장전된 화살이 있다면 먼저 풀로 반환 ---
        if (loadedArrowGO != null)
        {
            Debug.LogWarning("Bow: 새로운 화살을 장전하기 전에 이전에 장전된 화살을 반환합니다.");
>>>>>>> Stashed changes
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
        }

        loadedArrowGO = ObjectPoolManager.Instance.Get(ARROW_POOL_KEY);
        if (loadedArrowGO == null)
        {
            Debug.LogError($"Bow: 오브젝트 풀에서 '{ARROW_POOL_KEY}'를 가져오지 못했습니다. 화살 장전 실패.");
            StopAttack();
            return;
        }

        loadedArrowScript = loadedArrowGO.GetComponent<Arrow>();
        if (loadedArrowScript == null)
        {
            Debug.LogError("Bow: 화살 Prefab에 Arrow 스크립트가 없습니다! 화살 장전 실패.");
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
            StopAttack();
            return;
        }

        Rigidbody2D arrowRb = loadedArrowScript.GetComponent<Rigidbody2D>();
        if (arrowRb != null)
        {
            arrowRb.isKinematic = true; // 물리 영향 받지 않도록
            arrowRb.simulated = false; // 시뮬레이션 비활성화
            arrowRb.velocity = Vector2.zero;
            arrowRb.angularVelocity = 0;
        }
        loadedArrowGO.transform.position = firePoint.position;
        loadedArrowGO.transform.rotation = transform.rotation;
        loadedArrowScript.transform.localScale = Vector3.one * player.AttackSize;
        loadedArrowGO.SetActive(true);
<<<<<<< Updated upstream
        Debug.Log("Bow: 화살 장전 완료!");
    }


    public void StopAttack()
    {
        Debug.Log("Bow: StopAttack() 호출. 공격 상태를 초기화합니다.");
        StopAllCoroutines();
=======
    }

    public void StopAttack()
    {
        StopAllCoroutines(); // 모든 활 관련 코루틴 중지
>>>>>>> Stashed changes
        isAttack = false;

        if (animator != null)
        {
<<<<<<< Updated upstream
            animator.SetBool("IsAttack", false);
        }

=======
            animator.SetBool("IsAttack", false); // 공격 애니메이션 중지
        }

        // --- 핵심 수정 부분: 장전된 화살이 있다면 반드시 풀로 반환 ---
>>>>>>> Stashed changes
        if (loadedArrowGO != null)
        {
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
<<<<<<< Updated upstream
=======
            Debug.Log("Bow: 장전된 화살을 오브젝트 풀로 반환했습니다.");
>>>>>>> Stashed changes
        }
    }

    public void FireLoadedArrowFromAnimationEvent()
    {
        if (playerController != null && playerController.IsMoving)
        {
            Debug.Log("Bow: 플레이어가 움직여서 화살 발사를 취소하고 공격을 중지합니다.");
            StopAttack();
<<<<<<< Updated upstream
            return;
        }

        if (target == null)
        {
            Debug.Log("Bow: 타겟이 없어 화살 발사를 취소하고 공격을 중지합니다.");
            StopAttack(); // 타겟이 없으면 공격 사이클 중지
=======
            return;
        }

        if (target == null)
        {
            Debug.Log("Bow: 타겟이 없어 화살 발사를 취소하고 공격을 중지합니다.");
            StopAttack();
            return;
        }

        if (loadedArrowGO == null)
        {
            Debug.LogWarning("Bow: 장전된 화살이 없어 발사할 수 없습니다. 애니메이션 이벤트 호출 시점에 loadedArrowGO가 null입니다.");
            StopAttack();
>>>>>>> Stashed changes
            return;
        }

        // 장전된 화살이 없으면 발사하지 않고, 공격을 중단합니다.
        // 이제 PerformAttackCycle에서 화살을 미리 장전하므로, 이 시점에서 loadedArrowGO가 null이면 문제가 있는 것입니다.
        if (loadedArrowGO == null)
        {
            Debug.LogWarning("Bow: 장전된 화살이 없어 발사할 수 없습니다. 애니메이션 이벤트 호출 시점에 loadedArrowGO가 null입니다.");
            StopAttack();
            return;
        }

        Debug.Log("Bow: 화살 발사!");
        Vector3 finalLaunchDirection = (target.transform.position - firePoint.position).normalized;

        bool specialArrowFired = player.TryActivateSpecialArrowAbility(loadedArrowGO, loadedArrowScript);

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
            loadedArrowScript.LaunchTowards(finalLaunchDirection);
        }

<<<<<<< Updated upstream
        // 화살 발사 후 loadedArrowGO와 loadedArrowScript를 초기화하고 공격 상태 해제
        loadedArrowGO = null;
        loadedArrowScript = null;
        isAttack = false; // 공격 사이클 완료
=======
        loadedArrowGO = null;
        loadedArrowScript = null;
        isAttack = false;
>>>>>>> Stashed changes
        if (animator != null)
        {
            animator.SetBool("IsAttack", false);
        }
    }

    public override void ApplyEffect() { }

    public override void RemoveEffect() { }
}