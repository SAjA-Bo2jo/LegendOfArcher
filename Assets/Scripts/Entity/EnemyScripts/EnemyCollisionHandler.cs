using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionHandler : MonoBehaviour
{
    [SerializeField] private float contactDamage = 5f;                          // contactDamage : �浹 ���ݷ�

    private EnemyAnimationHandler enemyAnimationHandler;

    private void Awake()
    {
        enemyAnimationHandler = GetComponentInParent<EnemyAnimationHandler>();

        if (enemyAnimationHandler == null )
        {
            Debug.Log($"{gameObject.name}: EnemyAnimationHandler ����");
        }
    }

    private void OnTriggerEnter2D(Collider2D playerCollider)                    // �浹 �� ���� �޼��� (�÷��̾�� �浹 ������ ����)
    {
        if (playerCollider.CompareTag("Player")) return;                        // �浹 ��ü�� Player �´� �� Ȯ��    
        if (enemyAnimationHandler != null &&
            enemyAnimationHandler.IsDead) return;                               // �� ��ü ��� ������ �� Ȯ��

        ApplyContactDamageToCollider(playerCollider);                           // �浹 ������ ����
        Debug.Log($"{transform.parent.name}�� �÷��̾�� ����, {contactDamage} ������");
    }

    private void ApplyContactDamageToCollider(Collider2D Collider)               // ���� �������� �ִ� �޼���
    {
        ResourceController Rescource = Collider.GetComponent<ResourceController>();
        if (Rescource != null)
        {
            Rescource.ChangeHealth(-contactDamage);
        }
        else
        {
            Debug.LogWarning($"Player�� ResourceController ���� : {Collider.name}");
        }

        AnimationHandler Animation = Collider.GetComponent<AnimationHandler>();
        if ( Animation != null )
        {
            Animation.Damage();
        }
        else
        {
            Debug.LogWarning($"Player�� AnimationHandler ���� : {Collider.name}");
        }
    }
}
