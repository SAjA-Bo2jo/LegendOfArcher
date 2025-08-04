using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Bow : Ability
{
    // protected로 변경된 필드들 (GatlingBow 같은 자식 클래스나 관련 시스템에서 접근 가능하도록)
    [SerializeField] protected Transform weaponPivot;
    [SerializeField] protected float radius = 0.3f;
    protected PlayerController playerController;
    [SerializeField] protected Transform firePoint; // 화살이 발사될 정확한 위치

    protected bool isAttack = false; // 현재 공격 애니메이션 진행 중 여부

    protected GameObject loadedArrowGO; // 현재 활에 장전된 화살 GameObject
    protected Arrow loadedArrowScript; // 현재 활에 장전된 화살 스크립트

    protected Animator animator; // 활의 Animator 컴포넌트
    protected SpriteRenderer spriteRenderer; // 활의 SpriteRenderer 컴포넌트

    // 오브젝트 풀 키는 상수이므로 public const로 유지
    public const string ARROW_POOL_KEY = "Arrow";
    // FireArrow는 다른 능력에서 사용하므로 Bow에서는 필요 없습니다.

    // Start 메서드를 protected virtual로 변경하여 자식 클래스에서 오버라이드 가능하게 함
    protected virtual void Start()
    {
        // 부모 오브젝트 (Player)에서 Player와 PlayerController 컴포넌트를 가져옵니다.
        // Bow는 플레이어의 자식으로 붙어있다고 가정합니다.
        player = GetComponentInParent<Player>();
        playerController = GetComponentInParent<PlayerController>();

        // 활 오브젝트 자체에서 AnimationHandler 컴포넌트를 가져와 할당합니다.
        animationHandler = GetComponent<AnimationHandler>();
        if (animationHandler == null)
        {
            Debug.LogError($"{this.GetType().Name}: AnimationHandler 컴포넌트가 없습니다! {this.gameObject.name} 오브젝트에 있는지 확인하세요.");
        }

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

        // Bow는 기본 무기이므로 초기 능력 정보 설정
        AbilityName = "기본 활";
        Description = "플레이어의 기본 원거리 무기입니다.";
        MaxLevel = 1; // 기본 활은 레벨이 없거나 단일 레벨이라고 가정
        CurrentLevel = 1; // 획득 시 바로 1레벨로 시작 (OnAcquire가 호출되지 않는다면)
        // AbilityIcon은 Inspector에서 직접 할당하거나 Resources.Load를 통해 로드
        // AbilityIcon = Resources.Load<Sprite>("Icons/DefaultBowIcon");
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
        // Ability 클래스에 있는 FindTarget을 활용할 수도 있지만, Bow는 무기이므로 자체적으로 타겟을 찾는 것이 자연스러움
        target = FindTarget(); // Bow의 FindTarget은 Ability의 FindTarget보다 더 구체적인 범위 등을 가질 수 있음.

        if (target == null || (playerController != null && playerController.IsMoving))
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }
            StopAttack();
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

    // Bow가 직접 타겟을 찾는 로직
    protected new GameObject FindTarget() // 'new' 키워드는 Ability의 FindTarget을 숨깁니다.
    {
        if (player == null) return null;

        GameObject foundTarget = GameObject
            .FindGameObjectsWithTag("Enemy")
            .Where(enemy => enemy != null && enemy.activeInHierarchy && enemy.transform != null)
            .Where(enemy => Vector3.Distance(enemy.transform.position, player.transform.position) < player.AttackRange) // 플레이어의 공격 범위 사용
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, player.transform.position))
            .FirstOrDefault();
        return foundTarget;
    }

    protected float AttackDelay()
    {
        if (player == null) return 1f;

        // Player의 MaxAttackSpeed를 사용하여 딜레이 계산
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
            StartCoroutine(PerformAttackCycle());
            lastAttackTime = Time.time;
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

        if (animationHandler != null)
        {
            animationHandler.Attack();
        }
        else
        {
            Debug.LogWarning($"{this.GetType().Name}: AnimationHandler가 할당되지 않아 공격 애니메이션을 재생할 수 없습니다.");
            StopAttack();
            yield break;
        }

        LoadArrow();
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

        if (animator != null)
        {
            animator.SetBool("IsAttack", false);
        }

        if (loadedArrowGO != null)
        {
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
            Debug.Log($"{this.GetType().Name}: 장전된 화살을 오브젝트 풀로 반환했습니다.");
        }
    }

    // 애니메이션 이벤트에 의해 호출되는 화살 발사 메서드
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

        // 플레이어의 특수 화살 능력 발동 시도
        // 이때, loadedArrowGO와 loadedArrowScript는 특수 화살 능력에 의해 반환되거나 재사용될 수 있습니다.
        bool specialArrowFired = player.TryActivateSpecialArrowAbility(loadedArrowGO, loadedArrowScript);

        // 특수 화살이 발사되지 않았다면 일반 화살을 발사합니다.
        if (!specialArrowFired)
        {
            // 화살의 최종 발사 방향을 타겟을 향하도록 설정
            Vector3 finalLaunchDirection = (target.transform.position - firePoint.position).normalized;

            Rigidbody2D arrowRb = loadedArrowScript.GetComponent<Rigidbody2D>();
            if (arrowRb != null)
            {
                arrowRb.isKinematic = false;
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
        // 특수 화살이 발사되었든 아니든, loadedArrowGO는 이제 책임이 끝났습니다.
        loadedArrowGO = null;
        loadedArrowScript = null;
        isAttack = false;

        if (animator != null)
        {
            animator.SetBool("IsAttack", false);
        }
    }

    public override void ApplyEffect()
    {
        // Bow는 "무기"로서 기본 스탯 변경은 하지 않는다고 가정합니다.
        // 만약 Bow 자체에 레벨이 있고 레벨업에 따라 스탯이 변한다면 여기에 로직을 추가할 수 있습니다.
        // 하지만 일반적으로 Player.cs에서 Bow의 MaxAttackSpeed 등을 직접 참조하여 사용합니다.
    }
}