using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// 플레이어 캐릭터를 제어하는 컨트롤러 클래스입니다.
// 이동, 회전, 타겟 탐색, 능력 사용 등을 처리합니다.
public class PlayerController : BaseController
{
    private new Camera camera; // 현재 메인 카메라를 참조하기 위한 변수

    // Player.cs에서 타겟을 탐색하므로, 여기서는 타겟을 보관만 합니다.
    private GameObject target;

    protected Player player; // 플레이어 클래스 참조 (체력, 속도 등 데이터 보관)

    // 플레이어가 마지막으로 바라본 수평 방향을 저장하는 변수입니다.
    private Vector2 _lastHorizontalLookDirection = Vector2.right;

    // --- 새로 추가되거나 수정된 부분 ---

    // 플레이어가 현재 입력에 의해 움직이는 중인지 확인하는 프로퍼티
    public bool IsMoving { get; private set; }

    // --- 기존 코드 ---

    protected override void Awake() // 컴포넌트 초기화
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        animationHandler = GetComponent<AnimationHandler>();
    }

    protected override void Start() // 게임 시작 시 호출. 카메라 설정 포함.
    {
        base.Start();
        camera = Camera.main;

        LookDirection = _lastHorizontalLookDirection;
    }

    protected override void Update() // 매 프레임마다 호출. 입력 처리 및 회전 로직 수행.
    {
        HandleInput(); // 방향키 입력 및 타겟 설정
        Rotate(LookDirection); // 캐릭터 회전
    }

    protected override void FixedUpdate() // 물리 연산 주기마다 호출. 이동 및 넉백 적용 처리.
    {
        MoveToward(MoveDirection); // 실제 이동 처리

        if (KnockbackTime > 0.0f)
        {
            KnockbackTime -= Time.fixedDeltaTime;
        }
    }

    // 캐릭터를 특정 방향으로 이동시키는 메서드.
    protected override void MoveToward(Vector2 direction)
    {
        direction *= player.MoveSpeed; // Player의 MoveSpeed를 사용

        if (KnockbackTime > 0.0f)
        {
            direction *= 0.2f; // 넉백 시 이동 감속
            direction += Knockback; // 넉백 방향 적용
        }

        _rigidbody.velocity = direction; // 최종 이동 속도 적용
        animationHandler.Move(direction); // 애니메이션 처리
    }

    // 입력값을 받아서 이동 방향과 시선 방향을 설정합니다.
    protected override void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 currentInputDirection = new Vector2(horizontal, vertical);
        MoveDirection = currentInputDirection.normalized;

        IsMoving = currentInputDirection.magnitude > 0.01f;

        target = FindTarget(); // 플레이어 주변의 적을 찾습니다.

        // 1. 타겟이 있을 경우: 타겟 방향을 바라봅니다.
        if (target != null)
        {
            LookDirection = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;

            if (Mathf.Abs(LookDirection.x) > 0.01f)
            {
                _lastHorizontalLookDirection = new Vector2(LookDirection.x, 0).normalized;
            }
        }
        // 2. 타겟은 없고, 플레이어가 이동 중일 경우:
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
        // 3. 타겟도 없고 이동 중도 아닐 경우 (가만히 있을 때):
        else
        {
            LookDirection = _lastHorizontalLookDirection;
        }
    }

    // 현재 활성화된 적 중에서 가장 가까운 타겟을 반환합니다.
    public GameObject FindTarget()
    {
        GameObject target = GameObject
            .FindGameObjectsWithTag("Enemy")
            .Where(enemy => enemy != null && enemy.activeInHierarchy) // null 체크 추가
            .Where(enemy => Vector3.Distance(enemy.transform.position, transform.position) < player.AttackRange)
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position))
            .FirstOrDefault();

        return target;
    }

    // 플레이어와 타겟 간의 거리 계산 메서드입니다.
    protected float DistanceToTarget()
    {
        if (target == null) return float.MaxValue;
        return Vector2.Distance(transform.position, target.transform.position);
    }
    // --- 체력 관리 메서드 ---
    /// <summary>
    /// 플레이어에게 피해를 입힙니다.
    /// </summary>
    /// <param name="damageAmount">받을 피해량.</param>
    public void TakeDamage(float damageAmount)
    {
        animationHandler.Damage(); // 애니메이션 핸들러를 통해 피해 애니메이션 실행
        float finalDamage = Mathf.Max(0, damageAmount - player.Defense); // 방어력 적용
        player.Health -= finalDamage;
        Debug.Log($"피해를 받았습니다: {finalDamage}. 남은 체력: {player.Health}");

        if (player.Health <= 0)
        {
            Die(); // 플레이어 사망 처리
        }
    }

    /// <summary>
    /// 플레이어의 체력을 회복시킵니다.
    /// </summary>
    /// <param name="healAmount">회복할 체력 양.</param>
    public void Heal(float healAmount)
    {
        player.Health = Mathf.Min(player.MaxHealth, player.Health + healAmount);
        Debug.Log($"체력 회복: {healAmount}. 현재 체력: {player.Health}");
    }

    /// <summary>
    /// 플레이어 사망 처리 로직 (필요시 구현).
    /// </summary>
    private void Die()
    {
        animationHandler.Death(); // 애니메이션 핸들러를 통해 사망 애니메이션 실행
        Debug.Log("플레이어가 사망했습니다!");
        // 게임 오버 처리, UI 표시 등
        // Time.timeScale = 0f; // 게임 일시 정지 (예시)
    }
}
