using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// 플레이어 캐릭터를 제어하는 컨트롤러 클래스입니다.
// 이동, 회전, 타겟 탐색, 능력 사용 등을 처리합니다.
public class PlayerController : BaseController
{
    private new Camera camera;    // 현재 메인 카메라를 참조하기 위한 변수


    private GameObject target;    // 타겟이 되는 적 오브젝트


    [SerializeField] public Abillity AbillityPrefab;    // 능력 프리팹을 에디터에서 지정 가능하게 설정


    protected Abillity abillity;    // 플레이어가 소유한 능력 컴포넌트


    protected AnimationHandler animationHandler;    // 애니메이션을 제어하는 핸들러


    protected Player player;    // 플레이어 클래스 참조 (체력, 속도 등 데이터 보관)


    protected override void Awake()    // 컴포넌트 초기화. Rigidbody, Player, AnimationHandler, Abillity 등을 가져옵니다.
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        animationHandler = GetComponent<AnimationHandler>();
        abillity = GetComponentInChildren<Abillity>();
    }

    // 게임 시작 시 호출. 카메라 설정 포함.
    protected override void Start()
    {
        base.Start();
        camera = Camera.main;
    }

    protected override void Update()    // 매 프레임마다 호출. 입력 처리 및 회전 로직 수행.
    {
        HandleInput();              // 방향키 입력 및 타겟 설정
        Rotate(lookDirection);      // 캐릭터 회전
    }

    protected override void FixedUpdate()    // 물리 연산 주기마다 호출. 이동 및 넉백 적용 처리.
    {
        MoveToward(MoveDirection);  // 실제 이동 처리

        if (knockbackTime > 0.0f)
        {
            knockbackTime -= Time.fixedDeltaTime;
        }
    }

    // 캐릭터를 특정 방향으로 이동시키는 메서드.
    // 넉백 상태라면 이동 속도 감소 및 넉백 벡터 추가.
    protected override void MoveToward(Vector2 direction)
    {
        direction *= player.Speed;      // 기본 속도 반영

        if (KnockbackTime > 0.0f)
        {
            direction *= 0.2f;          // 넉백 시 이동 감속
            direction += Knockback;     // 넉백 방향 적용
        }

        _rigidbody.velocity = direction;    // 최종 이동 속도 적용
        animationHandler.Move(direction);   // 애니메이션 처리
    }

    protected override void HandleInput()    // 입력값을 받아서 이동 방향과 시선 방향을 설정합니다.
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        MoveDirection = new Vector2(horizontal, vertical).normalized;        // 이동 방향 설정 (정규화된 벡터)

        target = FindTarget();        // 가장 가까운 타겟 탐색

        if (target == null)        //  타겟이 없으면 회전하지 않도록 lookDirection을 0으로 설정
        {
            lookDirection = Vector2.zero;
            return;
        }

        lookDirection = ((Vector2)target.transform.position - (Vector2)player.transform.position);        // 타겟 방향 계산

        if (lookDirection.sqrMagnitude < 0.81f)        // 너무 가까우면 회전하지 않음
            lookDirection = Vector2.zero;
        else
            lookDirection = lookDirection.normalized;
    }

    public GameObject FindTarget()    // 현재 활성화된 적 중에서 가장 가까운 타겟을 반환합니다.
    {
        GameObject target = GameObject
            .FindGameObjectsWithTag("Enemy")                                        // 모든 적 가져오기
            .Where(enemy => enemy.activeInHierarchy)                               // 활성화된 것만
            .Where(enemy => Vector3.Distance(enemy.transform.position, transform.position) < player.AttackRange) // 사정거리 안
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position)) // 거리 순 정렬
            .FirstOrDefault();                                                     // 가장 가까운 적 선택

        return target;
    }

    protected float DistanceToTarget()    // 플레이어와 타겟 간의 거리 계산 메서드입니다.
    {
        // target이 null일 가능성은 HandleInput에서 처리했지만, 안전하게 null 체크해도 좋습니다.
        if (target == null) return float.MaxValue;

        return Vector2.Distance(transform.position, target.transform.position);
    }
}
