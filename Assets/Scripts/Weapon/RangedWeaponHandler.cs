using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeaponHandler : WeaponHandler
{
    [Header("Ranged Attack Data")]
    [SerializeField] private Transform projectileSpawnPosition;

    [SerializeField] private int bulletIndex;
    public int BulletIndex { get => bulletIndex; set => bulletIndex = value; }

    [SerializeField] private float bulletSize = 1;
    public float BulletSize { get => bulletSize; set => bulletSize = value; }

    [SerializeField] private float duration;
    public float Duration { get => duration; set => duration = value; }

    [SerializeField] private float spread;
    public float Spread { get => spread; set => spread = value; }

    [SerializeField] private int numberofProjectilesPerShot;
    public int NumberofProjectilesPerShot { get => numberofProjectilesPerShot; set => numberofProjectilesPerShot = value; }

    [SerializeField] private float multipleProjectilesAngel;
    public float MultipleProjectilesAngel { get => multipleProjectilesAngel; set => multipleProjectilesAngel = value; }

    [SerializeField] private Color projectileColor;
    public Color ProjectileColor { get => projectileColor; set => projectileColor = value; }

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
