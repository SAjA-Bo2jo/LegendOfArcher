using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Abillity : MonoBehaviour
{
    protected Player player;                                    // �÷��̾� ����
    protected AnimationHandler animationHandler;                // �ִϸ��̼� �ڵ鷯 ���� (���� �� Ȱ�� ����)
    protected GameObject target;                                  // ���� ���
    protected float lastAttackTime = 0f;                          // ������ ���� �ð� �����
}
