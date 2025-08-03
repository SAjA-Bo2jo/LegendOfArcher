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

    private Transform target;
    private EnemyAnimationHandler _animation;
    private IEnemyAttack EnemyAttack;

    bool isDead = false;
    public bool IsDead => isDead;



    // �����ֱ� �Լ�
    protected override void Awake()
    {
        base.Awake();

        switch (behaviorType)                                   // -> �ν����� �� �������� ���� Ÿ�� �Ǵ�
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

        stats.StatInitialize();

        Init(StageManager.Instance._Player.transform);
        _animation = GetComponent<EnemyAnimationHandler>();
    }

    public void Init(Transform target)                                      // �ʱ�ȭ �� ���� ��� ���ϴ� �޼���
    {
        this.target = target;
    }



    // ��ƿ��Ƽ �޼���
    protected float DistanceToTarget()                                      // DistanceToTarget �޼��� : �÷��̾� ~ �� �Ÿ� ���ϴ� �޼���
    {
        return Vector2.Distance(transform.position, target.position);
    }

    protected Vector2 DirectionToTarget()                                   // DirectionToTarget �޼��� : �� -> �÷��̾� �������� ����
    {
        return (target.position - transform.position).normalized;
    }

    public bool IsInAttackRange()                                           // IsInAttackRange �޼��� : ���� ��Ÿ� ���� �÷��̾� �ִ� �� Ȯ��
    {
        if (target == null) return false;
        return DistanceToTarget() <= stats.attackRange;
    }



    // HandleInput �޼��� : �� ��ü�� Ÿ��(�÷��̾�) ���� �ý���
    protected override void HandleInput()
    {
        if (target == null || stats.healthPoint <= 0)                       // ���� ���(�÷��̾�) ���ų� �� ��ü�� �׾��� ���
        {
            moveDirection = Vector2.zero;                                   // -> �̵� ����
            lookDirection = Vector2.zero;                                   // -> ȸ�� ����
            return;
        }

        float distance = DistanceToTarget();       
        Vector2 direction = DirectionToTarget();

        lookDirection = direction;                                          // �� ��ü�� Ÿ���� ��� �ٶ�

        if (distance <= stats.detectRange)                                  // ���� ���� ���� ���
        {
            float optimalDist = stats.OptimalDistance;

            if (distance >  optimalDist + stats.distanceTolerance)          // ��� 1. �÷��̾ �ʹ� �ָ� ���� ���
            {
                moveDirection = direction;                                  // �÷��̾� �������� �̵�
            }
            else if (distance < optimalDist - stats.distanceTolerance)      // ��� 2. �÷��̾ �ʹ� ������ ���� ���
            {
                if (stats.canRetreat)                                     
                {
                    moveDirection -= direction;                             // ���� ���� ��ü�� ����
                }
                else
                {
                    moveDirection = Vector2.zero;                           // ���� �Ұ��� �� ����
                }
            }
            else
            {
                moveDirection = Vector2.zero ;                              // ���3. �����Ÿ� ���� ���� ���
            }
        }
        else 
        { 
            moveDirection = Vector2.zero;                                   // ���� ���� ���� ��� : ����
        }

        if (IsInAttackRange() && EnemyAttack.CanAttack(this))               // �̵� �� -> ��Ÿ� ���� Ÿ�� ������ ����
        {
            EnemyAttack.Attack(this);
        }
    }



    // �� ��ü ��� ó�� ���� �޼���
    public void GetDamage(float dmg)                                        // GetDamage �޼��� : �� ��ü �ǰ� + ��� ���� ó��
    {
        Debug.Log("�������� �����ϴ�" + dmg);
        
        isDead = stats.TakeDamage(dmg);

        if (isDead)
        {
            HandleDeath();
        }
        else
        {
            _animation.Death();
        }
    }

    private void HandleDeath()                                              // HandleDeath �޼��� : �� ��ü ��� ó��
    {
        _animation.Damage();                                                // �ִϸ��̼� ����

        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()                                    // Death �ִϸ��̼� ��� �Ϸ� �� ������Ʈ ����
    {
        yield return new WaitForSeconds(0.4f);

        StageManager.Instance.RemoveMonsterFromList(gameObject);

        EnemyPoolObject poolObject = GetComponent<EnemyPoolObject>();
        if (poolObject != null)
        {
            poolObject.ReturnToPool();                                      // Pool�� ���� ��ü ��������
        }
        else
        {
            Debug.Log($"{gameObject.name}: EnemyPoolObject ���� Destroy�� ��ü");
            Destroy(gameObject);
        }
    }



    // ���� ��뿡�� ������ �ִ� �޼���
    public void ApplyContactDamage(Collider2D collider)
    {
        float damage = stats.contactDamage;

        ResourceController resourceController = collider.GetComponent<ResourceController>();
        if (resourceController != null)
        {
            resourceController.ChangeHealth(-damage);
        }
    }
}
