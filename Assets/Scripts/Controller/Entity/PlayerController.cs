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


    protected AnimationHandler animationHandler;    // �ִϸ��̼��� �����ϴ� �ڵ鷯


    protected Player player;    // �÷��̾� Ŭ���� ���� (ü��, �ӵ� �� ������ ����)


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
        HandleInput();              // ����Ű �Է� �� Ÿ�� ����
        Rotate(lookDirection);      // ĳ���� ȸ��
    }

    protected override void FixedUpdate()    // ���� ���� �ֱ⸶�� ȣ��. �̵� �� �˹� ���� ó��.
    {
        MoveToward(MoveDirection);  // ���� �̵� ó��

        if (knockbackTime > 0.0f)
        {
            knockbackTime -= Time.fixedDeltaTime;
        }
    }

    // ĳ���͸� Ư�� �������� �̵���Ű�� �޼���.
    // �˹� ���¶�� �̵� �ӵ� ���� �� �˹� ���� �߰�.
    protected override void MoveToward(Vector2 direction)
    {
        direction *= player.Speed;      // �⺻ �ӵ� �ݿ�

        if (KnockbackTime > 0.0f)
        {
            direction *= 0.2f;          // �˹� �� �̵� ����
            direction += Knockback;     // �˹� ���� ����
        }

        _rigidbody.velocity = direction;    // ���� �̵� �ӵ� ����
        animationHandler.Move(direction);   // �ִϸ��̼� ó��
    }

    protected override void HandleInput()    // �Է°��� �޾Ƽ� �̵� ����� �ü� ������ �����մϴ�.
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        MoveDirection = new Vector2(horizontal, vertical).normalized;        // �̵� ���� ���� (����ȭ�� ����)

        target = FindTarget();        // ���� ����� Ÿ�� Ž��

        if (target == null)        //  Ÿ���� ������ ȸ������ �ʵ��� lookDirection�� 0���� ����
        {
            lookDirection = Vector2.zero;
            return;
        }

        lookDirection = ((Vector2)target.transform.position - (Vector2)player.transform.position);        // Ÿ�� ���� ���

        if (lookDirection.sqrMagnitude < 0.81f)        // �ʹ� ������ ȸ������ ����
            lookDirection = Vector2.zero;
        else
            lookDirection = lookDirection.normalized;
    }

    public GameObject FindTarget()    // ���� Ȱ��ȭ�� �� �߿��� ���� ����� Ÿ���� ��ȯ�մϴ�.
    {
        GameObject target = GameObject
            .FindGameObjectsWithTag("Enemy")                                        // ��� �� ��������
            .Where(enemy => enemy.activeInHierarchy)                               // Ȱ��ȭ�� �͸�
            .Where(enemy => Vector3.Distance(enemy.transform.position, transform.position) < player.AttackRange) // �����Ÿ� ��
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position)) // �Ÿ� �� ����
            .FirstOrDefault();                                                     // ���� ����� �� ����

        return target;
    }

    protected float DistanceToTarget()    // �÷��̾�� Ÿ�� ���� �Ÿ� ��� �޼����Դϴ�.
    {
        // target�� null�� ���ɼ��� HandleInput���� ó��������, �����ϰ� null üũ�ص� �����ϴ�.
        if (target == null) return float.MaxValue;

        return Vector2.Distance(transform.position, target.transform.position);
    }
}
