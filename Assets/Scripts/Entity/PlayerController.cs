using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    private new Camera camera;

    private Transform target;

    [SerializeField] private float detectRange = 15f;
    [SerializeField] private float attackRange = 6f;
    [SerializeField] public WeaponHandler WeaponPrefab;
    protected WeaponHandler weaponHandler;
    protected AnimationHandler animationHandler;
    protected StatManager statManager;

    protected bool isAttacking;
    private float timeSinceLastAttack = float.MaxValue;

    protected override void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        statManager = GetComponent<StatManager>();
        animationHandler = GetComponent<AnimationHandler>();
        weaponHandler = GetComponentInChildren<WeaponHandler>();
    }

    protected override void Start()
    {
        base.Start();
        camera = Camera.main;
    }

    protected override void MoveToward(Vector2 direction)
    {
        direction = direction * statManager.Speed;
        if (KnockbackTime > 0.0f)
        {
            direction *= 0.2f;
            direction += Knockback;
        }

        _rigidbody.velocity = direction;
        animationHandler.Move(direction);
    }

    protected override void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        MoveDirection = new Vector2(horizontal, vertical).normalized;

        Vector2 mousePosition = Input.mousePosition;
        Vector2 worldPos = camera.ScreenToWorldPoint(mousePosition);
        lookDirection = (worldPos - (Vector2)transform.position);

        if (lookDirection.magnitude < .9f)
        {
            lookDirection = Vector2.zero;
        }
        else
        {
            lookDirection = lookDirection.normalized;
        }

        isAttacking = Input.GetMouseButton(0);
    }
    private void HandleAttackDelay()
    {
        if (weaponHandler == null)
            return;

        if (timeSinceLastAttack <= weaponHandler.Delay)
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        if (isAttacking && timeSinceLastAttack > weaponHandler.Delay)
        {
            timeSinceLastAttack = 0;
            Attack();
        }
    }

    protected virtual void Attack()
    {
        if (lookDirection != Vector2.zero)
            weaponHandler?.Attack();
    }

    public void Init(Transform target)                                      // ���� ��� ���ϴ� �޼���
    {
        this.target = target;
    }

    protected float DistanceToTarget()                                      // �÷��̾� ~ �� �Ÿ� ���ϴ� �޼���
    {
        return Vector2.Distance(transform.position, target.position);
    }

    protected Vector2 DirectionToTarget()                                   // �� -> �÷��̾� �������� ����
    {
        return (target.position - transform.position).normalized;
    }

    public bool IsInAttackRange()                                           // ���� ��Ÿ� ���� �÷��̾� �ִ� �� Ȯ��
    {
        if (target == null) return false;
        return DistanceToTarget() <= attackRange;
    }
}