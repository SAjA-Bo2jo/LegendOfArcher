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
    private bool isAttacking = false;

    private GameObject loadedArrowGO;
    private Arrow loadedArrowScript;

    // 애니메이션 속도 제어를 위해 Animator 컴포넌트 참조 추가
    private Animator animator;
    // 활의 스프라이트를 제어하기 위해 SpriteRenderer 컴포넌트 참조 추가
    private SpriteRenderer spriteRenderer;

    protected void Start()
    {
        player = GetComponentInParent<Player>();
        animationHandler = GetComponent<AnimationHandler>();
        playerController = GetComponentInParent<PlayerController>();

        // 스크린샷에서 확인된 Animator 컴포넌트를 가져옵니다.
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Bow 스크립트에 Animator 컴포넌트가 없습니다! Player > WeaponPivot > Bow에 Animator가 있는지 확인하세요.");
        }

        // SpriteRenderer 컴포넌트를 가져옵니다.
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
        target = FindTarget();
        TryAttack();

        // 타겟이 없으면 활을 숨기고, 있으면 보이게 합니다.
        if (target == null)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }
            if (animator != null)
            {
                // 공격 애니메이션을 멈춥니다.
                animator.SetBool("IsAttack", false);
            }
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
        if (isAttacking) return;
        if (playerController != null && playerController.IsMoving) return;

        // 타겟이 없으면 공격 시도 자체를 하지 않습니다.
        if (target == null) return;

        float delay = AttackDelay();
        if (Time.time >= lastAttackTime + delay)
        {
            StartCoroutine(PerformAttackCycle(delay));
            lastAttackTime = Time.time;
        }
    }

    private IEnumerator PerformAttackCycle(float totalAttackDelay)
    {
        isAttacking = true;

        if (player == null)
        {
            Debug.LogError("Bow: Player 컴포넌트를 찾을 수 없습니다.");
            isAttacking = false;
            yield break;
        }

        // 애니메이션 속도를 플레이어의 공격 속도에 맞춰 동적으로 조절합니다.
        if (animator != null)
        {
            animator.speed = player.MaxAttackSpeed;
        }

        if (animationHandler != null)
        {
            animationHandler.Attack();
        }
        else
        {
            Debug.LogWarning("Bow: AnimationHandler가 할당되지 않아 공격 애니메이션을 재생할 수 없습니다.");
            isAttacking = false;
            yield break;
        }

        // 화살을 미리 풀에서 가져와 장전 상태로 만듭니다.
        loadedArrowGO = ObjectPoolManager.Instance.Get(ARROW_POOL_KEY);
        if (loadedArrowGO == null)
        {
            Debug.LogError($"Bow: 오브젝트 풀에서 '{ARROW_POOL_KEY}'를 가져오지 못했습니다.");
            isAttacking = false;
            yield break;
        }

        loadedArrowScript = loadedArrowGO.GetComponent<Arrow>();
        if (loadedArrowScript == null)
        {
            Debug.LogError("Bow: 화살 Prefab에 Arrow 스크립트가 없습니다!");
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
            isAttacking = false;
            yield break;
        }

        Rigidbody2D arrowRb = loadedArrowScript.GetComponent<Rigidbody2D>();
        if (arrowRb != null)
        {
            arrowRb.isKinematic = true;
            arrowRb.simulated = false;
            arrowRb.velocity = Vector2.zero;
            arrowRb.angularVelocity = 0;
        }

        loadedArrowGO.transform.position = firePoint.position;
        loadedArrowGO.transform.rotation = transform.rotation;
        loadedArrowScript.transform.localScale = Vector3.one * player.AttackSize;
        loadedArrowGO.SetActive(true);

        // 총 공격 쿨타임 대기
        yield return new WaitForSeconds(totalAttackDelay);

        isAttacking = false;
    }

    /// <summary>
    /// Unity 애니메이션 이벤트에서 호출되는 화살 발사 메서드입니다.
    /// loadedArrowGO가 null일 경우, 즉시 화살을 생성하여 발사하는 안전장치 로직이 추가되었습니다.
    /// </summary>
    public void FireLoadedArrowFromAnimationEvent()
    {
        // 타겟이 없으면 발사하지 않습니다. 이중 확인 로직입니다.
        if (target == null)
        {
            if (loadedArrowGO != null)
            {
                ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            }
            loadedArrowGO = null;
            loadedArrowScript = null;
            return;
        }

        // 만약 loadedArrowGO가 준비되지 않았다면, 즉시 생성하여 발사하는 안전장치 로직
        if (loadedArrowGO == null)
        {

            loadedArrowGO = ObjectPoolManager.Instance.Get(ARROW_POOL_KEY);
            if (loadedArrowGO == null)
            {
                Debug.LogError($"Bow: 오브젝트 풀에서 '{ARROW_POOL_KEY}'를 가져오지 못했습니다.");
                return;
            }

            loadedArrowScript = loadedArrowGO.GetComponent<Arrow>();
            if (loadedArrowScript == null)
            {
                Debug.LogError("Bow: 화살 Prefab에 Arrow 스크립트가 없습니다!");
                ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
                loadedArrowGO = null;
                loadedArrowScript = null;
                return;
            }

            Rigidbody2D arrowRb = loadedArrowGO.GetComponent<Rigidbody2D>();
            if (arrowRb != null)
            {
                arrowRb.isKinematic = false;
                arrowRb.simulated = true;
            }

            loadedArrowGO.transform.position = firePoint.position;
            loadedArrowGO.transform.rotation = transform.rotation;
            loadedArrowScript.transform.localScale = Vector3.one * player.AttackSize;
            loadedArrowGO.SetActive(true);
        }

        Vector3 finalLaunchDirection = (target.transform.position - firePoint.position).normalized;
        Debug.Log($"Bow: 타겟 '{target.name}'를 찾았습니다. 발사 방향: {finalLaunchDirection}");

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

        // 발사 후 화살 참조를 초기화합니다.
        loadedArrowGO = null;
        loadedArrowScript = null;
    }

    public override void ApplyEffect() { }

    public override void RemoveEffect() { }
}
