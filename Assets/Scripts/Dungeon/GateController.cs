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

        // 출구 충돌 비활성화
        gateCollider.enabled = false;
    }

    public void OpenExitGate()
    {
        if (gateType == GateType.Exit)
        {
            animator.SetTrigger("OpenGate");
            gateCollider.enabled = true;
        }
        //Debug.Log("다음 스테이지로 이동합니다.");
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
        
        // 플레이어 외 오브젝트 충돌 무시
        if (!collision.CompareTag("Player")) return;
        
        Debug.Log("플레이어가 출구에 도달함");

        // StageManager 에서 다음 스테이지 이동 메서드 이름 확인 후 수정
        // OnPlayerEnterExitGate?.Invoke();
        StageManager.Instance.ToNextStage();
    }
}