using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public enum EnemyBehaviorType
{
    Melee,
    Ranged,
    Boss
}

public class EnemyController : BaseController
{
    [Header("�ɷ�ġ")]
    [SerializeField] private EnemyStats stats;
    public EnemyStats Stats { get { return stats; } }

    [Header("���� Ÿ��")]
    [SerializeField] private EnemyBehaviorType behaviorType;    // �ش� ���� Ÿ�� �ν����� �Է�

    [Header("���Ÿ� ����")]
    [SerializeField] private GameObject arrowPrefab;

    private Transform target;
    private EnemyAnimationHandler _animation;
    private IEnemyAttack EnemyAttack;

    bool isDead = false;
    public bool IsDead => isDead;
    public GameObject ArrowPrefab => arrowPrefab;



    // �����ֱ� �Լ�
    protected override void Awake()
    {
        base.Awake();

        moveSpeed = stats.moveSpeed;

        switch (behaviorType)                             // -> �ν����� �� �������� ���� Ÿ�� �Ǵ�
        {
            case EnemyBehaviorType.Melee:
                EnemyAttack = new MeleeEnemyAttack();
                break;

            case EnemyBehaviorType.Ranged:
                EnemyAttack = new RangedEnemyAttack();
                break;

            case EnemyBehaviorType.Boss:
                EnemyAttack = new BossEnemyAttack();
                break;
        }
    }

    protected override void Start()
    {
        base.Start();

        // stats�� null�� �ƴ� ���� �ʱ�ȭ�ϵ��� null üũ �߰�
        if (stats != null)
        {
            stats.StatInitialize();
        }

        // StageManager.Instance._Player�� null�� �� �����Ƿ� null üũ �߰�
        if (StageManager.Instance != null && StageManager.Instance._Player != null)
        {
            Init(StageManager.Instance._Player.transform);
        }
        _animation = GetComponent<EnemyAnimationHandler>();
    }

    protected override void Update()
    {
        base.Update();

        if (behaviorType == EnemyBehaviorType.Boss && EnemyAttack is BossEnemyAttack boss)
        {
            boss.Update(this);
        }
    }

    public void Init(Transform target)                       // �ʱ�ȭ �� ���� ��� ���ϴ� �޼���
    {
        this.target = target;
    }



    // ��ƿ��Ƽ �޼���
    protected float DistanceToTarget()                       // DistanceToTarget �޼��� : �÷��̾� ~ �� �Ÿ� ���ϴ� �޼���
    {
        if (target == null) return float.MaxValue;
        return Vector2.Distance(transform.position, target.position);
    }

    public Vector2 DirectionToTarget()                                   // DirectionToTarget �޼��� : �� -> �÷��̾� �������� ����
    {
        if (target == null) return Vector2.zero;
        return (target.position - transform.position).normalized;
    }

    public bool IsInAttackRange()                            // IsInAttackRange �޼��� : ���� ��Ÿ� ���� �÷��̾� �ִ� �� Ȯ��
    {
        if (target == null) return false;
        return DistanceToTarget() <= stats.attackRange;
    }



    // HandleInput �޼��� : �� ��ü�� Ÿ��(�÷��̾�) ���� �ý���
    protected override void HandleInput()
    {
        if (target == null || stats.healthPoint <= 0)         // ���� ���(�÷��̾�) ���ų� �� ��ü�� �׾��� ���
        {
            moveDirection = Vector2.zero;                     // -> �̵� ����
            lookDirection = Vector2.zero;                     // -> ȸ�� ����
            return;
        }

        float distance = DistanceToTarget();
        Vector2 direction = DirectionToTarget();

        lookDirection = direction;                             // �� ��ü�� Ÿ���� ��� �ٶ�

        if (distance <= stats.detectRange)                     // ���� ���� ���� ���
        {
            float optimalDist = stats.OptimalDistance;

            if (distance > optimalDist + stats.distanceTolerance) // ��� 1. �÷��̾ �ʹ� �ָ� ���� ���
            {
                moveDirection = direction;                     // �÷��̾� �������� �̵�
            }
            else
            {
                moveDirection = Vector2.zero ;                              // ��� 2. �����Ÿ� ���� ���� ���
            }
        }
        else
        {
            moveDirection = Vector2.zero;                      // ���� ���� ���� ��� : ����
        }

        if (IsInAttackRange() && EnemyAttack.CanAttack(this))   // �̵� �� -> ��Ÿ� ���� Ÿ�� ������ ����
        {
            EnemyAttack.Attack(this);
        }

        bool inRange = IsInAttackRange();
        bool canAttack = EnemyAttack.CanAttack(this);

        Debug.Log($"거리: {distance:F2}, 공격범위: {stats.attackRange}, 범위안: {inRange}, 공격가능: {canAttack}");

        if (inRange && canAttack)
        {
            Debug.Log("🎯 공격 조건 만족! Attack 호출!");
            EnemyAttack.Attack(this);
        }
    }



    // �� ��ü ��� ó�� ���� �޼���
    public void GetDamage(float dmg)                           // GetDamage �޼��� : �� ��ü �ǰ� + ��� ���� ó��
    {

        isDead = stats.TakeDamage(dmg);

        if (isDead)
        {
            // ������� ���, ��� ó�� �ڷ�ƾ�� ȣ��
            HandleDeath();
        }
        else
        {
            _animation.Damage();
        }
    }

    private void HandleDeath()                                 // HandleDeath �޼��� : �� ��ü ��� ó��
    {
        _animation.Death();                                                // �ִϸ��̼� ����

        // Death �ִϸ��̼� ��� �Ϸ� �� ������Ʈ�� Ǯ�� ��ȯ
        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()                       // Death �ִϸ��̼� ��� �Ϸ� �� ������Ʈ ����
    {
        // Death �ִϸ��̼��� ����� �ð���ŭ ���
        yield return new WaitForSeconds(0.4f);

        // StageManager�� ���� ���� ��������� �˸�
        if (StageManager.Instance != null)
        {
            StageManager.Instance.RemoveMonsterFromList(gameObject);
        }

        EnemyPoolObject poolObject = GetComponent<EnemyPoolObject>();
        if (poolObject != null)
        {
            poolObject.ReturnToPool();                         // Ǯ�� ���� ��ü ��������
        }
        else
        {
            Debug.Log($"{gameObject.name}: EnemyPoolObject�� ���� Destroy�� ��ü�մϴ�.");
            Destroy(gameObject);
        }
    }



    // ���� ��뿡�� ������ �ִ� �޼���
    public void ApplyContactDamage(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            float damage = stats.contactDamage;

            // 보스 돌진 중이면 3배 피해
            if (behaviorType == EnemyBehaviorType.Boss && EnemyAttack is BossEnemyAttack boss && boss.IsCharging())
            {
                damage = stats.contactDamage * 3f;
                Debug.Log("돌진 공격! 피해: " + damage);
            }
            else
            {
                Debug.Log("몸통 박치기! 피해: " + damage);
            }

            ResourceController rc = collider.GetComponent<ResourceController>();
            if (rc != null)
            {
                rc.ChangeHealth(-damage);
            }
        }
    }
}
