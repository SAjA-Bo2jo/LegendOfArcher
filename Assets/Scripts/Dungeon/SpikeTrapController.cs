using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D spikeCollider;
    [SerializeField] private float delay = 1.5f;
    [SerializeField] private float damage = 2.0f;
    [SerializeField] private float damageCooldowns = 1.0f;

    [SerializeField] PlayerController playerCrtl;
    private Dictionary<PlayerController, float> lastHitTime = new();


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

    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerCtrl = collision.GetComponent<PlayerController>();
            if (playerCtrl != null)
            {
                playerCtrl.TakeDamage(damage);
                Debug.Log($"가시 함정이 플레이어에게 {damage} 데미지!");
            }
        }
    }
    */

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerCtrl = collision.GetComponent<PlayerController>();
            if (playerCtrl != null && spikeCollider.enabled)
            {
                lastHitTime.TryGetValue(playerCtrl, out float lastHit);

                if (Time.time - lastHit >= damageCooldowns)
                {
                    playerCtrl.TakeDamage(damage);
                    // Debug.Log($"가시 함정이 플레이어에게 {damage} 데미지!");
                    lastHitTime[playerCtrl] = Time.time;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerCtrl = collision.GetComponent<PlayerController>();
            if (playerCrtl != null && lastHitTime.ContainsKey(playerCtrl))
            {
                lastHitTime.Remove(playerCtrl); // remove record when out of range
            }
        }
    }
}
