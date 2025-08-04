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

    private Dictionary<Player, float> lastHitTime = new();


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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.Health -= damage;
                Debug.Log($"플레이어에게 데미지 {damage}");
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null && spikeCollider.enabled)
            {
                lastHitTime.TryGetValue(player, out float lastHit);

                if (Time.time - lastHit >= damageCooldowns)
                {
                    player.Health -= damage;
                    Debug.Log($"플레이어에게 데미지 {damage}");
                    lastHitTime[player] = Time.time;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null && lastHitTime.ContainsKey(player))
            {
                lastHitTime.Remove(player); // remove record when out of range
            }
        }
    }
}
