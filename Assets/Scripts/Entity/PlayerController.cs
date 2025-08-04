using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// 플레이어 캐릭터를 제어하는 컨트롤러 클래스입니다.
// 이동, 회전, 타겟 탐색, 능력 사용 등을 처리합니다.
public class PlayerController : BaseController
{
    // === 직렬화된 필드 (인스펙터에서 설정) ===
    // 플레이어의 애니메이션 핸들러 컴포넌트 참조.
    // 이 필드는 PlayerController와 다른 GameObject에 있는 AnimationHandler를
    // Unity 에디터에서 직접 드래그 앤 드롭으로 연결하기 위해 사용됩니다.
    [SerializeField] private AnimationHandler playerAnimationHandler;

    // === 내부 상태 변수 ===
    private new Camera camera; // 현재 메인 카메라를 참조하기 위한 변수

    // Player.cs에서 타겟을 탐색하므로, 여기서는 타겟을 보관만 합니다.
    private GameObject target;

    protected Player player; // 플레이어 클래스 참조 (체력, 속도 등 데이터 보관)

    // 플레이어가 마지막으로 바라본 수평 방향을 저장하는 변수입니다.
    private Vector2 _lastHorizontalLookDirection = Vector2.right;

    // 플레이어가 현재 입력에 의해 움직이는 중인지 확인하는 프로퍼티
    public bool IsMoving { get; private set; }

    // === Unity 생명주기 메서드 ===

    // 오브젝트가 처음 활성화될 때 한 번 호출됩니다.
    // 주로 컴포넌트 참조를 가져오는 데 사용됩니다.
    protected override void Awake() // 컴포넌트 초기화
    {
        // BaseController의 Awake 호출 (BaseController에 _rigidbody 초기화가 있다면 중복 피하기)
        // 현재 코드에서는 _rigidbody = GetComponent<Rigidbody2D>(); 로 직접 초기화하고 있으므로 base.Awake()가 먼저 호출되어도 무방
        // 만약 BaseController가 _rigidbody를 초기화한다면, 이 줄은 base.Awake(); 아래로 옮길 수 있습니다.
        _rigidbody = GetComponent<Rigidbody2D>(); // 이 PlayerController 스크립트가 붙어있는 GameObject에서 Rigidbody2D를 찾습니다.

        // Player 컴포넌트도 같은 GameObject에 있을 것으로 예상합니다.
        player = GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("PlayerController: Player 컴포넌트를 찾을 수 없습니다! PlayerController와 같은 GameObject에 Player 스크립트가 있는지 확인하세요.");
        }

        // --- 핵심 수정 사항: AnimationHandler는 더 이상 여기서 GetComponent<T>()로 찾지 않습니다. ---
        // animationHandler = GetComponent<AnimationHandler>(); // 이 줄을 삭제했습니다.
        // 이제 playerAnimationHandler 필드를 통해 인스펙터에서 연결할 것입니다.
    }

    // 첫 번째 프레임 업데이트 직전에 호출됩니다.
    // Awake 이후에 초기화되어야 하는 로직을 처리합니다.
    protected override void Start() // 게임 시작 시 호출. 카메라 설정 포함.
    {
        base.Start(); // BaseController의 Start 호출
        camera = Camera.main;

        // 플레이어가 처음 시작할 때 바라보는 방향을 설정합니다.
        LookDirection = _lastHorizontalLookDirection;

        // --- 핵심 추가 사항: playerAnimationHandler가 인스펙터에서 제대로 연결되었는지 확인 ---
        if (playerAnimationHandler == null)
        {
            Debug.LogError("PlayerController: Start() 시점에 playerAnimationHandler가 NULL입니다. Unity 인스펙터에서 플레이어 메인 스프라이트의 AnimationHandler를 이 필드에 드래그 앤 드롭하여 연결해야 합니다.");
        }
        else
        {
            Debug.Log("PlayerController: Start() 시점에 playerAnimationHandler가 성공적으로 연결되었습니다: " + playerAnimationHandler.gameObject.name);
        }
    }

    // 매 프레임마다 호출됩니다. 입력 처리 및 회전 로직을 수행합니다.
    protected override void Update() // 매 프레임마다 호출. 입력 처리 및 회전 로직 수행.
    {
        HandleInput(); // 방향키 입력 및 타겟 설정
        Rotate(LookDirection); // 캐릭터 회전 (BaseController의 메서드)
    }

    // 물리 연산 주기마다 호출됩니다. 이동 및 넉백 적용을 처리합니다.
    protected override void FixedUpdate() // 물리 연산 주기마다 호출. 이동 및 넉백 적용 처리.
    {
        MoveToward(MoveDirection); // 실제 이동 처리 (이 메서드 내에서 애니메이션 핸들러 호출)

        // 넉백 시간 처리 (넉백 중일 때만 이동 속도에 영향을 줍니다)
        if (KnockbackTime > 0.0f)
        {
            KnockbackTime -= Time.fixedDeltaTime;
        }
    }

    // === 이동 로직 메서드 ===

    // 캐릭터를 특정 방향으로 이동시키는 메서드.
    // FixedUpdate에서 현재 MoveDirection을 인자로 받아 호출됩니다.
    protected override void MoveToward(Vector2 direction)
    {
        // 플레이어의 현재 이동 속도를 곱하여 실제 이동 벡터를 계산합니다.
        // player 참조가 null이 아닐 때만 접근하도록 null 체크를 포함하는 것이 안전합니다.
        if (player != null)
        {
            direction *= player.MoveSpeed; // Player의 MoveSpeed를 사용
        }
        else
        {
            Debug.LogWarning("PlayerController: Player 스크립트가 없어 이동 속도를 적용할 수 없습니다.");
            // 기본 이동 속도라도 적용하거나, 완전히 멈출 수 있습니다.
            // direction *= 5.0f; // 예시: 기본 속도 적용
        }

        // 넉백 중일 때 이동을 감속하고 넉백 벡터를 추가합니다.
        if (KnockbackTime > 0.0f)
        {
            direction *= 0.2f; // 넉백 시 이동 감속
            direction += Knockback; // 넉백 방향 적용 (BaseController의 Knockback)
        }

        // Rigidbody2D의 velocity를 직접 설정하여 물리적인 이동을 처리합니다.
        // _rigidbody 참조가 null이 아닐 때만 접근하도록 null 체크를 포함하는 것이 안전합니다.
        if (_rigidbody != null)
        {
            _rigidbody.velocity = direction; // 최종 이동 속도 적용
        }
        else
        {
            Debug.LogError("PlayerController: Rigidbody2D가 할당되지 않아 이동할 수 없습니다!");
        }

        // --- 핵심 수정 사항: playerAnimationHandler를 통해 애니메이션 처리 ---
        // playerAnimationHandler가 null이 아닌지 반드시 확인해야 NullReferenceException을 방지할 수 있습니다.
        if (playerAnimationHandler != null)
        {
            playerAnimationHandler.Move(direction); // 애니메이션 핸들러의 Move 메서드 호출
        }
        else
        {
            // 이 에러 메시지는 Start()에서도 출력되므로, 여기서 매 프레임 출력되는 것은 피할 수 있습니다.
            // Debug.LogError("PlayerController: playerAnimationHandler가 할당되지 않아 애니메이션을 제어할 수 없습니다.");
            // 하지만 첫 프레임 이후에도 계속 null이면 문제가 있으므로 진단을 위해 유지할 수 있습니다.
        }
    }

    // === 입력 처리 메서드 ===

    // 입력값을 받아서 이동 방향과 시선 방향을 설정합니다.
    protected override void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 currentInputDirection = new Vector2(horizontal, vertical);
        MoveDirection = currentInputDirection.normalized; // BaseController의 MoveDirection 프로퍼티

        // 입력의 크기가 0.01f보다 크면 움직이는 것으로 간주합니다.
        IsMoving = currentInputDirection.magnitude > 0.01f;

        target = FindTarget(); // 플레이어 주변의 적을 찾습니다.

        // 1. 타겟이 있을 경우: 타겟 방향을 바라봅니다.
        if (target != null)
        {
            LookDirection = ((Vector2)target.transform.position - (Vector2)transform.position).normalized; // BaseController의 LookDirection 프로퍼티

            // 수평 방향만 따로 저장하여, 수직 이동 시에도 가로 방향을 유지하도록 합니다.
            if (Mathf.Abs(LookDirection.x) > 0.01f)
            {
                _lastHorizontalLookDirection = new Vector2(LookDirection.x, 0).normalized;
            }
        }
        // 2. 타겟은 없고, 플레이어가 이동 중일 경우:
        else if (IsMoving)
        {
            // 수평 입력이 있을 때만 LookDirection을 현재 이동 방향으로 설정하고 _lastHorizontalLookDirection을 업데이트합니다.
            if (Mathf.Abs(horizontal) > 0.01f)
            {
                LookDirection = currentInputDirection.normalized;
                _lastHorizontalLookDirection = new Vector2(horizontal, 0).normalized;
            }
            else
            {
                // 수평 입력이 없으면 마지막으로 바라본 수평 방향을 유지합니다.
                LookDirection = _lastHorizontalLookDirection;
            }
        }
        // 3. 타겟도 없고 이동 중도 아닐 경우 (가만히 있을 때):
        else
        {
            // 가만히 있을 때는 마지막으로 바라본 수평 방향을 유지합니다.
            LookDirection = _lastHorizontalLookDirection;
        }
    }

    // === 타겟 탐색 메서드 ===

    // 현재 활성화된 적 중에서 가장 가까운 타겟을 반환합니다.
    public GameObject FindTarget()
    {
        // "Enemy" 태그를 가진 모든 GameObject를 찾습니다.
        GameObject target = GameObject
            .FindGameObjectsWithTag("Enemy")
            // null이 아니고 활성화된 GameObject만 필터링합니다.
            .Where(enemy => enemy != null && enemy.activeInHierarchy)
            // 플레이어의 AttackRange 내에 있는 적만 필터링합니다.
            // player 참조가 null이 아닐 때만 AttackRange를 사용합니다.
            .Where(enemy => player != null ? Vector3.Distance(enemy.transform.position, transform.position) < player.AttackRange : false)
            // 플레이어로부터의 거리에 따라 오름차순으로 정렬합니다.
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position))
            // 가장 첫 번째(가장 가까운) 적을 반환합니다. 없으면 null을 반환합니다.
            .FirstOrDefault();

        return target;
    }

    // === 유틸리티 메서드 ===

    // 플레이어와 타겟 간의 거리 계산 메서드입니다.
    protected float DistanceToTarget()
    {
        if (target == null) return float.MaxValue; // 타겟이 없으면 무한대 반환
        return Vector2.Distance(transform.position, target.transform.position);
    }
}