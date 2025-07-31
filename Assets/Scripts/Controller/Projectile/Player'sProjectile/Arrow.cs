using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{

    [Header("Ranged Attack Data")]
    [SerializeField] private Transform projectileSpawnPosition;

    [SerializeField] private int arrowIndex;                                                // 화살 
    public int ArrowIndex { get => arrowIndex; set => arrowIndex = value; }                 // 참조 - ArrowIndex

    [SerializeField] private float arrowSize = 1;                                           // 화살 크기
    public float ArrowSize { get => arrowSize; set => arrowSize = value; }                  // 참조 - ArrowSize

    [SerializeField] private float duration;
    public float Duration { get => duration; set => duration = value; }

    [SerializeField] private float spread;
    public float Spread { get => spread; set => spread = value; }

    [SerializeField] private int numberofProjectilesPerShot;                                // 화살 동시 발사 개수
    public int NumberofProjectilesPerShot                                                   // 참조 - NumberofProjectilesPerShot
    { get => numberofProjectilesPerShot; set => numberofProjectilesPerShot = value; }

    [SerializeField] private float multipleProjectilesAngel;
    public float MultipleProjectilesAngel
    { get => multipleProjectilesAngel; set => multipleProjectilesAngel = value; }

    [SerializeField] private Color projectileColor;                                         // 화살 색상
    public Color ProjectileColor                                                            // 참조 - ProjectileColor
    { get => projectileColor; set => projectileColor = value; }

    public override void Attack()
    {
        base.Attack();

        float projectilesAngleSpace = multipleProjectilesAngel;
        int numberOfProjectilesPerShot = numberofProjectilesPerShot;

        float minAngle = -(numberOfProjectilesPerShot / 2f) * projectilesAngleSpace + 0.5f * multipleProjectilesAngel;

        for (int i = 0; i < numberOfProjectilesPerShot; i++)
        {
            float angle = minAngle + projectilesAngleSpace * i;
            float randomSpread = Random.Range(-spread, spread);
            angle += randomSpread;
            CreateProjectile(Controller.LookDirection, angle);
        }
    }

    private void CreateProjectile(Vector2 _lookDirection, float angle)
    {

    }
    private static Vector2 RotateVector2(Vector2 v, float degree)
    {
        return Quaternion.Euler(0, 0, degree) * v;
    }
}
