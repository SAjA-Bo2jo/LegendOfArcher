using System.Collections;
using UnityEngine;

public class BossEnemyAttack : IEnemyAttack
{
    private bool isCharging = false;
    private float chargeTimer = 0f;

    public bool CanAttack(EnemyController controller)
    {
        // Debug.Log("CanAttack 호출됨: " + !isCharging);
        return !isCharging;
    }

    public void Attack(EnemyController controller)
    {
        // Debug.Log("보스 대검 공격!");

        Transform bigSword = controller.transform.Find("WeaponPivot/Weapon/BigSword");
        if (bigSword != null)
        {
            bigSword.GetComponent<Animator>()?.SetTrigger("Attack");
            bigSword.GetComponent<EnemyMeleeWeaponAttack>()?.StartAttack(controller.Stats.attackDamage);
        }
    }

    public void Update(EnemyController controller)
    {
        // Debug.Log("BossEnemyAttack.Update() 실행 중");
        chargeTimer += Time.deltaTime;

        // 10초마다 돌진
        if (chargeTimer >= 10f)
        {
            Debug.Log("보스 돌진 시작!");
            if (controller is MonoBehaviour mb)
                controller.StartCoroutine(ChargeAttack(controller));
            chargeTimer = 0f;
        }
    }

    private IEnumerator ChargeAttack(EnemyController controller)
    {
        isCharging = true;
        Debug.Log("돌진 준비 중...");

        Rigidbody2D rb = controller.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(1f);

        Debug.Log("돌진 실행!");
        Vector2 direction = controller.DirectionToTarget();
        float chargeSpeed = controller.Stats.moveSpeed * 3f;

        float time = 0f;
        while (time < 1f)
        {
            rb.velocity = direction * chargeSpeed;
            time += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        isCharging = false;
        Debug.Log("돌진 완료!");
    }

    public bool IsCharging() => isCharging;
}