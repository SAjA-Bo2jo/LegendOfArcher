using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
    private float damage = 0f;
    private float speed = 5f;

    void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ResourceController resourceController = collision.GetComponent<ResourceController>();

            if (resourceController != null)
            {
                resourceController.ChangeHealth(-damage);
            }

            Destroy(gameObject);
        }

        Destroy(gameObject);
    }
}
