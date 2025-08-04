using System.Linq;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    protected Player player; // 능력이 적용될 플레이어 참조
    protected AnimationHandler animationHandler; // 애니메이션 핸들러 (선택 사항)
    protected GameObject target; // 현재 타겟 (일부 능력에서 필요)

    public float lastAttackTime = -999f; // 마지막 공격 시간 (활 같은 무기 능력에서 사용)

    public int CurrentLevel { get; protected set; } = 0; // 현재 능력 레벨
    public int MaxLevel { get; protected set; } = 5; // 최대 레벨

    public GameObject AbilityPrefab { get; private set; } // 이 능력을 생성한 원본 프리팹

    public string AbilityName { get; protected set; } // 능력 이름
    public string Description { get; protected set; } // 능력 설명
    public Sprite AbilityIcon { get; protected set; } // 능력 아이콘 (UI용)

    public virtual void InitializeAbility(GameObject prefab)
    {
        AbilityPrefab = prefab;
    }

    public virtual void OnAcquire(Player playerInstance)
    {
        this.player = playerInstance; // 플레이어 인스턴스 할당
        CurrentLevel++; // 레벨 증가
        ApplyEffect(); // 효과 적용 (하위 클래스에서 구현됨)
    }

    public virtual void OnRemove()
    {
        CurrentLevel = 0; // 레벨 초기화
        // 제거 시 스탯 복구 로직이 필요하다면 여기에 추가 (RecalculateStats()가 처리하는 경우 많음)
    }

    // 각 능력의 실제 효과를 적용하는 메서드 (하위 클래스에서 반드시 구현)
    public abstract void ApplyEffect();

    // 선택적으로 각 능력 클래스에서 Update 로직을 가질 수 있습니다.
    public virtual void UpdateAbility() { }

    // Utility: 타겟 찾기 (일부 Ability에서 필요할 수 있음)
    // Bow에서 사용하므로, Bow는 이 메서드를 재정의하거나,
    // Bow에서 FindTarget()을 직접 호출하는 대신 Player.GetNearestEnemy()를 사용할 수 있습니다.
    // 여기서는 Bow가 FindTarget()을 자체적으로 구현하므로 이 메서드는 Ability의 하위 능력들이
    // 타겟이 필요할 때 사용하도록 남겨둡니다.
    protected GameObject FindTarget()
    {
        if (player == null) return null; // Player 컴포넌트가 없으면 타겟 탐색 불가

        // 플레이어의 위치를 기준으로 가장 가까운 적을 찾습니다.
        GameObject target = GameObject
            .FindGameObjectsWithTag("Enemy")
            .Where(enemy => enemy != null && enemy.activeInHierarchy && enemy.transform != null) // null 체크 및 활성 상태 체크
            .Where(enemy => Vector3.Distance(enemy.transform.position, player.transform.position) < player.AttackRange) // 플레이어 공격 범위 내 적 필터링
            .OrderBy(enemy => Vector3.Distance(enemy.transform.position, player.transform.position)) // 거리순 정렬
            .FirstOrDefault(); // 가장 가까운 적 선택
        return target;
    }
}