using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GateType { Entry, Exit };

public class GateController : MonoBehaviour
{

    [SerializeField] private GateType gateType;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D collider;

    public Action OnPlayerEnterExitGate;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();

        // �ⱸ �浹 ��Ȱ��ȭ
        collider.enabled = false;
    }

    public void OpenExitGate()
    {
        if (gateType == GateType.Exit)
        {
            animator.SetTrigger("OpenDoor");
            collider.enabled = true;
        }
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
        if (!collider.enabled) return;

        // �÷��̾� �� ������Ʈ �浹 ����
        if (!collision.CompareTag("Player")) return;

        Debug.Log("�÷��̾ �ⱸ�� ������");

        // StageManager ���� ���� �������� �̵� �޼��� �̸� Ȯ�� �� ����
        OnPlayerEnterExitGate?.Invoke();
    }
}