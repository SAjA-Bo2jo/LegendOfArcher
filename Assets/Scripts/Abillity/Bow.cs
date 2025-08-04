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
    private bool isAttack = false; // ���� ���� �ִϸ��̼��� ���� ������ ��Ÿ���ϴ�.

    private GameObject loadedArrowGO; // ���� Ȱ�� ������ ȭ�� GameObject
    private Arrow loadedArrowScript; // ���� Ȱ�� ������ ȭ�� ��ũ��Ʈ

    // �ִϸ��̼� �ӵ� ��� ���� Animator ������Ʈ ����
    private Animator animator;
    // Ȱ�� ��������Ʈ�� �����ϱ� ���� SpriteRenderer ������Ʈ ����
    private SpriteRenderer spriteRenderer;

    protected void Start()
    {
        // �θ� ������Ʈ (Player)���� Player�� PlayerController ������Ʈ�� �����ɴϴ�.
        player = GetComponentInParent<Player>();
        playerController = GetComponentInParent<PlayerController>();

        // Ȱ ������Ʈ ��ü���� AnimationHandler ������Ʈ�� ������ �Ҵ��մϴ�.
        // Bow�� Abillity�� ��ӹ޾Ұ� Abillity ���ο� animationHandler �ʵ尡 �����Ƿ�,
        // �� �ʵ带 Ȱ���Ͽ� Bow�� AnimationHandler�� �����մϴ�.
        animationHandler = GetComponent<AnimationHandler>();
        if (animationHandler == null)
        {
            Debug.LogError("Bow ��ũ��Ʈ�� AnimationHandler ������Ʈ�� �����ϴ�! Bow ������Ʈ�� �ִ��� Ȯ���ϼ���.");
        }

        // ��ũ�������� Ȯ�ε� Animator ������Ʈ�� �����ɴϴ�.
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Bow ��ũ��Ʈ�� Animator ������Ʈ�� �����ϴ�! Player > WeaponPivot > Bow GameObject�� Animator�� �ִ��� Ȯ���ϼ���.");
        }

        // Ȱ�� ��������Ʈ�� �����ϱ� ���� SpriteRenderer ������Ʈ�� �����ɴϴ�.
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Bow ��ũ��Ʈ�� SpriteRenderer ������Ʈ�� �����ϴ�!");
        }

        // ȭ���� �߻�� ��ġ�� Ȯ���մϴ�.
        if (firePoint == null)
        {
            Debug.LogError("Bow ��ũ��Ʈ�� Fire Point�� �Ҵ���� �ʾҽ��ϴ�! ȭ�� �߻� ��ġ�� �������ּ���.");
        }
    }

    // ������Ʈ�� ��ġ�� ȸ���� ������Ʈ�մϴ�. (�ַ� PlayerController�� LookDirection�� ����)
    private void LateUpdate()
    {
        if (player == null || weaponPivot == null || playerController == null) return;

        // �÷��̾��� �ü� ���⿡ ���� Ȱ�� ��ġ�� �������� ��ġ�մϴ�.
        float angle = Mathf.Atan2(playerController.LookDirection.y, playerController.LookDirection.x);
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
        transform.position = weaponPivot.position + offset;

        // Ȱ�� ȸ���� �÷��̾� �ü� ���⿡ ����ϴ�.
        float angleDeg = angle * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angleDeg);

        // �ִϸ��̼��� ����Ǵ� ���� ������ ȭ���� Ȱ ���� �����մϴ�.
        if (loadedArrowGO != null)
        {
            loadedArrowGO.transform.position = firePoint.position;
            loadedArrowGO.transform.rotation = transform.rotation;
        }
    }

    // �� �����Ӹ��� ���� ������ ������Ʈ�մϴ�.
    protected void Update()
    {
        // �� ������ Ÿ���� ã���ϴ�.
        target = FindTarget();

        // Ÿ���� ���ų� �÷��̾ ������ ��� ������ �ߴ��ϰ� Ȱ ��������Ʈ�� ��Ȱ��ȭ�մϴ�.
        if (target == null || (playerController != null && playerController.IsMoving))
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false; // Ȱ ��������Ʈ ��Ȱ��ȭ
            }
            StopAttack(); // ���� ����Ŭ ���� �� ������ ȭ�� ��ȯ, �ִϸ��̼� ����
        }
        else // Ÿ���� �ְ� �÷��̾ �������� ���� ��� ������ �õ��ϰ� Ȱ ��������Ʈ�� Ȱ��ȭ�մϴ�.
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true; // Ȱ ��������Ʈ Ȱ��ȭ
            }
            TryAttack(); // ���� �õ�
        }
    }

    // ���� ����� ���� Ÿ������ �����մϴ�.
    protected GameObject FindTarget()
    {
        if (player == null) return null; // Player ������Ʈ�� ������ Ÿ�� Ž�� �Ұ�

        GameObject target = GameObject
            .FindGameObjectsWithTag("Enemy")
            .Where(enemy => enemy != null && enemy.activeInHierarchy && enemy.transform != null) // null üũ �� Ȱ�� ���� üũ
            .Where(enemy => Vector3.Distance(enemy.transform.position, transform.position) < player.AttackRange) // ���� ���� �� �� ���͸�
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position)) // �Ÿ��� ����
            .FirstOrDefault(); // ���� ����� �� ����
        return target;
    }

    // ���� ������ �ð��� ����մϴ�.
    protected float AttackDelay()
    {
        float totalAttackSpeed = player.MaxAttackSpeed;

        if (totalAttackSpeed <= 0.01f) totalAttackSpeed = 0.01f; // 0���� ������ ���� ����

        float delay = 1f / totalAttackSpeed; // �ʴ� ���� Ƚ���� �����̷� ��ȯ
        return delay;
    }

    // ������ �õ��մϴ�.
    protected void TryAttack()
    {
        if (isAttack) return; // �̹� ���� ���̸� ��ŵ (�ߺ� ���� ����)

        // �÷��̾ �����̰ų� Ÿ���� ������ ���� �õ����� ����
        if ((playerController != null && playerController.IsMoving) || target == null)
        {
            // Debug.Log("Bow: �÷��̾ �����̰ų� Ÿ���� ���� ������ �õ����� �ʽ��ϴ�."); // �ʹ� ���� ��µ� �� �־� �ּ� ó��
            return;
        }

        // ���� ���ݱ����� �����̰� �������� Ȯ��
        float delay = AttackDelay();
        if (Time.time >= lastAttackTime + delay)
        {
            StartCoroutine(PerformAttackCycle()); // ���� ����Ŭ �ڷ�ƾ ����
            lastAttackTime = Time.time; // ������ ���� �ð� ������Ʈ
        }
    }

    // ���� �ִϸ��̼� �� �߻� ������ �����ϴ� �ڷ�ƾ
    private IEnumerator PerformAttackCycle()
    {
        isAttack = true; // ���� ���� ���·� ����

        // �ڷ�ƾ ���� ������ �÷��̾� �̵� �Ǵ� Ÿ�� ��� ���� �ٽ� Ȯ��
        if ((playerController != null && playerController.IsMoving) || target == null)
        {
            Debug.Log("Bow: PerformAttackCycle ���� ������ ���� �̴�. ������ ����մϴ�.");
            StopAttack();
            yield break; // �ڷ�ƾ ��� ����
        }

        if (player == null)
        {
            Debug.LogError("Bow: Player ������Ʈ�� ã�� �� �����ϴ�. ������ �ߴ��մϴ�.");
            StopAttack();
            yield break;
        }

        // Ȱ ���� �ִϸ��̼� ��� ��û
        if (animationHandler != null)
        {
            animationHandler.Attack();
        }
        else
        {
            Debug.LogWarning("Bow: AnimationHandler�� �Ҵ���� �ʾ� ���� �ִϸ��̼��� ����� �� �����ϴ�.");
            StopAttack();
            yield break;
        }

        // �ִϸ��̼� ���۰� ���ÿ� ȭ���� �̸� �����մϴ�.
        LoadArrow();

        // �ڷ�ƾ�� ���⼭ ������� �ʰ�, �ִϸ��̼� �̺�Ʈ�� ����
        // FireLoadedArrowFromAnimationEvent()�� ȣ��Ǳ⸦ ��ٸ��ϴ�.
        // FireLoadedArrowFromAnimationEvent()���� isAttack�� false�� �����ϰ�
        // loadedArrowGO�� null�� ����� ���� ����Ŭ�� �Ϸ��ų ���Դϴ�.
        // ���� �� �ڷ�ƾ�� FireLoadedArrowFromAnimationEvent() ȣ�� ��������
        // �ٸ� �۾��� ���� �ʰ� �ܼ��� ���¸� �����ϰ� ȭ���� �����ϴ� ���Ҹ� �մϴ�.
        // �ʿ��� ���, �ִϸ��̼��� �ʹ� ����� �̺�Ʈ�� ȣ����� �ʴ� ��Ȳ�� �����
        // Ÿ�Ӿƿ� ���� (yield return new WaitForSeconds(maxAnimationDuration);)�� �߰��� �� �ֽ��ϴ�.
    }

    // ȭ���� ������Ʈ Ǯ���� ������ Ȱ�� �����ϴ� �Լ�
    private void LoadArrow()
    {
        // ������ ������ ȭ���� �ִٸ� Ǯ�� ��ȯ�ϰ� �ʱ�ȭ�մϴ�.
        if (loadedArrowGO != null)
        {
            Debug.LogWarning("Bow: ���ο� ȭ���� �����ϱ� ���� ������ ������ ȭ���� Ǯ�� ��ȯ�մϴ�.");
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
        }

        // ������Ʈ Ǯ���� �Ϲ� ȭ���� �����ɴϴ�.
        loadedArrowGO = ObjectPoolManager.Instance.Get(ARROW_POOL_KEY);
        if (loadedArrowGO == null)
        {
            Debug.LogError($"Bow: ������Ʈ Ǯ���� '{ARROW_POOL_KEY}'�� �������� ���߽��ϴ�. ȭ�� ���� ����.");
            StopAttack(); // ���� �� ���� ����Ŭ �ߴ�
            return;
        }

        // ȭ�� ��ũ��Ʈ ������Ʈ�� �����ɴϴ�.
        loadedArrowScript = loadedArrowGO.GetComponent<Arrow>();
        if (loadedArrowScript == null)
        {
            Debug.LogError("Bow: ȭ�� Prefab�� Arrow ��ũ��Ʈ�� �����ϴ�! ȭ�� ���� ����.");
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO); // ������ ȭ�� ��ȯ
            loadedArrowGO = null;
            loadedArrowScript = null;
            StopAttack(); // ���� �� ���� ����Ŭ �ߴ�
            return;
        }

        // ������ ȭ���� ���� ������ ��Ȱ��ȭ�Ͽ� Ȱ�� �����ǵ��� �մϴ�.
        Rigidbody2D arrowRb = loadedArrowScript.GetComponent<Rigidbody2D>();
        if (arrowRb != null)
        {
            arrowRb.isKinematic = true; // ���� ���� ���� �ʵ���
            arrowRb.simulated = false; // ���� �ùķ��̼� ��Ȱ��ȭ
            arrowRb.velocity = Vector2.zero;
            arrowRb.angularVelocity = 0;
        }

        // ȭ���� ��ġ�� ȸ���� Ȱ ���� ���߰� �������� �����մϴ�.
        loadedArrowGO.transform.position = firePoint.position;
        loadedArrowGO.transform.rotation = transform.rotation;
        loadedArrowScript.transform.localScale = Vector3.one * player.AttackSize;
        loadedArrowGO.SetActive(true); // ȭ�� Ȱ��ȭ
        Debug.Log("Bow: ȭ�� ���� �Ϸ�!");
    }

    // ���� ���� ���� ������ �ߴ��ϰ� ��� ���� ���¸� �ʱ�ȭ�մϴ�.
    public void StopAttack()
    {
        StopAllCoroutines(); // Ȱ ���� ��� �ڷ�ƾ ����
        isAttack = false; // ���� �� �ƴ����� ����

        // Animator�� ���� ���¸� �����մϴ�.
        if (animator != null)
        {
            animator.SetBool("IsAttack", false);
        }

        // ������ ȭ���� �ִٸ� ������Ʈ Ǯ�� ��ȯ�մϴ�.
        if (loadedArrowGO != null)
        {
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
            Debug.Log("Bow: ������ ȭ���� ������Ʈ Ǯ�� ��ȯ�߽��ϴ�.");
        }
    }

    // �ִϸ��̼� �̺�Ʈ�� ���� ȣ��Ǵ� ȭ�� �߻� �޼���
    public void FireLoadedArrowFromAnimationEvent()
    {
        // �߻� ������ �ٽ� �� �� �÷��̾� ���¿� Ÿ�� ���θ� Ȯ���մϴ�.
        if ((playerController != null && playerController.IsMoving) || target == null)
        {
            Debug.Log("Bow: �߻� ���� �÷��̾� �̵� �Ǵ� Ÿ�� ��� ����. ȭ�� �߻縦 ����ϰ� ������ �ߴ��մϴ�.");
            StopAttack();
            return;
        }

        // ������ ȭ���� ������ ����ϰ� ������ �ߴ��մϴ�. (LoadArrow�� ����� �۵����� �ʾҰų�, �̹� �߻�� ���)
        if (loadedArrowGO == null)
        {
            Debug.LogWarning("Bow: ������ ȭ���� ���� �߻��� �� �����ϴ�. �ִϸ��̼� �̺�Ʈ ȣ�� ������ loadedArrowGO�� null�Դϴ�.");
            StopAttack();
            return;
        }

        Debug.Log("Bow: �ִϸ��̼� �̺�Ʈ�� ���� ȭ�� �߻�!");
        // Ÿ�� �������� ȭ���� �߻��մϴ�.
        Vector3 finalLaunchDirection = (target.transform.position - firePoint.position).normalized;

        // Ư�� ȭ�� �ɷ��� Ȱ��ȭ�� �� �ִ��� �õ��մϴ�.
        bool specialArrowFired = player.TryActivateSpecialArrowAbility(loadedArrowGO, loadedArrowScript);

        // Ư�� ȭ���� �߻���� �ʾҴٸ� �Ϲ� ȭ���� �߻��մϴ�.
        if (!specialArrowFired)
        {
            Rigidbody2D arrowRb = loadedArrowScript.GetComponent<Rigidbody2D>();
            if (arrowRb != null)
            {
                arrowRb.isKinematic = false; // ���� �ùķ��̼� �ٽ� Ȱ��ȭ
                arrowRb.simulated = true;
            }

            // ȭ���� ������ �÷��̾��� ���� ���ȿ� ���� �����մϴ�.
            loadedArrowScript.Setup(
                damage: player.AttackDamage,
                size: player.AttackSize,
                critRate: player.CriticalRate,
                speed: player.ProjectileSpeed
            );
            loadedArrowScript.LaunchTowards(finalLaunchDirection); // ȭ�� �߻�
        }

        // ȭ�� �߻� �� ������ ȭ�� ������ �ʱ�ȭ�ϰ� ���� ���¸� �����մϴ�.
        loadedArrowGO = null;
        loadedArrowScript = null;
        isAttack = false; // ���� ����Ŭ �Ϸ�

        // Animator�� ���� ���¸� �����մϴ�.
        if (animator != null)
        {
            animator.SetBool("IsAttack", false);
        }
    }

    // Abillity Ŭ�������� ��ӹ��� �߻� �޼��� ���� (�ʿ信 ���� ���� �߰�)
    public override void ApplyEffect() { }
    public override void RemoveEffect() { }
}