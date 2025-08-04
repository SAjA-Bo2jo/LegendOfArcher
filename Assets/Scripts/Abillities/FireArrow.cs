using UnityEngine;

public class FireArrow : Abillity
{
    // 레벨별 요구 공격 횟수 (예: 3, 2, 2, 1, 1)
    [SerializeField] private int[] attackCountForFireArrowPerLevel = { 3, 2, 2, 1, 1 };
    // 레벨별 불화살 데미지 배율 (예: 1.5, 1.7, 2.0, 2.2, 2.5)
    [SerializeField] private float[] damageMultiplierPerLevel = { 1.5f, 1.7f, 2.0f, 2.2f, 2.5f };

    private int currentAttackCount = 0; // 현재 공격 횟수 카운터

    // 불화살 오브젝트 풀 키 (ObjectPoolManager에서 사용할 이름)
    private const string FIRE_ARROW_POOL_KEY = "FireArrow";

    void Awake()
    {
        AbilityName = "불화살";
        MaxLevel = attackCountForFireArrowPerLevel.Length;
        InitializeAbility(this.gameObject);
    }

    // 능력을 획득하거나 레벨업 할 때 호출
    public override void OnAcquire(Player playerInstance)
    {
        base.OnAcquire(playerInstance); // 부모 클래스 OnAcquire 호출 (레벨 증가)
        currentAttackCount = 0; // 능력 획득/레벨업 시 카운트 초기화
        UpdateDescription(); // 능력 설명 업데이트
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} 획득/강화: {Description}");
    }

    // 능력이 제거될 때 호출
    public override void OnRemove()
    {
        base.OnRemove();
        Debug.Log($"[{AbilityName}] 효과 제거됨.");
    }

    // 이 능력은 스탯 변경이 아닌 조건부 발동이므로 ApplyEffect에서는 특별한 스탯 변경 없음
    public override void ApplyEffect()
    {
        // 주로 UpdateDescription()을 호출하여 UI에 표시될 설명을 업데이트
        UpdateDescription();
    }

    // 제거 시 특별한 스탯 변경 없음
    public override void RemoveEffect()
    {
        // 특별히 해야 할 일 없음
    }

    // 능력 설명을 현재 레벨에 맞춰 업데이트
    private void UpdateDescription()
    {
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel)
        {
            Description = $"매 {attackCountForFireArrowPerLevel[CurrentLevel - 1]}번째 공격 시 불화살 발사 (데미지 {damageMultiplierPerLevel[CurrentLevel - 1]}배)";
        }
        else
        {
            Description = "불화살 능력 (획득 대기)";
        }
    }

    /// <summary>
    /// 일반 화살 대신 불화살 발사를 시도합니다. Bow 스크립트에서 호출됩니다.
    /// </summary>
    /// <param name="regularArrowGO">발사하려던 일반 화살 GameObject</param>
    /// <param name="regularArrowScript">발사하려던 일반 화살 Arrow 컴포넌트</param>
    /// <returns>불화살이 발사되었으면 true, 아니면 false</returns>
    public bool TryActivateFireArrow(GameObject regularArrowGO, Arrow regularArrowScript)
    {
        currentAttackCount++;
        // 현재 레벨이 유효하고, 공격 카운트가 요구 횟수에 도달했을 때
        if (CurrentLevel > 0 && CurrentLevel <= MaxLevel &&
            currentAttackCount >= attackCountForFireArrowPerLevel[CurrentLevel - 1])
        {
            currentAttackCount = 0; // 카운트 초기화

            Debug.Log($"불화살 발동! (Lv.{CurrentLevel})");

            // 기존 일반 화살은 풀로 반환
            ObjectPoolManager.Instance.Return("Arrow", regularArrowGO); // "Arrow"는 일반 화살의 풀 키라고 가정

            // 불화살을 풀에서 가져옵니다.
            GameObject fireArrowGO = ObjectPoolManager.Instance.Get(FIRE_ARROW_POOL_KEY);
            if (fireArrowGO == null)
            {
                Debug.LogError($"불화살 오브젝트 풀에서 '{FIRE_ARROW_POOL_KEY}'를 가져오지 못했습니다.");
                return false;
            }

            // 불화살의 Arrow 컴포넌트를 가져옵니다.
            Arrow fireArrowScript = fireArrowGO.GetComponent<Arrow>();
            if (fireArrowScript == null)
            {
                Debug.LogError("불화살 Prefab에 Arrow 스크립트가 없습니다!");
                ObjectPoolManager.Instance.Return(FIRE_ARROW_POOL_KEY, fireArrowGO);
                return false;
            }

            // 불화살의 위치, 회전, 스케일을 일반 화살과 동일하게 설정
            fireArrowGO.transform.position = regularArrowGO.transform.position;
            fireArrowGO.transform.rotation = regularArrowGO.transform.rotation;
            fireArrowScript.transform.localScale = regularArrowScript.transform.localScale; // 크기 유지

            // 불화살의 스탯을 설정합니다. (플레이어 스탯 기반 + 레벨별 배율)
            fireArrowScript.Setup(
                damage: player.AttackDamage * damageMultiplierPerLevel[CurrentLevel - 1],
                size: player.AttackSize,
                critRate: player.CriticalRate,
                speed: player.ProjectileSpeed
            );

            // Rigidbody 활성화 및 발사
            Rigidbody2D fireArrowRb = fireArrowGO.GetComponent<Rigidbody2D>();
            if (fireArrowRb != null)
            {
                fireArrowRb.isKinematic = false;
                fireArrowRb.simulated = true;
            }
            fireArrowScript.LaunchTowards(fireArrowGO.transform.right); // 활의 방향으로 발사

            return true; // 불화살 발사 성공
        }
        return false; // 불화살 발사 조건 미충족
    }
}