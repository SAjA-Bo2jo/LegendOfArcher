using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GateType { Entry, Exit };

public class GateController : MonoBehaviour
{

    [SerializeField] private GateType gateType;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D gateCollider;

    public Action OnPlayerEnterExitGate;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        gateCollider = GetComponent<Collider2D>();

        // �ⱸ �浹 ��Ȱ��ȭ
        gateCollider.enabled = false;
    }

    public void OpenExitGate()
    {
        if (gateType == GateType.Exit)
        {
            animator.SetTrigger("OpenGate");
            gateCollider.enabled = true;
        }
        //Debug.Log("���� ���������� �̵��մϴ�.");
    }

    public void SetGateType(GateType type)
    {
        gateType = type;

        if (animator == null)
            animator = GetComponent<Animator>();

        if (gateType == GateType.Entry)
        {
            animator.SetTrigger("CloseGate");
        }
        else
        {
            animator.Play("IdleClose");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gateCollider.enabled) return;
        
        // �÷��̾� �� ������Ʈ �浹 ����
        if (!collision.CompareTag("Player")) return;
        
        Debug.Log("�÷��̾ �ⱸ�� ������");

        // StageManager ���� ���� �������� �̵� �޼��� �̸� Ȯ�� �� ����
        // OnPlayerEnterExitGate?.Invoke();
        StageManager.Instance.ToNextStage();
    }
}