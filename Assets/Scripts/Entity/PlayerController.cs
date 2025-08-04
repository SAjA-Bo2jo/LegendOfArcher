using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// �÷��̾� ĳ���͸� �����ϴ� ��Ʈ�ѷ� Ŭ�����Դϴ�.
// �̵�, ȸ��, Ÿ�� Ž��, �ɷ� ��� ���� ó���մϴ�.
public class PlayerController : BaseController
{
    private new Camera camera; // ���� ���� ī�޶� �����ϱ� ���� ����

    // Player.cs���� Ÿ���� Ž���ϹǷ�, ���⼭�� Ÿ���� ������ �մϴ�.
    private GameObject target;

    protected Player player; // �÷��̾� Ŭ���� ���� (ü��, �ӵ� �� ������ ����)

    // �÷��̾ ���������� �ٶ� ���� ������ �����ϴ� �����Դϴ�.
    private Vector2 _lastHorizontalLookDirection = Vector2.right;

    // --- ���� �߰��ǰų� ������ �κ� ---

    // �÷��̾ ���� �Է¿� ���� �����̴� ������ Ȯ���ϴ� ������Ƽ
    public bool IsMoving { get; private set; }

    // --- ���� �ڵ� ---

    protected override void Awake() // ������Ʈ �ʱ�ȭ
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        animationHandler = GetComponent<AnimationHandler>();
    }

    protected override void Start() // ���� ���� �� ȣ��. ī�޶� ���� ����.
    {
        base.Start();
        camera = Camera.main;

        LookDirection = _lastHorizontalLookDirection;
    }

    protected override void Update() // �� �����Ӹ��� ȣ��. �Է� ó�� �� ȸ�� ���� ����.
    {
        HandleInput(); // ����Ű �Է� �� Ÿ�� ����
        Rotate(LookDirection); // ĳ���� ȸ��
    }

    protected override void FixedUpdate() // ���� ���� �ֱ⸶�� ȣ��. �̵� �� �˹� ���� ó��.
    {
        MoveToward(MoveDirection); // ���� �̵� ó��

        if (KnockbackTime > 0.0f)
        {
            KnockbackTime -= Time.fixedDeltaTime;
        }
    }

    // ĳ���͸� Ư�� �������� �̵���Ű�� �޼���.
    protected override void MoveToward(Vector2 direction)
    {
        direction *= player.MoveSpeed; // Player�� MoveSpeed�� ���

        if (KnockbackTime > 0.0f)
        {
            direction *= 0.2f; // �˹� �� �̵� ����
            direction += Knockback; // �˹� ���� ����
        }

        _rigidbody.velocity = direction; // ���� �̵� �ӵ� ����
        animationHandler.Move(direction); // �ִϸ��̼� ó��
    }

    // �Է°��� �޾Ƽ� �̵� ����� �ü� ������ �����մϴ�.
    protected override void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 currentInputDirection = new Vector2(horizontal, vertical);
        MoveDirection = currentInputDirection.normalized;

        IsMoving = currentInputDirection.magnitude > 0.01f;

        target = FindTarget(); // �÷��̾� �ֺ��� ���� ã���ϴ�.

        // 1. Ÿ���� ���� ���: Ÿ�� ������ �ٶ󺾴ϴ�.
        if (target != null)
        {
            LookDirection = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;

            if (Mathf.Abs(LookDirection.x) > 0.01f)
            {
                _lastHorizontalLookDirection = new Vector2(LookDirection.x, 0).normalized;
            }
        }
        // 2. Ÿ���� ����, �÷��̾ �̵� ���� ���:
        else if (IsMoving)
        {
            if (Mathf.Abs(horizontal) > 0.01f)
            {
                LookDirection = currentInputDirection.normalized;
                _lastHorizontalLookDirection = new Vector2(horizontal, 0).normalized;
            }
            else
            {
                LookDirection = _lastHorizontalLookDirection;
            }
        }
        // 3. Ÿ�ٵ� ���� �̵� �ߵ� �ƴ� ��� (������ ���� ��):
        else
        {
            LookDirection = _lastHorizontalLookDirection;
        }
    }

    // ���� Ȱ��ȭ�� �� �߿��� ���� ����� Ÿ���� ��ȯ�մϴ�.
    public GameObject FindTarget()
    {
        GameObject target = GameObject
            .FindGameObjectsWithTag("Enemy")
            .Where(enemy => enemy != null && enemy.activeInHierarchy) // null üũ �߰�
            .Where(enemy => Vector3.Distance(enemy.transform.position, transform.position) < player.AttackRange)
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position))
            .FirstOrDefault();

        return target;
    }

    // �÷��̾�� Ÿ�� ���� �Ÿ� ��� �޼����Դϴ�.
    protected float DistanceToTarget()
    {
        if (target == null) return float.MaxValue;
        return Vector2.Distance(transform.position, target.transform.position);
    }
    // --- ü�� ���� �޼��� ---
    /// <summary>
    /// �÷��̾�� ���ظ� �����ϴ�.
    /// </summary>
    /// <param name="damageAmount">���� ���ط�.</param>
    public void TakeDamage(float damageAmount)
    {
        animationHandler.Damage(); // �ִϸ��̼� �ڵ鷯�� ���� ���� �ִϸ��̼� ����
        float finalDamage = Mathf.Max(0, damageAmount - player.Defense); // ���� ����
        player.Health -= finalDamage;
        Debug.Log($"���ظ� �޾ҽ��ϴ�: {finalDamage}. ���� ü��: {player.Health}");

        if (player.Health <= 0)
        {
            Die(); // �÷��̾� ��� ó��
        }
    }

    /// <summary>
    /// �÷��̾��� ü���� ȸ����ŵ�ϴ�.
    /// </summary>
    /// <param name="healAmount">ȸ���� ü�� ��.</param>
    public void Heal(float healAmount)
    {
        player.Health = Mathf.Min(player.MaxHealth, player.Health + healAmount);
        Debug.Log($"ü�� ȸ��: {healAmount}. ���� ü��: {player.Health}");
    }

    /// <summary>
    /// �÷��̾� ��� ó�� ���� (�ʿ�� ����).
    /// </summary>
    private void Die()
    {
        animationHandler.Death(); // �ִϸ��̼� �ڵ鷯�� ���� ��� �ִϸ��̼� ����
        Debug.Log("�÷��̾ ����߽��ϴ�!");
        // ���� ���� ó��, UI ǥ�� ��
        // Time.timeScale = 0f; // ���� �Ͻ� ���� (����)
    }
}
