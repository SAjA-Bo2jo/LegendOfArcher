using UnityEngine;

public class FireArrow : Ability
{
    [SerializeField] private int[] attackCountForFireArrowPerLevel = { 3, 2, 2, 1, 1 };
    [SerializeField] private float[] damageMultiplierPerLevel = { 1.5f, 1.7f, 2.0f, 2.2f, 2.5f };

    private int currentAttackCount = 0;
    private const string FIRE_ARROW_POOL_KEY = "FireArrow";

    public override void OnAcquire(Player playerInstance)
    {
        base.OnAcquire(playerInstance);
        currentAttackCount = 0;
        UpdateDescription();
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} 획득/강화: {Description}");
    }

    public override void OnRemove()
    {
        base.OnRemove();
        Debug.Log($"[{AbilityName}] 효과 제거됨.");
    }

    public override void ApplyEffect()
    {
        UpdateDescription();
    }

    private void UpdateDescription()
    {
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel)
        {
            description = $"매 {attackCountForFireArrowPerLevel[CurrentLevel - 1]}번째 공격 시 불화살 발사 (데미지 {damageMultiplierPerLevel[CurrentLevel - 1]}배)";
        }
        else
        {
            description = "불화살 능력 (획득 대기)";
        }
    }

    /// <summary>
    /// 일반 화살 대신 불화살 발사를 시도합니다. Player 스크립트에서 호출됩니다.
    /// </summary>
    /// <param name="regularArrowGO">발사하려던 일반 화살 GameObject</param>
    /// <param name="regularArrowScript">발사하려던 일반 화살 Arrow 컴포넌트</param>
    /// <returns>불화살이 발사되었으면 true, 아니면 false</returns>
    public bool TryActivateFireArrow(GameObject regularArrowGO, Arrow regularArrowScript)
    {
        // 공격 횟수를 증가시킵니다.
        currentAttackCount++;

        // --- 디버그 로그 ---
        Debug.Log($"[FireArrow] TryActivateFireArrow 호출됨. 현재 공격 횟수: {currentAttackCount}");
        if (CurrentLevel > 0)
        {
            Debug.Log($"[FireArrow] 조건 확인: 현재 레벨: {CurrentLevel}, 필요 공격 횟수: {attackCountForFireArrowPerLevel[CurrentLevel - 1]}");
        }
        // --- 디버그 로그 끝 ---

        // 조건이 충족되었을 경우에만 불화살을 발사합니다.
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel &&
            currentAttackCount >= attackCountForFireArrowPerLevel[CurrentLevel - 1])
        {
            // 불화살 발사 조건이 충족되었으므로, 현재 공격 횟수를 리셋합니다.
            currentAttackCount = 0;
            Debug.Log($"FireArrow: 불화살 발동! (Lv.{CurrentLevel})");

            // 기존 일반 화살은 풀로 반환합니다.
            ObjectPoolManager.Instance.Return("Arrow", regularArrowGO);

            // 불화살을 오브젝트 풀에서 가져옵니다.
            GameObject fireArrowGO = ObjectPoolManager.Instance.Get(FIRE_ARROW_POOL_KEY);
            if (fireArrowGO == null)
            {
                Debug.LogError($"FireArrow: 불화살 오브젝트 풀에서 '{FIRE_ARROW_POOL_KEY}'를 가져오지 못했습니다.");
                return false;
            }

            Arrow fireArrowScript = fireArrowGO.GetComponent<Arrow>();
            if (fireArrowScript == null)
            {
                Debug.LogError("FireArrow: 불화살 Prefab에 Arrow 스크립트가 없습니다!");
                ObjectPoolManager.Instance.Return(FIRE_ARROW_POOL_KEY, fireArrowGO);
                return false;
            }

            fireArrowGO.transform.position = regularArrowGO.transform.position;
            fireArrowGO.transform.rotation = regularArrowGO.transform.rotation;
            fireArrowScript.transform.localScale = regularArrowScript.transform.localScale;

            Rigidbody2D fireArrowRb = fireArrowGO.GetComponent<Rigidbody2D>();
            if (fireArrowRb != null)
            {
                fireArrowRb.isKinematic = false;
                fireArrowRb.simulated = true;
            }

            if (player == null)
            {
                Debug.LogError("FireArrow: Player 참조가 null입니다. 스탯을 설정할 수 없습니다.");
                ObjectPoolManager.Instance.Return(FIRE_ARROW_POOL_KEY, fireArrowGO);
                return false;
            }

            fireArrowScript.Setup(
                damage: player.AttackDamage * damageMultiplierPerLevel[CurrentLevel - 1],
                size: player.AttackSize,
                critRate: player.CriticalRate,
                speed: player.ProjectileSpeed
            );
            fireArrowScript.LaunchTowards(fireArrowGO.transform.right);

            return true; // 불화살 발사에 성공했으므로 true 반환
        }

        // 조건이 충족되지 않았다면 false를 반환합니다.
        return false;
    }
}