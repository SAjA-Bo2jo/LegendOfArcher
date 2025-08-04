using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// 플레이어 캐릭터를 제어하는 컨트롤러 클래스입니다.
// 이동, 회전, 타겟 탐색, 능력 사용 등을 처리합니다.
public class PlayerController : BaseController
{
    private new Camera camera;   // 현재 메인 카메라를 참조하기 위한 변수

    private GameObject target;   // 타겟이 되는 적 오브젝트

    [SerializeField] public Abillity AbillityPrefab;   // 능력 프리팹을 에디터에서 지정 가능하게 설정

    protected Abillity abillity;   // 플레이어가 소유한 능력 컴포넌트

    protected Player player;   // 플레이어 클래스 참조 (체력, 속도 등 데이터 보관)

    // 플레이어가 마지막으로 바라본 수평 방향을 저장하는 변수입니다.
    // BaseController의 LookDirection과 연동되어 사용되므로, 필드명을 BaseController와 구분되게 수정했습니다.
    private Vector2 _lastHorizontalLookDirection = Vector2.right;

    // --- 새로 추가되거나 수정된 부분 ---

    // 플레이어가 현재 입력에 의해 움직이는 중인지 확인하는 프로퍼티
    public bool IsMoving { get; private set; }

    // --- 기존 코드 ---

    protected override void Awake()   // 컴포넌트 초기화. Rigidbody, Player, AnimationHandler, Abillity 등을 가져옵니다.
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        animationHandler = GetComponent<AnimationHandler>();
        abillity = GetComponentInChildren<Abillity>(); // PlayerController가 Abillity를 직접 제어합니다.
                                                       // BaseController의 weaponHandler와는 별개입니다.
    }

    // 게임 시작 시 호출. 카메라 설정 포함.
    protected override void Start()
    {
        base.Start();
        camera = Camera.main;

        // 플레이어의 초기 LookDirection 설정 (BaseController의 LookDirection 프로퍼티 사용)
        LookDirection = _lastHorizontalLookDirection;
    }

    protected override void Update()   // 매 프레임마다 호출. 입력 처리 및 회전 로직 수행.
    {
        HandleInput();           // 방향키 입력 및 타겟 설정
        Rotate(LookDirection);   // 캐릭터 회전 (BaseController의 LookDirection 프로퍼티 사용)
    }

    protected override void FixedUpdate()   // 물리 연산 주기마다 호출. 이동 및 넉백 적용 처리.
    {
        MoveToward(MoveDirection); // 실제 이동 처리

        if (KnockbackTime > 0.0f)
        {
            KnockbackTime -= Time.fixedDeltaTime;
        }
    }

    // 캐릭터를 특정 방향으로 이동시키는 메서드.
    // 넉백 상태라면 이동 속도 감소 및 넉백 벡터 추가.
    protected override void MoveToward(Vector2 direction)
    {
        // BaseController의 moveSpeed 대신 Player의 Speed를 사용합니다.
        direction *= player.Speed;   // 기본 속도 반영

        if (KnockbackTime > 0.0f)
        {
            direction *= 0.2f;       // 넉백 시 이동 감속
            direction += Knockback;    // 넉백 방향 적용
        }

        _rigidbody.velocity = direction;   // 최종 이동 속도 적용
        animationHandler.Move(direction);  // 애니메이션 처리
    }

    // 입력값을 받아서 이동 방향과 시선 방향을 설정합니다.
    protected override void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 currentInputDirection = new Vector2(horizontal, vertical);
        MoveDirection = currentInputDirection.normalized; // BaseController의 MoveDirection 업데이트

        // 플레이어가 이동 입력을 받았는지 확인하여 IsMoving 프로퍼티 업데이트
        IsMoving = currentInputDirection.magnitude > 0.01f;

        target = FindTarget(); // 플레이어 주변의 적을 찾습니다.

        // 1. 타겟이 있을 경우: 타겟 방향을 바라봅니다.
        if (target != null)
        {
            // BaseController의 LookDirection 프로퍼티를 업데이트합니다.
            LookDirection = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;

            // 타겟을 바라볼 때도 마지막 수평 방향을 갱신하여, 타겟이 사라지면 이 방향을 바라보게 합니다.
            if (Mathf.Abs(LookDirection.x) > 0.01f)
            {
                _lastHorizontalLookDirection = new Vector2(LookDirection.x, 0).normalized;
            }
        }
        // 2. 타겟은 없고, 플레이어가 이동 중일 경우:
        else if (IsMoving) // 이제 IsMoving 프로퍼티를 사용
        {
            // 수평 입력이 있을 때만 LookDirection을 이동 방향으로 설정하고 _lastHorizontalLookDirection을 갱신합니다.
            if (Mathf.Abs(horizontal) > 0.01f)
            {
                LookDirection = currentInputDirection.normalized;
                _lastHorizontalLookDirection = new Vector2(horizontal, 0).normalized;
            }
            // 수평 입력 없이 수직으로만 이동 중일 때:
            else
            {
                // 마지막으로 바라본 수평 방향을 유지합니다.
                LookDirection = _lastHorizontalLookDirection;
            }
        }
        // 3. 타겟도 없고 이동 중도 아닐 경우 (가만히 있을 때):
        else
        {
            // 마지막으로 이동했던 수평 방향을 바라봅니다.
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
}