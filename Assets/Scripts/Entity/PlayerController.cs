using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// �÷��̾� ĳ���͸� �����ϴ� ��Ʈ�ѷ� Ŭ�����Դϴ�.
// �̵�, ȸ��, Ÿ�� Ž��, �ɷ� ��� ���� ó���մϴ�.
public class PlayerController : BaseController
{
    private new Camera camera;    // ���� ���� ī�޶� �����ϱ� ���� ����

    private GameObject target;    // Ÿ���� �Ǵ� �� ������Ʈ

    [SerializeField] public Abillity AbillityPrefab;    // �ɷ� �������� �����Ϳ��� ���� �����ϰ� ����

    protected Abillity abillity;    // �÷��̾ ������ �ɷ� ������Ʈ

    protected Player player;    // �÷��̾� Ŭ���� ���� (ü��, �ӵ� �� ������ ����)

    // �÷��̾ ���������� �ٶ� ���� ������ �����ϴ� �����Դϴ�.
    private Vector2 lastMoveDirection = Vector2.right;

    protected override void Awake()    // ������Ʈ �ʱ�ȭ. Rigidbody, Player, AnimationHandler, Abillity ���� �����ɴϴ�.
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        animationHandler = GetComponent<AnimationHandler>();
        abillity = GetComponentInChildren<Abillity>();
    }

    // ���� ���� �� ȣ��. ī�޶� ���� ����.
    protected override void Start()
    {
        base.Start();
        camera = Camera.main;
    }

    protected override void Update()    // �� �����Ӹ��� ȣ��. �Է� ó�� �� ȸ�� ���� ����.
    {
        HandleInput();            // ����Ű �Է� �� Ÿ�� ����
        Rotate(lookDirection);    // ĳ���� ȸ��
    }

    protected override void FixedUpdate()    // ���� ���� �ֱ⸶�� ȣ��. �̵� �� �˹� ���� ó��.
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
        direction *= player.Speed;    // �⺻ �ӵ� �ݿ�

        if (KnockbackTime > 0.0f)
        {
            direction *= 0.2f;        // �˹� �� �̵� ����
            direction += Knockback;    // �˹� ���� ����
        }

        _rigidbody.velocity = direction;    // ���� �̵� �ӵ� ����
        animationHandler.Move(direction);    // �ִϸ��̼� ó��
    }

    protected override void HandleInput()    // �Է°��� �޾Ƽ� �̵� ����� �ü� ������ �����մϴ�.
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 currentInputDirection = new Vector2(horizontal, vertical);
        MoveDirection = currentInputDirection.normalized;

        target = FindTarget();

        // 1. Ÿ���� ���� ���: Ÿ�� ������ �ٶ󺾴ϴ�.
        if (target != null)
        {
            lookDirection = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
            // Ÿ���� �ٶ� ���� ������ ���� ������ �����Ͽ�, Ÿ���� ������� �� ������ �ٶ󺸰� �մϴ�.
            if (Mathf.Abs(lookDirection.x) > 0.01f)
            {
                lastMoveDirection = new Vector2(lookDirection.x, 0).normalized;
            }
        }
        // 2. Ÿ���� ����, �÷��̾ �̵� ���� ���:
        else if (currentInputDirection.magnitude > 0.01f)
        {
            // ���� �Է��� ���� ���� lookDirection�� �̵� �������� �����ϰ� lastMoveDirection�� �����մϴ�.
            if (Mathf.Abs(horizontal) > 0.01f)
            {
                lookDirection = currentInputDirection.normalized;
                lastMoveDirection = new Vector2(horizontal, 0).normalized;
            }
            // ���� �Է� ���� �������θ� �̵� ���� ��:
            else
            {
                // ���������� �ٶ� ���� ������ �����մϴ�.
                lookDirection = lastMoveDirection;
            }
        }
        // 3. Ÿ�ٵ� ���� �̵� �ߵ� �ƴ� ��� (������ ���� ��):
        else
        {
            // ���������� �̵��ߴ� ���� ������ �ٶ󺾴ϴ�.
            lookDirection = lastMoveDirection;
        }
    }

    public GameObject FindTarget()    // ���� Ȱ��ȭ�� �� �߿��� ���� ����� Ÿ���� ��ȯ�մϴ�.
    {
        GameObject target = GameObject
            .FindGameObjectsWithTag("Enemy")
            .Where(enemy => enemy.activeInHierarchy)
            .Where(enemy => Vector3.Distance(enemy.transform.position, transform.position) < player.AttackRange)
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position))
            .FirstOrDefault();

        return target;
    }

    protected float DistanceToTarget()    // �÷��̾�� Ÿ�� ���� �Ÿ� ��� �޼����Դϴ�.
    {
        if (target == null) return float.MaxValue;
        return Vector2.Distance(transform.position, target.transform.position);
    }
}
