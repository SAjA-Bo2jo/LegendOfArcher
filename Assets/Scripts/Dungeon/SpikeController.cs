using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D spikeCollider;
    [SerializeField] private float delay = 1.5f;

    private void Start()
    {
        StartCoroutine(SpikeRoutione());
    }

    private IEnumerator SpikeRoutione()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);

            // start animation
            animator.SetTrigger("IsActive");
        }
    }

}
