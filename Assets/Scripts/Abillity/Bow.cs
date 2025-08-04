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

    [Header("Ȱ ���� ����")]
    private bool isAttacking = false;

    private GameObject loadedArrowGO;
    private Arrow loadedArrowScript;

    // �ִϸ��̼� �ӵ� ��� ���� Animator ������Ʈ ���� �߰�
    private Animator animator;
    // Ȱ�� ��������Ʈ�� �����ϱ� ���� SpriteRenderer ������Ʈ ���� �߰�
    private SpriteRenderer spriteRenderer;

    protected void Start()
    {
        player = GetComponentInParent<Player>();
        animationHandler = GetComponent<AnimationHandler>();
        playerController = GetComponentInParent<PlayerController>();

        // ��ũ�������� Ȯ�ε� Animator ������Ʈ�� �����ɴϴ�.
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Bow ��ũ��Ʈ�� Animator ������Ʈ�� �����ϴ�! Player > WeaponPivot > Bow�� Animator�� �ִ��� Ȯ���ϼ���.");
        }

        // SpriteRenderer ������Ʈ�� �����ɴϴ�.
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Bow ��ũ��Ʈ�� SpriteRenderer ������Ʈ�� �����ϴ�!");
        }

        if (firePoint == null)
        {
            Debug.LogError("Bow ��ũ��Ʈ�� Fire Point�� �Ҵ���� �ʾҽ��ϴ�! ȭ�� �߻� ��ġ�� �������ּ���.");
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

        // �ִϸ��̼��� ����Ǵ� ���� ������ ȭ���� Ȱ ���� �����մϴ�.
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

        // Ÿ���� ������ Ȱ�� �����, ������ ���̰� �մϴ�.
        if (target == null)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }
            if (animator != null)
            {
                // ���� �ִϸ��̼��� ����ϴ�.
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

        // Ÿ���� ������ ���� �õ� ��ü�� ���� �ʽ��ϴ�.
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
            Debug.LogError("Bow: Player ������Ʈ�� ã�� �� �����ϴ�.");
            isAttacking = false;
            yield break;
        }

        // �ִϸ��̼� �ӵ��� �÷��̾��� ���� �ӵ��� ���� �������� �����մϴ�.
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
            Debug.LogWarning("Bow: AnimationHandler�� �Ҵ���� �ʾ� ���� �ִϸ��̼��� ����� �� �����ϴ�.");
            isAttacking = false;
            yield break;
        }

        // ȭ���� �̸� Ǯ���� ������ ���� ���·� ����ϴ�.
        loadedArrowGO = ObjectPoolManager.Instance.Get(ARROW_POOL_KEY);
        if (loadedArrowGO == null)
        {
            Debug.LogError($"Bow: ������Ʈ Ǯ���� '{ARROW_POOL_KEY}'�� �������� ���߽��ϴ�.");
            isAttacking = false;
            yield break;
        }

        loadedArrowScript = loadedArrowGO.GetComponent<Arrow>();
        if (loadedArrowScript == null)
        {
            Debug.LogError("Bow: ȭ�� Prefab�� Arrow ��ũ��Ʈ�� �����ϴ�!");
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

        // �� ���� ��Ÿ�� ���
        yield return new WaitForSeconds(totalAttackDelay);

        isAttacking = false;
    }

    /// <summary>
    /// Unity �ִϸ��̼� �̺�Ʈ���� ȣ��Ǵ� ȭ�� �߻� �޼����Դϴ�.
    /// loadedArrowGO�� null�� ���, ��� ȭ���� �����Ͽ� �߻��ϴ� ������ġ ������ �߰��Ǿ����ϴ�.
    /// </summary>
    public void FireLoadedArrowFromAnimationEvent()
    {
        // Ÿ���� ������ �߻����� �ʽ��ϴ�. ���� Ȯ�� �����Դϴ�.
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

        // ���� loadedArrowGO�� �غ���� �ʾҴٸ�, ��� �����Ͽ� �߻��ϴ� ������ġ ����
        if (loadedArrowGO == null)
        {

            loadedArrowGO = ObjectPoolManager.Instance.Get(ARROW_POOL_KEY);
            if (loadedArrowGO == null)
            {
                Debug.LogError($"Bow: ������Ʈ Ǯ���� '{ARROW_POOL_KEY}'�� �������� ���߽��ϴ�.");
                return;
            }

            loadedArrowScript = loadedArrowGO.GetComponent<Arrow>();
            if (loadedArrowScript == null)
            {
                Debug.LogError("Bow: ȭ�� Prefab�� Arrow ��ũ��Ʈ�� �����ϴ�!");
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
        Debug.Log($"Bow: Ÿ�� '{target.name}'�� ã�ҽ��ϴ�. �߻� ����: {finalLaunchDirection}");

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

        // �߻� �� ȭ�� ������ �ʱ�ȭ�մϴ�.
        loadedArrowGO = null;
        loadedArrowScript = null;
    }

    public override void ApplyEffect() { }

    public override void RemoveEffect() { }
}
