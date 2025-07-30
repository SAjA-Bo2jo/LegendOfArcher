using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public enum GateType { Entry, Exit  };

    [SerializeField] private GateType gateType;
    [SerializeField] Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (gateType == GateType.Entry)
        {
            animator.SetTrigger("CloseGate");
        }
        else
        {
            animator.Play("IdleClose");
        }
    }

    public void OpenExitGate()
    {
        if (gateType == GateType.Exit)
        {
            animator.SetTrigger("OpenDoor");
        }
    }

}
