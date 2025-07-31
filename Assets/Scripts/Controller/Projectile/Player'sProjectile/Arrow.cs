using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{

    [Header("Ranged Attack Data")]
    [SerializeField] private Transform projectileSpawnPosition;

    [SerializeField] private int arrowIndex;                                                // ȭ�� 
    public int ArrowIndex { get => arrowIndex; set => arrowIndex = value; }                 // ���� - ArrowIndex

    [SerializeField] private float arrowSize = 1;                                           // ȭ�� ũ��
    public float ArrowSize { get => arrowSize; set => arrowSize = value; }                  // ���� - ArrowSize

    [SerializeField] private float duration;
    public float Duration { get => duration; set => duration = value; }

    [SerializeField] private float spread;
    public float Spread { get => spread; set => spread = value; }

    [SerializeField] private int numberofProjectilesPerShot;                                // ȭ�� ���� �߻� ����
    public int NumberofProjectilesPerShot                                                   // ���� - NumberofProjectilesPerShot
    { get => numberofProjectilesPerShot; set => numberofProjectilesPerShot = value; }

    [SerializeField] private float multipleProjectilesAngel;
    public float MultipleProjectilesAngel
    { get => multipleProjectilesAngel; set => multipleProjectilesAngel = value; }

    [SerializeField] private Color projectileColor;                                         // ȭ�� ����
    public Color ProjectileColor                                                            // ���� - ProjectileColor
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
