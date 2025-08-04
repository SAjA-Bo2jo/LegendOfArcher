using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// �÷��̾� ĳ���͸� �����ϴ� ��Ʈ�ѷ� Ŭ�����Դϴ�.
// �̵�, ȸ��, Ÿ�� Ž��, �ɷ� ��� ���� ó���մϴ�.
public class PlayerController : BaseController
{
    // === ����ȭ�� �ʵ� (�ν����Ϳ��� ����) ===
    // �÷��̾��� �ִϸ��̼� �ڵ鷯 ������Ʈ ����.
    // �� �ʵ�� PlayerController�� �ٸ� GameObject�� �ִ� AnimationHandler��
    // Unity �����Ϳ��� ���� �巡�� �� ������� �����ϱ� ���� ���˴ϴ�.
    [SerializeField] private AnimationHandler playerAnimationHandler;

    // === ���� ���� ���� ===
    private new Camera camera; // ���� ���� ī�޶� �����ϱ� ���� ����

    // Player.cs���� Ÿ���� Ž���ϹǷ�, ���⼭�� Ÿ���� ������ �մϴ�.
    private GameObject target;

    protected Player player; // �÷��̾� Ŭ���� ���� (ü��, �ӵ� �� ������ ����)

    // �÷��̾ ���������� �ٶ� ���� ������ �����ϴ� �����Դϴ�.
    private Vector2 _lastHorizontalLookDirection = Vector2.right;

    // �÷��̾ ���� �Է¿� ���� �����̴� ������ Ȯ���ϴ� ������Ƽ
    public bool IsMoving { get; private set; }

    // === Unity �����ֱ� �޼��� ===

    // ������Ʈ�� ó�� Ȱ��ȭ�� �� �� �� ȣ��˴ϴ�.
    // �ַ� ������Ʈ ������ �������� �� ���˴ϴ�.
    protected override void Awake() // ������Ʈ �ʱ�ȭ
    {
        // BaseController�� Awake ȣ�� (BaseController�� _rigidbody �ʱ�ȭ�� �ִٸ� �ߺ� ���ϱ�)
        // ���� �ڵ忡���� _rigidbody = GetComponent<Rigidbody2D>(); �� ���� �ʱ�ȭ�ϰ� �����Ƿ� base.Awake()�� ���� ȣ��Ǿ ����
        // ���� BaseController�� _rigidbody�� �ʱ�ȭ�Ѵٸ�, �� ���� base.Awake(); �Ʒ��� �ű� �� �ֽ��ϴ�.
        _rigidbody = GetComponent<Rigidbody2D>(); // �� PlayerController ��ũ��Ʈ�� �پ��ִ� GameObject���� Rigidbody2D�� ã���ϴ�.

        // Player ������Ʈ�� ���� GameObject�� ���� ������ �����մϴ�.
        player = GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("PlayerController: Player ������Ʈ�� ã�� �� �����ϴ�! PlayerController�� ���� GameObject�� Player ��ũ��Ʈ�� �ִ��� Ȯ���ϼ���.");
        }

        // --- �ٽ� ���� ����: AnimationHandler�� �� �̻� ���⼭ GetComponent<T>()�� ã�� �ʽ��ϴ�. ---
        // animationHandler = GetComponent<AnimationHandler>(); // �� ���� �����߽��ϴ�.
        // ���� playerAnimationHandler �ʵ带 ���� �ν����Ϳ��� ������ ���Դϴ�.
    }

    // ù ��° ������ ������Ʈ ������ ȣ��˴ϴ�.
    // Awake ���Ŀ� �ʱ�ȭ�Ǿ�� �ϴ� ������ ó���մϴ�.
    protected override void Start() // ���� ���� �� ȣ��. ī�޶� ���� ����.
    {
        base.Start(); // BaseController�� Start ȣ��
        camera = Camera.main;

        // �÷��̾ ó�� ������ �� �ٶ󺸴� ������ �����մϴ�.
        LookDirection = _lastHorizontalLookDirection;

        // --- �ٽ� �߰� ����: playerAnimationHandler�� �ν����Ϳ��� ����� ����Ǿ����� Ȯ�� ---
        if (playerAnimationHandler == null)
        {
            Debug.LogError("PlayerController: Start() ������ playerAnimationHandler�� NULL�Դϴ�. Unity �ν����Ϳ��� �÷��̾� ���� ��������Ʈ�� AnimationHandler�� �� �ʵ忡 �巡�� �� ����Ͽ� �����ؾ� �մϴ�.");
        }
        else
        {
            Debug.Log("PlayerController: Start() ������ playerAnimationHandler�� ���������� ����Ǿ����ϴ�: " + playerAnimationHandler.gameObject.name);
        }
    }

    // �� �����Ӹ��� ȣ��˴ϴ�. �Է� ó�� �� ȸ�� ������ �����մϴ�.
    protected override void Update() // �� �����Ӹ��� ȣ��. �Է� ó�� �� ȸ�� ���� ����.
    {
        HandleInput(); // ����Ű �Է� �� Ÿ�� ����
        Rotate(LookDirection); // ĳ���� ȸ�� (BaseController�� �޼���)
    }

    // ���� ���� �ֱ⸶�� ȣ��˴ϴ�. �̵� �� �˹� ������ ó���մϴ�.
    protected override void FixedUpdate() // ���� ���� �ֱ⸶�� ȣ��. �̵� �� �˹� ���� ó��.
    {
        MoveToward(MoveDirection); // ���� �̵� ó�� (�� �޼��� ������ �ִϸ��̼� �ڵ鷯 ȣ��)

        // �˹� �ð� ó�� (�˹� ���� ���� �̵� �ӵ��� ������ �ݴϴ�)
        if (KnockbackTime > 0.0f)
        {
            KnockbackTime -= Time.fixedDeltaTime;
        }
    }

    // === �̵� ���� �޼��� ===

    // ĳ���͸� Ư�� �������� �̵���Ű�� �޼���.
    // FixedUpdate���� ���� MoveDirection�� ���ڷ� �޾� ȣ��˴ϴ�.
    protected override void MoveToward(Vector2 direction)
    {
        // �÷��̾��� ���� �̵� �ӵ��� ���Ͽ� ���� �̵� ���͸� ����մϴ�.
        // player ������ null�� �ƴ� ���� �����ϵ��� null üũ�� �����ϴ� ���� �����մϴ�.
        if (player != null)
        {
            direction *= player.MoveSpeed; // Player�� MoveSpeed�� ���
        }
        else
        {
            Debug.LogWarning("PlayerController: Player ��ũ��Ʈ�� ���� �̵� �ӵ��� ������ �� �����ϴ�.");
            // �⺻ �̵� �ӵ��� �����ϰų�, ������ ���� �� �ֽ��ϴ�.
            // direction *= 5.0f; // ����: �⺻ �ӵ� ����
        }

        // �˹� ���� �� �̵��� �����ϰ� �˹� ���͸� �߰��մϴ�.
        if (KnockbackTime > 0.0f)
        {
            direction *= 0.2f; // �˹� �� �̵� ����
            direction += Knockback; // �˹� ���� ���� (BaseController�� Knockback)
        }

        // Rigidbody2D�� velocity�� ���� �����Ͽ� �������� �̵��� ó���մϴ�.
        // _rigidbody ������ null�� �ƴ� ���� �����ϵ��� null üũ�� �����ϴ� ���� �����մϴ�.
        if (_rigidbody != null)
        {
            _rigidbody.velocity = direction; // ���� �̵� �ӵ� ����
        }
        else
        {
            Debug.LogError("PlayerController: Rigidbody2D�� �Ҵ���� �ʾ� �̵��� �� �����ϴ�!");
        }

        // --- �ٽ� ���� ����: playerAnimationHandler�� ���� �ִϸ��̼� ó�� ---
        // playerAnimationHandler�� null�� �ƴ��� �ݵ�� Ȯ���ؾ� NullReferenceException�� ������ �� �ֽ��ϴ�.
        if (playerAnimationHandler != null)
        {
            playerAnimationHandler.Move(direction); // �ִϸ��̼� �ڵ鷯�� Move �޼��� ȣ��
        }
        else
        {
            // �� ���� �޽����� Start()������ ��µǹǷ�, ���⼭ �� ������ ��µǴ� ���� ���� �� �ֽ��ϴ�.
            // Debug.LogError("PlayerController: playerAnimationHandler�� �Ҵ���� �ʾ� �ִϸ��̼��� ������ �� �����ϴ�.");
            // ������ ù ������ ���Ŀ��� ��� null�̸� ������ �����Ƿ� ������ ���� ������ �� �ֽ��ϴ�.
        }
    }

    // === �Է� ó�� �޼��� ===

    // �Է°��� �޾Ƽ� �̵� ����� �ü� ������ �����մϴ�.
    protected override void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 currentInputDirection = new Vector2(horizontal, vertical);
        MoveDirection = currentInputDirection.normalized; // BaseController�� MoveDirection ������Ƽ

        // �Է��� ũ�Ⱑ 0.01f���� ũ�� �����̴� ������ �����մϴ�.
        IsMoving = currentInputDirection.magnitude > 0.01f;

        target = FindTarget(); // �÷��̾� �ֺ��� ���� ã���ϴ�.

        // 1. Ÿ���� ���� ���: Ÿ�� ������ �ٶ󺾴ϴ�.
        if (target != null)
        {
            LookDirection = ((Vector2)target.transform.position - (Vector2)transform.position).normalized; // BaseController�� LookDirection ������Ƽ

            // ���� ���⸸ ���� �����Ͽ�, ���� �̵� �ÿ��� ���� ������ �����ϵ��� �մϴ�.
            if (Mathf.Abs(LookDirection.x) > 0.01f)
            {
                _lastHorizontalLookDirection = new Vector2(LookDirection.x, 0).normalized;
            }
        }
        // 2. Ÿ���� ����, �÷��̾ �̵� ���� ���:
        else if (IsMoving)
        {
            // ���� �Է��� ���� ���� LookDirection�� ���� �̵� �������� �����ϰ� _lastHorizontalLookDirection�� ������Ʈ�մϴ�.
            if (Mathf.Abs(horizontal) > 0.01f)
            {
                LookDirection = currentInputDirection.normalized;
                _lastHorizontalLookDirection = new Vector2(horizontal, 0).normalized;
            }
            else
            {
                // ���� �Է��� ������ ���������� �ٶ� ���� ������ �����մϴ�.
                LookDirection = _lastHorizontalLookDirection;
            }
        }
        // 3. Ÿ�ٵ� ���� �̵� �ߵ� �ƴ� ��� (������ ���� ��):
        else
        {
            // ������ ���� ���� ���������� �ٶ� ���� ������ �����մϴ�.
            LookDirection = _lastHorizontalLookDirection;
        }
    }

    // === Ÿ�� Ž�� �޼��� ===

    // ���� Ȱ��ȭ�� �� �߿��� ���� ����� Ÿ���� ��ȯ�մϴ�.
    public GameObject FindTarget()
    {
        // "Enemy" �±׸� ���� ��� GameObject�� ã���ϴ�.
        GameObject target = GameObject
            .FindGameObjectsWithTag("Enemy")
            // null�� �ƴϰ� Ȱ��ȭ�� GameObject�� ���͸��մϴ�.
            .Where(enemy => enemy != null && enemy.activeInHierarchy)
            // �÷��̾��� AttackRange ���� �ִ� ���� ���͸��մϴ�.
            // player ������ null�� �ƴ� ���� AttackRange�� ����մϴ�.
            .Where(enemy => player != null ? Vector3.Distance(enemy.transform.position, transform.position) < player.AttackRange : false)
            // �÷��̾�κ����� �Ÿ��� ���� ������������ �����մϴ�.
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position))
            // ���� ù ��°(���� �����) ���� ��ȯ�մϴ�. ������ null�� ��ȯ�մϴ�.
            .FirstOrDefault();

        return target;
    }

    // === ��ƿ��Ƽ �޼��� ===

    // �÷��̾�� Ÿ�� ���� �Ÿ� ��� �޼����Դϴ�.
    protected float DistanceToTarget()
    {
        if (target == null) return float.MaxValue; // Ÿ���� ������ ���Ѵ� ��ȯ
        return Vector2.Distance(transform.position, target.transform.position);
    }
}