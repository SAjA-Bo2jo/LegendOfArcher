using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Bow : Ability
{
    // protected�� ����� �ʵ�� (�ڽ� Ŭ������ ���� �ý��ۿ��� ���� �����ϵ���)
    [SerializeField] protected Transform weaponPivot;
    [SerializeField] protected float radius = 0.3f;
    protected PlayerController playerController;
    [SerializeField] protected Transform firePoint; // ȭ���� �߻�� ��Ȯ�� ��ġ

    // ���� ���� �ִϸ��̼� ���� �� ����
    protected bool isAttack = false;

    protected GameObject loadedArrowGO; // ���� Ȱ�� ������ ȭ�� GameObject
    protected Arrow loadedArrowScript; // ���� Ȱ�� ������ ȭ�� ��ũ��Ʈ

    protected Animator animator; // Ȱ�� Animator ������Ʈ
    protected SpriteRenderer spriteRenderer; // Ȱ�� SpriteRenderer ������Ʈ

    // ������Ʈ Ǯ Ű�� ����̹Ƿ� public const�� ����
    public const string ARROW_POOL_KEY = "Arrow";

    // --- ������ �κ�: �ִϸ����� �Ķ���� �ؽø� Trigger�� ��Ȯ�ϰ� ���� ---
    private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");

    protected virtual void Start()
    {
        // �θ� ������Ʈ (Player)���� Player�� PlayerController ������Ʈ�� �����ɴϴ�.
        player = GetComponentInParent<Player>();
        playerController = GetComponentInParent<PlayerController>();

        // Bow ������Ʈ ��ü���� Animator ������Ʈ�� ������ �Ҵ��մϴ�.
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError($"{this.GetType().Name}: Animator ������Ʈ�� �����ϴ�! {this.gameObject.name}�� Animator�� �ִ��� Ȯ���ϼ���.");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"{this.GetType().Name}: SpriteRenderer ������Ʈ�� �����ϴ�! {this.gameObject.name}�� �ִ��� Ȯ���ϼ���.");
        }

        if (firePoint == null)
        {
            Debug.LogError($"{this.GetType().Name}: Fire Point�� �Ҵ���� �ʾҽ��ϴ�! {this.gameObject.name}���� ȭ�� �߻� ��ġ�� �������ּ���.");
        }

        abilityName = "�⺻ Ȱ";
        description = "�÷��̾��� �⺻ ���Ÿ� �����Դϴ�.";
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
            Debug.Log($"{this.GetType().Name}: PerformAttackCycle ���� ������ ���� �̴�. ������ ����մϴ�.");
            StopAttack();
            yield break;
        }

        LoadArrow();

        // --- ������ �κ�: Trigger �Ķ���͸� ����Ͽ� �ִϸ��̼� ��� ---
        if (animator != null)
        {
            animator.SetTrigger(AttackTriggerHash);
        }
        else
        {
            Debug.LogWarning($"{this.GetType().Name}: Animator�� �Ҵ���� �ʾ� ���� �ִϸ��̼��� ����� �� �����ϴ�.");
            StopAttack();
            yield break;
        }
        // �� �ڷ�ƾ�� �ִϸ��̼� �̺�Ʈ�� ȣ��� ������ ������� �ʽ��ϴ�.
    }

    protected void LoadArrow()
    {
        if (loadedArrowGO != null)
        {
            Debug.LogWarning($"{this.GetType().Name}: ���ο� ȭ���� �����ϱ� ���� ������ ������ ȭ���� Ǯ�� ��ȯ�մϴ�.");
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
        }

        loadedArrowGO = ObjectPoolManager.Instance.Get(ARROW_POOL_KEY);
        if (loadedArrowGO == null)
        {
            Debug.LogError($"{this.GetType().Name}: ������Ʈ Ǯ���� '{ARROW_POOL_KEY}'�� �������� ���߽��ϴ�. ȭ�� ���� ����.");
            StopAttack();
            return;
        }

        loadedArrowScript = loadedArrowGO.GetComponent<Arrow>();
        if (loadedArrowScript == null)
        {
            Debug.LogError($"{this.GetType().Name}: ȭ�� Prefab�� Arrow ��ũ��Ʈ�� �����ϴ�! ȭ�� ���� ����.");
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
            Debug.Log($"{this.GetType().Name}: ������ ȭ���� ������Ʈ Ǯ�� ��ȯ�߽��ϴ�.");
        }
    }

    // --- ������ �κ�: �ִϸ��̼� �̺�Ʈ�� ���� ȣ��Ǵ� ȭ�� �߻� �޼��� ---
    public void FireLoadedArrowFromAnimationEvent()
    {
        if ((playerController != null && playerController.IsMoving) || target == null)
        {
            StopAttack();
            return;
        }

        if (loadedArrowGO == null)
        {
            Debug.LogWarning($"{this.GetType().Name}: FireLoadedArrowFromAnimationEvent ȣ��Ǿ����� ������ ȭ���� �����ϴ�.");
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
        // Bow�� "����"�μ� �⺻ ���� ������ ���� �ʴ´ٰ� �����մϴ�.
    }
}