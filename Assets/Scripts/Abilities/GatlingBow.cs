using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GatlingBow : Ability // Ability 클래스를 상속받습니다.
{
    [Header("개틀링 보우 능력 설정")]
    [SerializeField] private Sprite gatlingBowAbilityIcon; // 능력 선택 UI용 아이콘
    [SerializeField] private int arrowsPerShot = 3; // 한 번에 발사될 화살 수
    [SerializeField] private float spreadAngle = 10f; // 화살이 퍼지는 각도
    [SerializeField] private float fireIntervalBetweenArrows = 0.05f; // 다연장 발사 시 화살 간 시간 간격

    [Header("스탯 변경")]
    [SerializeField] private float attackSpeedBonus = 0.5f; // 플레이어의 AttackSpeed에 더해질 보너스

    // 개틀링 화살 오브젝트 풀 키
    private const string GATLING_ARROW_POOL_KEY = "GatlingArrow";

    // 능력을 획득하거나 레벨업 할 때 호출
    public override void OnAcquire(Player playerInstance)
    {
        base.OnAcquire(playerInstance); // 부모 클래스 OnAcquire 호출 (레벨 증가)
        // this.player는 Ability 클래스에서 이미 할당됨
        UpdateDescription(); // 능력 설명 업데이트
        Debug.Log($"[{AbilityName}] Lv.{CurrentLevel} 획득/강화: {Description}");
    }

    // 능력이 제거될 때 호출
    public override void OnRemove()
    {
        base.OnRemove();
        Debug.Log($"[{AbilityName}] 효과 제거됨.");
    }

    // 이 능력이 활성화될 때 플레이어 스탯에 영향을 줍니다.
    public override void ApplyEffect()
    {
        if (player != null)
        {
            player.AttackSpeed += attackSpeedBonus; // 플레이어의 기본 공격 속도에 보너스 추가
            // Player.RecalculateStats()에서 이 값들이 최종 MaxAttackSpeed에 반영됩니다.
        }
        UpdateDescription();
    }

    // 능력 설명을 현재 레벨에 맞춰 업데이트
    private void UpdateDescription()
    {
        if (CurrentLevel > 0)
        {
            description = $"플레이어의 공격 속도를 {attackSpeedBonus:F1} 증가시키고, 매 공격 시 {arrowsPerShot}발의 특수 화살을 발사합니다.";
        }
        else
        {
            description = "개틀링 보우 능력 (획득 대기)";
        }
    }

    /// <summary>
    /// 일반 화살 대신 개틀링 화살 발사를 시도합니다. Player 스크립트에서 호출됩니다.
    /// </summary>
    /// <param name="regularArrowGO">발사하려던 일반 화살 GameObject (풀로 반환됩니다)</param>
    /// <param name="regularArrowScript">발사하려던 일반 화살 Arrow 컴포넌트</param>
    /// <param name="playerInstance">현재 플레이어 인스턴스 (스탯 참조용)</param>
    /// <returns>개틀링 화살이 발사되었으면 true, 아니면 false</returns>
    public bool TryActivateGatlingArrow(GameObject regularArrowGO, Arrow regularArrowScript, Player playerInstance)
    {
        // GatlingBow 능력은 능동적으로 발동되는 것이므로, CurrentLevel만 확인하면 됩니다.
        if (CurrentLevel > 0) // 능력이 활성화(획득)되어 있다면
        {
            Debug.Log($"GatlingBow: 개틀링 보우 발동!");

            // 기존 일반 화살은 풀로 반환
            ObjectPoolManager.Instance.Return("Arrow", regularArrowGO);

            // 다연장 화살 발사 코루틴 시작
            // 기존 화살의 위치와 회전을 기반으로 합니다.
            StartCoroutine(FireMultipleGatlingArrows(regularArrowGO.transform.position, regularArrowGO.transform.rotation, regularArrowScript.transform.localScale, playerInstance));

            return true; // 개틀링 화살 발사 시작
        }
        return false; // 개틀링 보우 능력 조건 미충족
    }

    // 여러 발의 개틀링 화살을 발사하는 코루틴
    private IEnumerator FireMultipleGatlingArrows(Vector3 startPosition, Quaternion startRotation, Vector3 startScale, Player playerInstance)
    {
        // Player의 LookDirection (현재 타겟 방향)을 사용하여 화살 발사 방향을 결정
        // PlayerController는 Player의 멤버이므로, playerInstance.GetComponent<PlayerController>()로 접근할 수 있습니다.
        PlayerController playerController = playerInstance.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("GatlingBow: PlayerController를 찾을 수 없습니다.");
            yield break;
        }

        Vector3 baseDirection = playerController.LookDirection; // 플레이어가 현재 바라보는 방향 (Bow가 타겟을 향하게 회전시키는 방향)
        float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;

        for (int i = 0; i < arrowsPerShot; i++)
        {
            // 각 화살의 발사 방향에 퍼지는 각도 적용
            float minSpread = -spreadAngle / 2f;
            float maxSpread = spreadAngle / 2f;
            float randomSpread = Random.Range(minSpread, maxSpread);

            Quaternion rotation = Quaternion.Euler(0, 0, baseAngle + randomSpread);
            Vector3 launchDirection = rotation * Vector3.right; // Z축 회전 후 X축 방향

            // 개틀링 화살을 풀에서 가져옵니다.
            GameObject gatlingArrowGO = ObjectPoolManager.Instance.Get(GATLING_ARROW_POOL_KEY);
            if (gatlingArrowGO == null)
            {
                Debug.LogError($"GatlingBow: 오브젝트 풀에서 '{GATLING_ARROW_POOL_KEY}'를 가져오지 못했습니다.");
                break; // 더 이상 화살을 발사할 수 없음
            }

            // 개틀링 화살의 Arrow 컴포넌트를 가져옵니다.
            Arrow gatlingArrowScript = gatlingArrowGO.GetComponent<Arrow>();
            if (gatlingArrowScript == null)
            {
                Debug.LogError("GatlingBow: 개틀링 화살 Prefab에 Arrow 스크립트가 없습니다!");
                ObjectPoolManager.Instance.Return(GATLING_ARROW_POOL_KEY, gatlingArrowGO);
                continue;
            }

            // 물리 설정 활성화
            Rigidbody2D gatlingArrowRb = gatlingArrowGO.GetComponent<Rigidbody2D>();
            if (gatlingArrowRb != null)
            {
                gatlingArrowRb.isKinematic = false;
                gatlingArrowRb.simulated = true;
            }

            // 화살의 위치, 회전, 스케일을 설정
            // 시작 위치는 Bow의 FirePoint와 동일하게 사용하는 것이 좋습니다.
            // 여기서는 regularArrowGO.transform.position (즉, Bow의 FirePoint 위치)를 그대로 사용합니다.
            gatlingArrowGO.transform.position = startPosition;
            gatlingArrowGO.transform.rotation = rotation;
            gatlingArrowScript.transform.localScale = startScale; // 일반 화살의 크기 유지
            gatlingArrowGO.SetActive(true);

            // 개틀링 화살의 스탯을 설정합니다. (플레이어 스탯 기반)
            gatlingArrowScript.Setup(
                damage: playerInstance.AttackDamage,
                size: playerInstance.AttackSize,
                critRate: playerInstance.CriticalRate,
                speed: playerInstance.ProjectileSpeed
            );
            gatlingArrowScript.LaunchTowards(launchDirection); // 화살 발사

            yield return new WaitForSeconds(fireIntervalBetweenArrows); // 다음 화살 발사까지 대기
        }
    }
}