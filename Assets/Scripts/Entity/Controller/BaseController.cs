using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Rigidbody2D _rigidbody;
    protected WeaponHandler weaponHandler;
    protected AnimationHandler animationHandler;

    [SerializeField] private SpriteRenderer characterRenderer;
    [SerializeField] private Transform weaponPivot;
    [SerializeField] public WeaponHandler WeaponPrefab;

    protected Vector2 moveDirection = Vector2.zero;                     // moveDirection : �̵��Ϸ��� ����
    public Vector2 MoveDirection                                        // -> ���� �ÿ� MoveDirection
    { get => moveDirection; set => moveDirection = value; }      

    protected Vector2 lookDirection = Vector2.zero;                     // lookDirection : �ٶ󺸴� ����
    public Vector2 LookDirection                                        // -> ���� �ÿ� LookDirection
    { get => lookDirection; set => lookDirection = value; }      

    private Vector2 knockback = Vector2.zero;                           // knockback : �˹� ����, ũ��
    public Vector2 Knockback                                            // -> ���� �ÿ� Knockback
    { get => knockback; set => knockback = value; }

    private float knockbackTime = 0.0f;                                 // knockbackTime : �˹� ���� �ð�
    public float KnockbackTime                                          // -> ���� �ÿ� KnockbackTime
    { get => knockbackTime; set => knockbackTime = value; }

    public float moveSpeed = 5.0f;                                      // moveSpeed : �̵��ӵ�
    public float MoveSpeed                                              // -> ���� �ÿ� MoveSpeed
    { get => moveSpeed; set => moveSpeed = value; }

    protected bool isAttacking;
    private float timeSinceLastAttack = float.MaxValue;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        animationHandler = GetComponent<AnimationHandler>();

        if (WeaponPrefab != null)
        {
            weaponHandler = Instantiate(WeaponPrefab, weaponPivot);     // WeaponPrefeb ������ WeaponPivot �ڸ��� ����
        }
        else
        {
            weaponHandler = GetComponentInChildren<WeaponHandler>();    // ������ �ڽ����� ��ġ�� weaponHandler ���
        }
    }
    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        HandleInput();

        Rotate(lookDirection);

        HandleAttackDelay();
    }

    protected virtual void FixedUpdate()
    {
        MoveToward(MoveDirection);                                      // �̵� ó��

        if (knockbackTime > 0.0f)
        {
            knockbackTime -= Time.fixedDeltaTime;
        }
    }

    protected virtual void HandleInput()                                // ����� �Է¹��� (�� ��ü�� ������ AI ó��)
    {

    }

    protected virtual void MoveToward(Vector2 direction)                // ���� ���͸� �޾Ƽ� �̵��ӵ���ŭ �����̰� ��
    {
        direction = direction * MoveSpeed;                              

        if (knockbackTime > 0.0f)
        {
            direction *= 0.2f;
            direction += knockback;
        }

        _rigidbody.velocity = direction;
    }

    protected void Rotate(Vector2 direction)                            // ĳ����, ���� ���� ó��
    {
        float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        bool isLeft = Mathf.Abs(rotationAngle) > 90f;

        characterRenderer.flipX = isLeft;

        if (weaponPivot != null)
        {
            weaponPivot.rotation = Quaternion.Euler(0, 0, rotationAngle);
        }

        weaponHandler?.Rotate(isLeft);
    }

    private void HandleAttackDelay()                                    // ���� ������ ���ϴ� �޼���
    {
        if (weaponHandler == null) return;                              // ���� ������ ���� X.

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

    protected virtual void Attack()                                     // ���� ���� �޼���
    {
        if (lookDirection != Vector2.zero)
        {
            weaponHandler?.Attack();                                    // ��Ȯ�� ������ �־�� ����
        }
    }

    public void ApplyKnockback(Transform otherObject, float power, float duration)
    {
        knockbackTime = duration;

        knockback = -(otherObject.position - transform.position).normalized * power;
    }
}
