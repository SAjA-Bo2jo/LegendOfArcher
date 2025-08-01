using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Y 좌표에 따라 SpriteRenderer의 sortingOrder를 자동 조정
// Player, Enemy, Obstacle, Projectile 등 던전 내에서 이미지 순서가 중요한 오브젝트에 부착

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

        // 정규화 ( 0 ~ 1 )
        float normalizedY = Mathf.InverseLerp(minY, maxY, y);   // = (y - minY) / (maxY - minY)
        
        // 정렬 값 보간
        float rawOrder = Mathf.Lerp(maxOrder, minOrder, normalizedY);   // 최대, 최소, 비율

        // 보간값 정수화해서 적용
        spriteRenderer.sortingOrder = Mathf.RoundToInt(rawOrder);
    }
}
