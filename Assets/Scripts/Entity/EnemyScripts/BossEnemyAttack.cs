using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BossEnemyAttack : IEnemyAttack
{
    private float lastAttackTime = 0f;

    // 돌진 기믹 변수
    private bool isCharging = false;                // 돌진 중 여부 판단
    private bool canCharge = true;                  // 돌진 가능한 지 여부 판단
    private float chargeTimer = 0f;                 // 쿨타임 계산용 변수
    private float chargeCooldown = 10f;             // 돌진 쿨타임
    private float chargeDuration = 1f;              // 돌진 지속 시간

    private float chargeDamageMultiplier = 3f;      // 돌진 데미지 배율
    private float chargeSpeedMultiplier = 3f;       // 돌진 이동속도 배율

    public float GetSwordDamage(EnemyController controller) => controller.Stats.attackDamage;
    public float GetChargeDamage(EnemyController controller) => controller.Stats.contactDamage * chargeDamageMultiplier;
    public float GetBodyDamage(EnemyController controller) => controller.Stats.contactDamage;
    public bool IsCharging() => isCharging;

    public bool CanAttack(EnemyController controller)
    {
        return (!isCharging &&
                Time.time > lastAttackTime + controller.Stats.attackCooldown);
    }

    public void Attack(EnemyController controller)
    {
        
    }

    private void Update(EnemyController controller)
    {
        if (canCharge)
        {
            chargeTimer += Time.deltaTime;          // 쿨타임 체크
            if (chargeTimer >= chargeCooldown)
            {
                controller.StartCoroutine(ChargeAttack(controller));
            }
        }
    }

    private void StartSwardAttack(EnemyController controller)
    {
        Transform weaponPivot = controller.transform.Find("WeaponPivot");
        if (weaponPivot != null)
        {
            Transform weapon = weaponPivot.Find("Weapon");
            if (weapon != null)
            {
                Transform bigSword = weapon.Find("BigSword");
                if (bigSword != null)
                {
                    Animator swordAnimator = bigSword.GetComponent<Animator>();
                    if (swordAnimator != null)
                    {
                        swordAnimator.SetTrigger("Attack");

                        EnemyMeleeWeaponAttack meleeAttack = bigSword.GetComponent<EnemyMeleeWeaponAttack>();
                        if (meleeAttack != null)
                        {
                            meleeAttack.StartAttack(controller.Stats.attackDamage);
                        }
                    }

                }
            }
        }
    }

    private IEnumerator ChargeAttack(EnemyController controller)
    {
        canCharge = false;
        chargeTimer = 0f;
        isCharging = true;

        Vector2 originalVelocity = controller.GetComponent<Rigidbody2D>().velocity;
        controller.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        yield return new WaitForSeconds(1f);

        Vector2 chargeDirection = controller.DirectionToTarget();
        Rigidbody2D rigidbody = controller.GetComponent<Rigidbody2D>();
        float chargeSpeed = controller.Stats.moveSpeed * chargeSpeedMultiplier;

        float chargeTimeLeft = chargeDuration;
        while (chargeTimeLeft > 0f)
        {
            rigidbody.velocity = chargeDirection * chargeSpeed;
            chargeTimeLeft -= Time.deltaTime;
            yield return null;
        }

        rigidbody.velocity = Vector2.zero;
        isCharging = false;

        yield return new WaitForSeconds(2f); // 돌진 후 2초 쿨다운
        canCharge = true;
    }
}
