using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Rigidbody2D _rigidbody;

    [SerializeField] private SpriteRenderer characterRenderer;
    [SerializeField] private Transform weaponPivot;

    protected Vector2 moveDirection = Vector2.zero;                     // moveDirection : �̵��Ϸ��� ����
    public Vector2 MoveDirection                                        // -> ���� �ÿ� MoveDirection
    {
        get { return moveDirection; }
        set { moveDirection = value; }
    }      

    protected Vector2 lookDirection = Vector2.zero;                     // lookDirection : �ٶ󺸴� ����
    public Vector2 LookDirection { get { return lookDirection; } }      // -> ���� �ÿ� LookDirection

    private Vector2 knockback = Vector2.zero;                           // knockback : �˹� ����, ũ��
    public Vector2 Knockback { get { return knockback; } }

    private float knockbackTime = 0.0f;                                 // knockbackTime : �˹� ���� �ð�
    public float KnockbackTime {get { return knockbackTime; } }

    public float moveSpeed = 5.0f;                                      // moveSpeed : �̵��ӵ�

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        HandleInput();

        Rotate(lookDirection);
    }

    protected virtual void FixedUpdate()
    {
        MoveToward(MoveDirection);

        if (knockbackTime > 0.0f)
        {
            knockbackTime -= Time.fixedDeltaTime;
        }
    }

    protected virtual void HandleInput()                                // ����� �Է¹��� (�� ��ü�� ������ AI ó��)
    {

    }

    protected virtual void MoveToward(Vector2 direction)                          // ���� ���͸� �޾Ƽ� �̵��ӵ���ŭ �����̰� ��
    {
        direction = direction * moveSpeed;                              

        if (knockbackTime > 0.0f)
        {
            direction *= 0.2f;
            direction += knockback;
        }

        _rigidbody.velocity = direction;
    }

    private void Rotate(Vector2 direction)                              // ĳ����, ���� ���� ó��
    {
        float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        bool isLeft = Mathf.Abs(rotationAngle) > 90f;

        characterRenderer.flipX = isLeft;

        if (weaponPivot != null)
        {
            weaponPivot.rotation = Quaternion.Euler(0, 0, rotationAngle);
        }
    }

    public void ApplyKnockback(Transform otherObject, float power, float duration)
    {
        knockbackTime = duration;

        knockback = -(otherObject.position - transform.position).normalized * power;
    }
}
