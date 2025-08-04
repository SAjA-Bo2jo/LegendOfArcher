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
    private bool isAttack = false;

    private GameObject loadedArrowGO;
    private Arrow loadedArrowScript;

    // �ִϸ��̼� �ӵ� ��� ���� Animator ������Ʈ ���� �߰�
    private Animator animator;
    // Ȱ�� ��������Ʈ�� �����ϱ� ���� SpriteRenderer ������Ʈ ���� �߰�
    private SpriteRenderer spriteRenderer;

    protected void Start()
    {
        // �θ� ������Ʈ���� Player�� PlayerController ������Ʈ�� �����ɴϴ�.
        player = GetComponentInParent<Player>();
        playerController = GetComponentInParent<PlayerController>();

<<<<<<< Updated upstream
        // Ȱ ������Ʈ���� AnimationHandler ������Ʈ�� ������ �Ҵ��մϴ�.
        // ������ �θ�(Player)���� �������� ���� �����մϴ�.
=======
>>>>>>> Stashed changes
        animationHandler = GetComponent<AnimationHandler>();
        if (animationHandler == null)
        {
            Debug.LogError("Bow ��ũ��Ʈ�� AnimationHandler ������Ʈ�� �����ϴ�! Bow ������Ʈ�� �ִ��� Ȯ���ϼ���.");
        }

<<<<<<< Updated upstream
        // ��ũ�������� Ȯ�ε� Animator ������Ʈ�� �����ɴϴ�.
=======
>>>>>>> Stashed changes
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Bow ��ũ��Ʈ�� Animator ������Ʈ�� �����ϴ�! Player > WeaponPivot > Bow�� Animator�� �ִ��� Ȯ���ϼ���.");
        }

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
        // �� ������ Ÿ���� ã��, ������ �õ��մϴ�.
        target = FindTarget();
        TryAttack();

        // --- �ٽ� ���� �κ�: Ÿ���� ���ų� �÷��̾ ������ ��� ���� �ߴ� �� ȭ�� ��ȯ ---
        if (target == null || playerController.IsMoving)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }
<<<<<<< Updated upstream
            if (animator != null)
            {
                // ���� �ִϸ��̼��� ����ϴ�.
                animator.SetBool("IsAttack", false);
            }
            StopAttack(); // Ÿ���� ������ ���� ����Ŭ�� ����
=======
            StopAttack(); // ���� �ߴ� �� ������ ȭ�� ��ȯ
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
        if (isAttack) return; // �̹� ���� ���̸� ��ŵ

        if (playerController != null && playerController.IsMoving)
        {
            Debug.Log("Bow: �÷��̾ �����̰� �־� ������ �õ����� �ʽ��ϴ�.");
=======
        if (isAttack) return;

        // �÷��̾� ������ üũ�� AttackDelay ������ �ű��� �ʰ�, ���⼭ �̸� üũ�մϴ�.
        if (playerController != null && playerController.IsMoving)
        {
            // Debug.Log("Bow: �÷��̾ �����̰� �־� ������ �õ����� �ʽ��ϴ�."); // �ʹ� ���� ��µ� �� �־� �ּ� ó��
>>>>>>> Stashed changes
            return;
        }

        if (target == null)
        {
<<<<<<< Updated upstream
            Debug.Log("Bow: Ÿ���� ���� ������ �õ����� �ʽ��ϴ�.");
=======
            // Debug.Log("Bow: Ÿ���� ���� ������ �õ����� �ʽ��ϴ�."); // �ʹ� ���� ��µ� �� �־� �ּ� ó��
>>>>>>> Stashed changes
            return;
        }

        float delay = AttackDelay();
        if (Time.time >= lastAttackTime + delay)
        {
<<<<<<< Updated upstream
            Debug.Log("Bow: ���� ����!");
            StartCoroutine(PerformAttackCycle()); // �����̴� �ڷ�ƾ ������ ó��
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
            Debug.Log("Bow: PerformAttackCycle ���� ������ �÷��̾� �̵� ����. ������ ����մϴ�.");
            StopAttack();
            yield break;
        }

        if (player == null)
        {
            Debug.LogError("Bow: Player ������Ʈ�� ã�� �� �����ϴ�.");
            StopAttack();
            yield break;
        }

        if (animationHandler != null)
        {
            animationHandler.Attack(); // Ȱ ���� �ִϸ��̼� ���
        }
        else
        {
            Debug.LogWarning("Bow: AnimationHandler�� �Ҵ���� �ʾ� ���� �ִϸ��̼��� ����� �� �����ϴ�.");
            StopAttack();
            yield break;
        }

<<<<<<< Updated upstream
        // �ִϸ��̼��� ���۵Ǹ� �ٷ� ȭ���� ����
        LoadArrow();

        // �ִϸ��̼� �̺�Ʈ�� ���� ȭ���� �߻�� ������ ���
        // isAttack�� Animation Event���� FireLoadedArrowFromAnimationEvent ȣ�� �� false�� ����
        // �Ǵ� ���� �ִϸ��̼��� �����µ��� FireLoadedArrowFromAnimationEvent�� ȣ����� �ʾҴٸ� (��: �ִϸ��̼��� �ߴܵ� ���)
        // ���� ��ġ�� ���� �ð� �� isAttack�� false�� �ǵ����ϴ�.
        // ���⼭�� �ִϸ��̼� �̺�Ʈ���� isAttack�� false�� �����ϴ� ���� ����մϴ�.
        // ���� �ִϸ��̼� �̺�Ʈ�� FireLoadedArrowFromAnimationEvent�� ȣ������ �ʴ� ��츦 ����� Ÿ�Ӿƿ� ������ �ʿ��ϴٸ� �߰��� �� �ֽ��ϴ�.
        // ���� ���, �ִ� ���� ������ �ð���ŭ ��ٸ� �Ŀ��� isAttack�� true��� ���� ����
        // yield return new WaitForSeconds(totalAttackDelay * 2); // ����: �ִ� ���� �������� �� �� �ð� �Ŀ��� ���� ���̶��
        // if (isAttack) { StopAttack(); }

        // ������ ȭ���� �߻�ǰų� ������ �ߴܵ� ������ ��ٸ��ϴ�.
        // Animator�� "IsAttack" ���°� false�� �ǰų�, loadedArrowGO�� null�� �� ������ ��ٸ� �� �ֽ��ϴ�.
        // ���⼭�� FireLoadedArrowFromAnimationEvent()���� isAttack�� false�� �����ϰ� loadedArrowGO�� null�� ����� ������
        // ������ �� ��� �ð� ���� �ִϸ��̼� �̺�Ʈ�� ȣ���� ��ٸ��� �˴ϴ�.
=======
        LoadArrow(); // �ִϸ��̼� ���۰� ���ÿ� ȭ�� ����
>>>>>>> Stashed changes
    }

    // ȭ���� ������Ʈ Ǯ���� ������ Ȱ�� �����ϴ� �Լ�
    private void LoadArrow()
    {
<<<<<<< Updated upstream
        if (loadedArrowGO != null)
        {
            // �̹� ������ ȭ���� �ִٸ� ��ȯ�ϰ� ���� ����
=======
        // --- �ٽ� ���� �κ�: �̹� ������ ȭ���� �ִٸ� ���� Ǯ�� ��ȯ ---
        if (loadedArrowGO != null)
        {
            Debug.LogWarning("Bow: ���ο� ȭ���� �����ϱ� ���� ������ ������ ȭ���� ��ȯ�մϴ�.");
>>>>>>> Stashed changes
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
        }

        loadedArrowGO = ObjectPoolManager.Instance.Get(ARROW_POOL_KEY);
        if (loadedArrowGO == null)
        {
            Debug.LogError($"Bow: ������Ʈ Ǯ���� '{ARROW_POOL_KEY}'�� �������� ���߽��ϴ�. ȭ�� ���� ����.");
            StopAttack();
            return;
        }

        loadedArrowScript = loadedArrowGO.GetComponent<Arrow>();
        if (loadedArrowScript == null)
        {
            Debug.LogError("Bow: ȭ�� Prefab�� Arrow ��ũ��Ʈ�� �����ϴ�! ȭ�� ���� ����.");
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
            StopAttack();
            return;
        }

        Rigidbody2D arrowRb = loadedArrowScript.GetComponent<Rigidbody2D>();
        if (arrowRb != null)
        {
            arrowRb.isKinematic = true; // ���� ���� ���� �ʵ���
            arrowRb.simulated = false; // �ùķ��̼� ��Ȱ��ȭ
            arrowRb.velocity = Vector2.zero;
            arrowRb.angularVelocity = 0;
        }
        loadedArrowGO.transform.position = firePoint.position;
        loadedArrowGO.transform.rotation = transform.rotation;
        loadedArrowScript.transform.localScale = Vector3.one * player.AttackSize;
        loadedArrowGO.SetActive(true);
<<<<<<< Updated upstream
        Debug.Log("Bow: ȭ�� ���� �Ϸ�!");
    }


    public void StopAttack()
    {
        Debug.Log("Bow: StopAttack() ȣ��. ���� ���¸� �ʱ�ȭ�մϴ�.");
        StopAllCoroutines();
=======
    }

    public void StopAttack()
    {
        StopAllCoroutines(); // ��� Ȱ ���� �ڷ�ƾ ����
>>>>>>> Stashed changes
        isAttack = false;

        if (animator != null)
        {
<<<<<<< Updated upstream
            animator.SetBool("IsAttack", false);
        }

=======
            animator.SetBool("IsAttack", false); // ���� �ִϸ��̼� ����
        }

        // --- �ٽ� ���� �κ�: ������ ȭ���� �ִٸ� �ݵ�� Ǯ�� ��ȯ ---
>>>>>>> Stashed changes
        if (loadedArrowGO != null)
        {
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
<<<<<<< Updated upstream
=======
            Debug.Log("Bow: ������ ȭ���� ������Ʈ Ǯ�� ��ȯ�߽��ϴ�.");
>>>>>>> Stashed changes
        }
    }

    public void FireLoadedArrowFromAnimationEvent()
    {
        if (playerController != null && playerController.IsMoving)
        {
            Debug.Log("Bow: �÷��̾ �������� ȭ�� �߻縦 ����ϰ� ������ �����մϴ�.");
            StopAttack();
<<<<<<< Updated upstream
            return;
        }

        if (target == null)
        {
            Debug.Log("Bow: Ÿ���� ���� ȭ�� �߻縦 ����ϰ� ������ �����մϴ�.");
            StopAttack(); // Ÿ���� ������ ���� ����Ŭ ����
=======
            return;
        }

        if (target == null)
        {
            Debug.Log("Bow: Ÿ���� ���� ȭ�� �߻縦 ����ϰ� ������ �����մϴ�.");
            StopAttack();
            return;
        }

        if (loadedArrowGO == null)
        {
            Debug.LogWarning("Bow: ������ ȭ���� ���� �߻��� �� �����ϴ�. �ִϸ��̼� �̺�Ʈ ȣ�� ������ loadedArrowGO�� null�Դϴ�.");
            StopAttack();
>>>>>>> Stashed changes
            return;
        }

        // ������ ȭ���� ������ �߻����� �ʰ�, ������ �ߴ��մϴ�.
        // ���� PerformAttackCycle���� ȭ���� �̸� �����ϹǷ�, �� �������� loadedArrowGO�� null�̸� ������ �ִ� ���Դϴ�.
        if (loadedArrowGO == null)
        {
            Debug.LogWarning("Bow: ������ ȭ���� ���� �߻��� �� �����ϴ�. �ִϸ��̼� �̺�Ʈ ȣ�� ������ loadedArrowGO�� null�Դϴ�.");
            StopAttack();
            return;
        }

        Debug.Log("Bow: ȭ�� �߻�!");
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
        // ȭ�� �߻� �� loadedArrowGO�� loadedArrowScript�� �ʱ�ȭ�ϰ� ���� ���� ����
        loadedArrowGO = null;
        loadedArrowScript = null;
        isAttack = false; // ���� ����Ŭ �Ϸ�
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