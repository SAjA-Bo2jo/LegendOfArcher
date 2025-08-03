using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStats
{
    [Header("ü��")]
    public float maxHealth = 5f;                                        // maxHealth : �� ��ü �ִ� ü��
    public float healthPoint = 5f;                                      // healthPoint : ü��

    [Header("����")]
    public float attackDamage = 5f;                                     // attackDamage : ���� ���ݷ�
    public float contactDamage = 1f;                                    // contactDamage : ���� ���ݷ�
    public float attackCooldown = 3f;                                   // attackCooldown : ���� ��Ÿ��

    [Header("�̵�")]
    public float moveSpeed = 2f;                                        // moveSpeed : �̵� �ӵ�
    public float detectRange = 15f;                                     // detectRange : Ÿ�� ���� �Ÿ�

    [Header("���� ��Ÿ�")]
    public float attackRange = 6f;                                      // attackRange : ���� ��Ÿ�
    public float optimalDistanceRatio = 0.75f;                          // optimalDistanceRatio : �����Ÿ� ���� (���� ��Ÿ��� ���� ����)
    public float distanceTolerance = 0.5f;                              // distanceTolerance : ���� ��� ����
    public bool canRetreat = true;                                      // canRetreat : ���� ���� ����

    public float OptimalDistance => attackRange * optimalDistanceRatio; // OptimalDistance : ���� �Ÿ�

    public void StatInitialize()
    {
        healthPoint = maxHealth;
    }

    public bool TakeDamage(float damage)                                // ������ ��� + �� ��ü ��� ���� �Ǵ�
    {
        healthPoint -= damage;
        return healthPoint <= 0;
    }
}
