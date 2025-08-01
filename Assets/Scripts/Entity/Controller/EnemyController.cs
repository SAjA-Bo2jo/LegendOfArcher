using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : BaseController
{
    private Transform target;
    private EnemyAnimationHandler _animation;

    [SerializeField] private float detectRange = 15f;                       // detectRange : ���� �÷��̾� ���� �Ÿ�

    [Header("Temporary setting")]                                           // �ӽ� ���ð�. �����Ÿ�, ���� �Ÿ� ������ ���߿� stat Ŭ������ ���� DB�� ��ü�� ������
    [SerializeField] private float attackRange = 6f;                        // attackRange : ���� �����Ÿ�
    [SerializeField] private float optimalDistanceRatio = 0.75f;            // optimalDistanceRatio : ���� �����ϴ� �Ÿ��� ����
    [SerializeField] private float distanceTolerance = 0.5f;                // distanceTolerance : �Ÿ� ��� ����
    [SerializeField] private bool canRetreat = true;                        // canRetreat : ���� ������ �� �ִ��� ����
    [SerializeField] private float healthPoint = 5f;

    private float OptimalDistance => attackRange * optimalDistanceRatio;    // OptimalDistance : ���� �����Ÿ�

    protected override void Start()
    {
        base.Start();
        
        Init(StageManager.Instance._Player.transform);
        _animation = GetComponent<EnemyAnimationHandler>();
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

    protected override void HandleInput()                                   // �� ��ü ���� AI
    {
        if (target == null)                                         // ���� ���(�÷��̾�) ���� ���
        {
            moveDirection = Vector2.zero;                           // -> �̵� ����
            lookDirection = Vector2.zero;                           // -> ȸ�� ����
            return;
        }

        float distance = DistanceToTarget();       
        Vector2 direction = DirectionToTarget();

        lookDirection = direction;

        if (distance <= detectRange)                                // ���� ���� ���� ���
        {
            float optimalDist = OptimalDistance;

            if (distance >  optimalDist + distanceTolerance)        // ��� 1. �÷��̾ �ʹ� �ָ� ���� ���
            {
                moveDirection = direction;                          // �÷��̾� �������� �̵�
            }
            else if (distance < optimalDist - distanceTolerance)    // ��� 2. �÷��̾ �ʹ� ������ ���� ���
            {
                if (canRetreat)                                     
                {
                    moveDirection -= direction;                     // ���� ���� ��ü�� ����
                }
                else
                {
                    moveDirection = Vector2.zero;                   // ���� �Ұ��� �� ����
                }
            }
            else
            {
                moveDirection = Vector2.zero ;                      // ���3. �����Ÿ� ���� ���� ���
            }
        }
        else { moveDirection = Vector2.zero ; }                     // ���� ���� ���� ��� : ����
    }

    public void GetDamage(float dmg)
    {
        Debug.Log("�������� �����ϴ�" + dmg);
        healthPoint -= dmg;
        if (healthPoint > 0)
        {
            _animation.Damage();
        }
        else
        {
            _animation.Death();
        }
    }
}
