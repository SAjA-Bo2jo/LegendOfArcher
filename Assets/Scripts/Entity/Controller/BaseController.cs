using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Rigidbody2D _rigidbody;
    protected AnimationHandler animationHandler;

    [SerializeField] private SpriteRenderer characterRenderer;
    [SerializeField] private Transform weaponPivot;

    protected Vector2 moveDirection = Vector2.zero;                     // moveDirection : 이동하려는 방향
    public Vector2 MoveDirection                                        // -> 참조 시에 MoveDirection
    { get => moveDirection; set => moveDirection = value; }

    protected Vector2 lookDirection = Vector2.zero;                     // lookDirection : 바라보는 방향
    public Vector2 LookDirection                                        // -> 참조 시에 LookDirection
    { get => lookDirection; set => lookDirection = value; }

    private Vector2 knockback = Vector2.zero;                           // knockback : 넉백 방향, 크기
    public Vector2 Knockback                                            // -> 참조 시에 Knockback
    { get => knockback; set => knockback = value; }

    private float knockbackTime = 0.0f;                                 // knockbackTime : 넉백 지속 시간
    public float KnockbackTime                                          // -> 참조 시에 KnockbackTime
    { get => knockbackTime; set => knockbackTime = value; }

    public float moveSpeed = 5.0f;                                      // moveSpeed : 이동속도
    public float MoveSpeed                                              // -> 참조 시에 MoveSpeed
    { get => moveSpeed; set => moveSpeed = value; }

    protected bool isAttacking;
    private float timeSinceLastAttack = float.MaxValue;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        animationHandler = GetComponent<AnimationHandler>();
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
        MoveToward(MoveDirection);                                      // 이동 처리

        if (knockbackTime > 0.0f)
        {
            knockbackTime -= Time.fixedDeltaTime;
        }
    }

    protected virtual void HandleInput()                                // 사용자 입력받음 (적 개체에 붙으면 AI 처리)
    {

    }

    protected virtual void MoveToward(Vector2 direction)                // 단위 벡터만 받아서 이동속도만큼 움직이게 함
    {
        if (this is EnemyController enemy &&
            enemy.Stats != null &&
            enemy.BehaviorType == EnemyBehaviorType.Boss &&
            enemy.EnemyAttack is BossEnemyAttack boss &&
            boss.IsCharging())
        {
            return; // 돌진 중에는 움직임 무시 (velocity 덮어쓰기 방지)
        }

        direction = direction * MoveSpeed;

        if (knockbackTime > 0.0f)
        {
            direction *= 0.2f;
            direction += knockback;
        }

        _rigidbody.velocity = direction;
    }

    protected void Rotate(Vector2 direction)                            // 캐릭터, 무기 방향 처리
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
