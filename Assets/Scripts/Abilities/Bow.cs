// Bow.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Bow : Ability
{
    // --- Bow���� �ʿ��� �������� ���⿡ �����մϴ�. ---
    [SerializeField] protected Transform weaponPivot;
    [SerializeField] protected float radius = 0.3f;
    protected PlayerController playerController;
    [SerializeField] protected Transform firePoint;
    protected float lastAttackTime; // ���� �����̸� ���� ����
    protected bool isAttack = false;
    protected GameObject loadedArrowGO;
    protected Arrow loadedArrowScript;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    public const string ARROW_POOL_KEY = "Arrow";
    private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");

    protected override void Start()
    {
        base.Start(); // <-- �θ� Ŭ����(Ability)�� Start()�� ȣ���մϴ�.

        // Bow�� ������ �ʱ�ȭ ������ ���⿡ �ۼ��մϴ�.
        playerController = GetComponentInParent<PlayerController>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (playerController == null) Debug.LogError($"{this.GetType().Name}: PlayerController ������Ʈ�� �����ϴ�!");
        if (animator == null) Debug.LogError($"{this.GetType().Name}: Animator ������Ʈ�� �����ϴ�!");
        if (spriteRenderer == null) Debug.LogError($"{this.GetType().Name}: SpriteRenderer ������Ʈ�� �����ϴ�!");
        if (firePoint == null) Debug.LogError($"{this.GetType().Name}: Fire Point�� �Ҵ���� �ʾҽ��ϴ�!");
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
            if (spriteRenderer != null) spriteRenderer.enabled = false;
            if (isAttack) StopAttack();
        }
        else
        {
            if (spriteRenderer != null) spriteRenderer.enabled = true;
            TryAttack();
        }
    }

    protected GameObject FindTarget()
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
        if ((playerController != null && playerController.IsMoving) || target == null) return;

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
            StopAttack();
            yield break;
        }

        LoadArrow();
        if (animator != null)
        {
            animator.SetTrigger(AttackTriggerHash);
        }
        else
        {
            StopAttack();
            yield break;
        }
    }

    protected void LoadArrow()
    {
        if (loadedArrowGO != null)
        {
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
        }

        loadedArrowGO = ObjectPoolManager.Instance.Get(ARROW_POOL_KEY);
        if (loadedArrowGO == null)
        {
            StopAttack();
            return;
        }

        loadedArrowScript = loadedArrowGO.GetComponent<Arrow>();
        if (loadedArrowScript == null)
        {
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
        }
    }

    public void FireLoadedArrowFromAnimationEvent()
    {
        if ((playerController != null && playerController.IsMoving) || target == null)
        {
            StopAttack();
            return;
        }

        if (loadedArrowGO == null)
        {
            StopAttack();
            return;
        }

        bool specialArrowFired = player.TryActivateSpecialArrowAbility(loadedArrowGO, loadedArrowScript);

        if (!specialArrowFired)
        {
            Vector3 finalLaunchDirection = (target.transform.position - firePoint.position).normalized;

            // --- Arrow ��ũ��Ʈ�� player ������ ���� �Ҵ��մϴ�. ---
            if (loadedArrowScript != null)
            {
                loadedArrowScript.player = this.player;
            }

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
        // ... (Bow���� ȿ��)
    }
}