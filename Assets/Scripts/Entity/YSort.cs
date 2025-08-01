using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Y ��ǥ�� ���� SpriteRenderer�� sortingOrder�� �ڵ� ����
// Player, Enemy, Obstacle, Projectile �� ���� ������ �̹��� ������ �߿��� ������Ʈ�� ����

public class YSort : MonoBehaviour
{
    [SerializeField] private int minOrder = 2;
    [SerializeField] private int maxOrder = 999;
    [SerializeField] private float minY = -1.2f;
    [SerializeField] private float maxY = 3.7f;


    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        float y = transform.position.y;

        // ����ȭ ( 0 ~ 1 )
        float normalizedY = Mathf.InverseLerp(minY, maxY, y);   // = (y - minY) / (maxY - minY)
        
        // ���� �� ����
        float rawOrder = Mathf.Lerp(maxOrder, minOrder, normalizedY);   // �ִ�, �ּ�, ����

        // ������ ����ȭ�ؼ� ����
        spriteRenderer.sortingOrder = Mathf.RoundToInt(rawOrder);
    }
}
