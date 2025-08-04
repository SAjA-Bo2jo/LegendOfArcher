using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// �÷��̾� ĳ���͸� �����ϴ� ��Ʈ�ѷ� Ŭ�����Դϴ�.
// �̵�, ȸ��, Ÿ�� Ž��, �ɷ� ��� ���� ó���մϴ�.
public class PlayerController : BaseController
{
    private new Camera camera;   // ���� ���� ī�޶� �����ϱ� ���� ����

    private GameObject target;   // Ÿ���� �Ǵ� �� ������Ʈ

    [SerializeField] public Abillity AbillityPrefab;   // �ɷ� �������� �����Ϳ��� ���� �����ϰ� ����

    protected Abillity abillity;   // �÷��̾ ������ �ɷ� ������Ʈ

    protected Player player;   // �÷��̾� Ŭ���� ���� (ü��, �ӵ� �� ������ ����)

    // �÷��̾ ���������� �ٶ� ���� ������ �����ϴ� �����Դϴ�.
    // BaseController�� LookDirection�� �����Ǿ� ���ǹǷ�, �ʵ���� BaseController�� ���еǰ� �����߽��ϴ�.
    private Vector2 _lastHorizontalLookDirection = Vector2.right;

    // --- ���� �߰��ǰų� ������ �κ� ---

    // �÷��̾ ���� �Է¿� ���� �����̴� ������ Ȯ���ϴ� ������Ƽ
    public bool IsMoving { get; private set; }

    // --- ���� �ڵ� ---

    protected override void Awake()   // ������Ʈ �ʱ�ȭ. Rigidbody, Player, AnimationHandler, Abillity ���� �����ɴϴ�.
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        animationHandler = GetComponent<AnimationHandler>();
        abillity = GetComponentInChildren<Abillity>(); // PlayerController�� Abillity�� ���� �����մϴ�.
                                                       // BaseController�� weaponHandler�ʹ� �����Դϴ�.
    }

    // ���� ���� �� ȣ��. ī�޶� ���� ����.
    protected override void Start()
    {
        base.Start();
        camera = Camera.main;

        // �÷��̾��� �ʱ� LookDirection ���� (BaseController�� LookDirection ������Ƽ ���)
        LookDirection = _lastHorizontalLookDirection;
    }

    protected override void Update()   // �� �����Ӹ��� ȣ��. �Է� ó�� �� ȸ�� ���� ����.
    {
        HandleInput();           // ����Ű �Է� �� Ÿ�� ����
        Rotate(LookDirection);   // ĳ���� ȸ�� (BaseController�� LookDirection ������Ƽ ���)
    }

    protected override void FixedUpdate()   // ���� ���� �ֱ⸶�� ȣ��. �̵� �� �˹� ���� ó��.
    {
        MoveToward(MoveDirection); // ���� �̵� ó��

        if (KnockbackTime > 0.0f)
        {
            KnockbackTime -= Time.fixedDeltaTime;
        }
    }

    // ĳ���͸� Ư�� �������� �̵���Ű�� �޼���.
    // �˹� ���¶�� �̵� �ӵ� ���� �� �˹� ���� �߰�.
    protected override void MoveToward(Vector2 direction)
    {
        // BaseController�� moveSpeed ��� Player�� Speed�� ����մϴ�.
        direction *= player.Speed;   // �⺻ �ӵ� �ݿ�

        if (KnockbackTime > 0.0f)
        {
            direction *= 0.2f;       // �˹� �� �̵� ����
            direction += Knockback;    // �˹� ���� ����
        }

        _rigidbody.velocity = direction;   // ���� �̵� �ӵ� ����
        animationHandler.Move(direction);  // �ִϸ��̼� ó��
    }

    // �Է°��� �޾Ƽ� �̵� ����� �ü� ������ �����մϴ�.
    protected override void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 currentInputDirection = new Vector2(horizontal, vertical);
        MoveDirection = currentInputDirection.normalized; // BaseController�� MoveDirection ������Ʈ

        // �÷��̾ �̵� �Է��� �޾Ҵ��� Ȯ���Ͽ� IsMoving ������Ƽ ������Ʈ
        IsMoving = currentInputDirection.magnitude > 0.01f;

        target = FindTarget(); // �÷��̾� �ֺ��� ���� ã���ϴ�.

        // 1. Ÿ���� ���� ���: Ÿ�� ������ �ٶ󺾴ϴ�.
        if (target != null)
        {
            // BaseController�� LookDirection ������Ƽ�� ������Ʈ�մϴ�.
            LookDirection = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;

            // Ÿ���� �ٶ� ���� ������ ���� ������ �����Ͽ�, Ÿ���� ������� �� ������ �ٶ󺸰� �մϴ�.
            if (Mathf.Abs(LookDirection.x) > 0.01f)
            {
                _lastHorizontalLookDirection = new Vector2(LookDirection.x, 0).normalized;
            }
        }
        // 2. Ÿ���� ����, �÷��̾ �̵� ���� ���:
        else if (IsMoving) // ���� IsMoving ������Ƽ�� ���
        {
            // ���� �Է��� ���� ���� LookDirection�� �̵� �������� �����ϰ� _lastHorizontalLookDirection�� �����մϴ�.
            if (Mathf.Abs(horizontal) > 0.01f)
            {
                LookDirection = currentInputDirection.normalized;
                _lastHorizontalLookDirection = new Vector2(horizontal, 0).normalized;
            }
            // ���� �Է� ���� �������θ� �̵� ���� ��:
            else
            {
                // ���������� �ٶ� ���� ������ �����մϴ�.
                LookDirection = _lastHorizontalLookDirection;
            }
        }
        // 3. Ÿ�ٵ� ���� �̵� �ߵ� �ƴ� ��� (������ ���� ��):
        else
        {
            // ���������� �̵��ߴ� ���� ������ �ٶ󺾴ϴ�.
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
}