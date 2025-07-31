using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GateType { Entry, Exit };

public class GateController : MonoBehaviour
{

    [SerializeField] private GateType gateType;
    [SerializeField] Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenExitGate()
    {
        if (gateType == GateType.Exit)
        {
            animator.SetTrigger("OpenDoor");
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

}
