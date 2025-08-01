using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseController
{
    private Transform target;

    [SerializeField] private float detectRange = 15f;                       // detectRange : ���� �÷��̾� ���� �Ÿ�

    [Header("Temporary setting")]                                           // �ӽ� ���ð�. �����Ÿ�, ���� �Ÿ� ������ ���߿� stat Ŭ������ ���� DB�� ��ü�� ������
    [SerializeField] private float attackRange = 6f;                        // attackRange : ���� �����Ÿ�
    [SerializeField] private float optimalDistanceRatio = 0.75f;            // optimalDistanceRatio : ���� �����ϴ� �Ÿ��� ����
    [SerializeField] private float distanceTolerance = 0.5f;                // distanceTolerance : �Ÿ� ��� ����
    [SerializeField] private bool canRetreat = true;                        // canRetreat : ���� ������ �� �ִ��� ����

    private float OptimalDistance => attackRange * optimalDistanceRatio;    // OptimalDistance : ���� �����Ÿ�

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
}
