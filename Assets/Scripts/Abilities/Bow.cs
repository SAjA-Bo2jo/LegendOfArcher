using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Bow : Ability
{
    // protected�� ����� �ʵ�� (GatlingBow ���� �ڽ� Ŭ������ ���� �ý��ۿ��� ���� �����ϵ���)
    [SerializeField] protected Transform weaponPivot;
    [SerializeField] protected float radius = 0.3f;
    protected PlayerController playerController;
    [SerializeField] protected Transform firePoint; // ȭ���� �߻�� ��Ȯ�� ��ġ

    protected bool isAttack = false; // ���� ���� �ִϸ��̼� ���� �� ����

    protected GameObject loadedArrowGO; // ���� Ȱ�� ������ ȭ�� GameObject
    protected Arrow loadedArrowScript; // ���� Ȱ�� ������ ȭ�� ��ũ��Ʈ

    protected Animator animator; // Ȱ�� Animator ������Ʈ
    protected SpriteRenderer spriteRenderer; // Ȱ�� SpriteRenderer ������Ʈ

    // ������Ʈ Ǯ Ű�� ����̹Ƿ� public const�� ����
    public const string ARROW_POOL_KEY = "Arrow";
    // FireArrow�� �ٸ� �ɷ¿��� ����ϹǷ� Bow������ �ʿ� �����ϴ�.

    // Start �޼��带 protected virtual�� �����Ͽ� �ڽ� Ŭ�������� �������̵� �����ϰ� ��
    protected virtual void Start()
    {
        // �θ� ������Ʈ (Player)���� Player�� PlayerController ������Ʈ�� �����ɴϴ�.
        // Bow�� �÷��̾��� �ڽ����� �پ��ִٰ� �����մϴ�.
        player = GetComponentInParent<Player>();
        playerController = GetComponentInParent<PlayerController>();

        // Ȱ ������Ʈ ��ü���� AnimationHandler ������Ʈ�� ������ �Ҵ��մϴ�.
        animationHandler = GetComponent<AnimationHandler>();
        if (animationHandler == null)
        {
            Debug.LogError($"{this.GetType().Name}: AnimationHandler ������Ʈ�� �����ϴ�! {this.gameObject.name} ������Ʈ�� �ִ��� Ȯ���ϼ���.");
        }

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

        // Bow�� �⺻ �����̹Ƿ� �ʱ� �ɷ� ���� ����
        AbilityName = "�⺻ Ȱ";
        Description = "�÷��̾��� �⺻ ���Ÿ� �����Դϴ�.";
        MaxLevel = 1; // �⺻ Ȱ�� ������ ���ų� ���� �����̶�� ����
        CurrentLevel = 1; // ȹ�� �� �ٷ� 1������ ���� (OnAcquire�� ȣ����� �ʴ´ٸ�)
        // AbilityIcon�� Inspector���� ���� �Ҵ��ϰų� Resources.Load�� ���� �ε�
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
        // Ability Ŭ������ �ִ� FindTarget�� Ȱ���� ���� ������, Bow�� �����̹Ƿ� ��ü������ Ÿ���� ã�� ���� �ڿ�������
        target = FindTarget(); // Bow�� FindTarget�� Ability�� FindTarget���� �� ��ü���� ���� ���� ���� �� ����.

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

    // Bow�� ���� Ÿ���� ã�� ����
    protected new GameObject FindTarget() // 'new' Ű����� Ability�� FindTarget�� ����ϴ�.
    {
        if (player == null) return null;

        GameObject foundTarget = GameObject
            .FindGameObjectsWithTag("Enemy")
            .Where(enemy => enemy != null && enemy.activeInHierarchy && enemy.transform != null)
            .Where(enemy => Vector3.Distance(enemy.transform.position, player.transform.position) < player.AttackRange) // �÷��̾��� ���� ���� ���
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, player.transform.position))
            .FirstOrDefault();
        return foundTarget;
    }

    protected float AttackDelay()
    {
        if (player == null) return 1f;

        // Player�� MaxAttackSpeed�� ����Ͽ� ������ ���
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
            Debug.Log($"{this.GetType().Name}: PerformAttackCycle ���� ������ ���� �̴�. ������ ����մϴ�.");
            StopAttack();
            yield break;
        }

        if (animationHandler != null)
        {
            animationHandler.Attack();
        }
        else
        {
            Debug.LogWarning($"{this.GetType().Name}: AnimationHandler�� �Ҵ���� �ʾ� ���� �ִϸ��̼��� ����� �� �����ϴ�.");
            StopAttack();
            yield break;
        }

        LoadArrow();
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

        if (animator != null)
        {
            animator.SetBool("IsAttack", false);
        }

        if (loadedArrowGO != null)
        {
            ObjectPoolManager.Instance.Return(ARROW_POOL_KEY, loadedArrowGO);
            loadedArrowGO = null;
            loadedArrowScript = null;
            Debug.Log($"{this.GetType().Name}: ������ ȭ���� ������Ʈ Ǯ�� ��ȯ�߽��ϴ�.");
        }
    }

    // �ִϸ��̼� �̺�Ʈ�� ���� ȣ��Ǵ� ȭ�� �߻� �޼���
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

        // �÷��̾��� Ư�� ȭ�� �ɷ� �ߵ� �õ�
        // �̶�, loadedArrowGO�� loadedArrowScript�� Ư�� ȭ�� �ɷ¿� ���� ��ȯ�ǰų� ����� �� �ֽ��ϴ�.
        bool specialArrowFired = player.TryActivateSpecialArrowAbility(loadedArrowGO, loadedArrowScript);

        // Ư�� ȭ���� �߻���� �ʾҴٸ� �Ϲ� ȭ���� �߻��մϴ�.
        if (!specialArrowFired)
        {
            // ȭ���� ���� �߻� ������ Ÿ���� ���ϵ��� ����
            Vector3 finalLaunchDirection = (target.transform.position - firePoint.position).normalized;

            Rigidbody2D arrowRb = loadedArrowScript.GetComponent<Rigidbody2D>();
            if (arrowRb != null)
            {
                arrowRb.isKinematic = false;
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
        // Ư�� ȭ���� �߻�Ǿ��� �ƴϵ�, loadedArrowGO�� ���� å���� �������ϴ�.
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
        // Bow�� "����"�μ� �⺻ ���� ������ ���� �ʴ´ٰ� �����մϴ�.
        // ���� Bow ��ü�� ������ �ְ� �������� ���� ������ ���Ѵٸ� ���⿡ ������ �߰��� �� �ֽ��ϴ�.
        // ������ �Ϲ������� Player.cs���� Bow�� MaxAttackSpeed ���� ���� �����Ͽ� ����մϴ�.
    }
}