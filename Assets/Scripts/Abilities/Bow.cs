using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Bow : Ability
{
    // protected로 변경된 필드들 (자식 클래스나 관련 시스템에서 접근 가능하도록)
    [SerializeField] protected Transform weaponPivot;
    [SerializeField] protected float radius = 0.3f;
    protected PlayerController playerController;
    [SerializeField] protected Transform firePoint; // 화살이 발사될 정확한 위치

    // 현재 공격 애니메이션 진행 중 여부
    protected bool isAttack = false;

    protected GameObject loadedArrowGO; // 현재 활에 장전된 화살 GameObject
    protected Arrow loadedArrowScript; // 현재 활에 장전된 화살 스크립트

    protected Animator animator; // 활의 Animator 컴포넌트
    protected SpriteRenderer spriteRenderer; // 활의 SpriteRenderer 컴포넌트

    // 오브젝트 풀 키는 상수이므로 public const로 유지
    public const string ARROW_POOL_KEY = "Arrow";

    // --- 수정된 부분: 애니메이터 파라미터 해시를 Trigger로 명확하게 정의 ---
    private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");

    protected virtual void Start()
    {
        // 부모 오브젝트 (Player)에서 Player와 PlayerController 컴포넌트를 가져옵니다.
        player = GetComponentInParent<Player>();
        playerController = GetComponentInParent<PlayerController>();

        // Bow 오브젝트 자체에서 Animator 컴포넌트를 가져와 할당합니다.
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError($"{this.GetType().Name}: Animator 컴포넌트가 없습니다! {this.gameObject.name}에 Animator가 있는지 확인하세요.");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"{this.GetType().Name}: SpriteRenderer 컴포넌트가 없습니다! {this.gameObject.name}에 있는지 확인하세요.");
        }

        if (firePoint == null)
        {
            Debug.LogError($"{this.GetType().Name}: Fire Point가 할당되지 않았습니다! {this.gameObject.name}에서 화살 발사 위치를 지정해주세요.");
        }

        abilityName = "기본 활";
        description = "플레이어의 기본 원거리 무기입니다.";
        MaxLevel = 1;
        CurrentLevel = 1;
    }

    protected void LateUpdate()
    {
        if (player == null || weaponPivot == null || playerController == null) return;

        float angle = Mathf.Atan2(playerController.LookDirection.y, playerController.LookDirection.x);
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
        transform.position = weaponPivot.position + offset;

        float angleDeg = angle * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angleDeg);

        if (loadedArrowGO != null)
        {
            loadedArrowGO.transform.position = firePoint.position;
            loadedArrowGO.transform.rotation = transform.rotation;
        }
    }

    protected void Update()
    {
        target = FindTarget();

        if (target == null || (playerController != null && playerController.IsMoving))
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }
            if (isAttack) StopAttack();
        }
        else
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }
            TryAttack();
        }
    }

    protected new GameObject FindTarget()
    {
        if (player == null) return null;

        GameObject foundTarget = GameObject
            .FindGameObjectsWithTag("Enemy")
            .Where(enemy => enemy != null && enemy.activeInHierarchy && enemy.transform != null)
            .Where(enemy => Vector3.Distance(enemy.transform.position, player.transform.position) < player.AttackRange)
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, player.transform.position))
            .FirstOrDefault();
        return foundTarget;
    }

    protected float AttackDelay()
    {
        if (player == null) return 1f;

        float totalAttackSpeed = player.MaxAttackSpeed;
        if (totalAttackSpeed <= 0.01f) totalAttackSpeed = 0.01f;

        float delay = 1f / totalAttackSpeed;
        return delay;
    }

    protected virtual void TryAttack()
    {
        if (isAttack) return;

        if ((playerController != null && playerController.IsMoving) || target == null)
        {
            return;
        }

        float delay = AttackDelay();
        if (Time.time >= lastAttackTime + delay)
        {
            lastAttackTime = Time.time;
            StartCoroutine(PerformAttackCycle());
        }
    }

    protected IEnumerator PerformAttackCycle()
    {
        isAttack = true;

        if ((playerController != null && playerController.IsMoving) || target == null || player == null)
        {
            Debug.Log($"{this.GetType().Name}: PerformAttackCycle 시작 직전에 조건 미달. 공격을 취소합니다.");
            StopAttack();
            yield break;
        }

        LoadArrow();

        // --- 수정된 부분: Trigger 파라미터를 사용하여 애니메이션 재생 ---
        if (animator != null)
        {
            animator.SetTrigger(AttackTriggerHash);
        }
        else
        {
            Debug.LogWarning($"{this.GetType().Name}: Animator가 할당되지 않아 공격 애니메이션을 재생할 수 없습니다.");
            StopAttack();
            yield break;
        }
        // 이 코루틴은 애니메이션 이벤트가 호출될 때까지 대기하지 않습니다.
    }

    protected void LoadArrow()
    {
        if (loadedArrowGO != null)
        {
            Debug.LogWarning($"{this.GetType().Name}: 새로운 화살을 장전하기 전에 이전에 장전된 화살을 풀로 반환합니다.");
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
        }

        loadedArrowGO = ObjectPoolManager.Instance.Get(ARROW_POOL_KEY);
        if (loadedArrowGO == null)
        {
            Debug.LogError($"{this.GetType().Name}: 오브젝트 풀에서 '{ARROW_POOL_KEY}'를 가져오지 못했습니다. 화살 장전 실패.");
            StopAttack();
            return;
        }

        loadedArrowScript = loadedArrowGO.GetComponent<Arrow>();
        if (loadedArrowScript == null)
        {
            Debug.LogError($"{this.GetType().Name}: 화살 Prefab에 Arrow 스크립트가 없습니다! 화살 장전 실패.");
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
            StopAttack();
            return;
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
    }

    public void StopAttack()
    {
        StopAllCoroutines();
        isAttack = false;

        if (loadedArrowGO != null)
        {
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
            Debug.Log($"{this.GetType().Name}: 장전된 화살을 오브젝트 풀로 반환했습니다.");
        }
    }

    // --- 수정된 부분: 애니메이션 이벤트에 의해 호출되는 화살 발사 메서드 ---
    public void FireLoadedArrowFromAnimationEvent()
    {
        if ((playerController != null && playerController.IsMoving) || target == null)
        {
            StopAttack();
            return;
        }

        if (loadedArrowGO == null)
        {
            Debug.LogWarning($"{this.GetType().Name}: FireLoadedArrowFromAnimationEvent 호출되었지만 장전된 화살이 없습니다.");
            StopAttack();
            return;
        }

        bool specialArrowFired = player.TryActivateSpecialArrowAbility(loadedArrowGO, loadedArrowScript);

        if (!specialArrowFired)
        {
            Vector3 finalLaunchDirection = (target.transform.position - firePoint.position).normalized;

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

        loadedArrowGO = null;
        loadedArrowScript = null;
        isAttack = false;
    }

    public override void ApplyEffect()
    {
        // Bow는 "무기"로서 기본 스탯 변경은 하지 않는다고 가정합니다.
    }
}