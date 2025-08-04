using UnityEngine;

public abstract class Abillity : MonoBehaviour
{
    // 능력이 적용될 플레이어 참조
    protected Player player;
    // 애니메이션 핸들러 (선택 사항, 필요시 하위 클래스에서 사용)
    protected AnimationHandler animationHandler;
    // 현재 타겟 (일부 능력에서 필요할 수 있음)
    protected GameObject target;

    // 마지막 공격 시간 (Bow 스크립트에서 사용)
    public float lastAttackTime = -999f;

    // 현재 능력 레벨
    public int CurrentLevel { get; protected set; } = 0;
    // 최대 레벨
    public int MaxLevel { get; protected set; } = 5;

    // 이 능력이 활성화될 때 참조할 프리팹 (능력 선택 UI 등에서 활용)
    public GameObject AbilityPrefab { get; private set; }

    // 이 능력에 대한 설명, 아이콘 등을 위한 데이터
    public string AbilityName { get; protected set; }
    public string Description { get; protected set; }
    public Sprite AbilityIcon { get; protected set; } // <--- 이 줄을 추가합니다.

    // 능력이 생성될 때 호출되며, 이 능력의 프리팹 정보를 할당합니다.
    public virtual void InitializeAbility(GameObject prefab)
    {
        AbilityPrefab = prefab;
    }

    // 능력을 획득하거나 레벨업 할 때 호출될 메서드
    public virtual void OnAcquire(Player playerInstance)
    {
        this.player = playerInstance; // 플레이어 인스턴스 할당
        CurrentLevel++; // 레벨 증가
        ApplyEffect(); // 효과 적용
    }

    // 능력이 제거될 때 (예: 합성으로 사라질 때) 호출될 메서드
    public virtual void OnRemove()
    {
        CurrentLevel = 0; // 레벨 초기화
    }

    // 각 능력의 실제 효과를 적용하는 메서드 (하위 클래스에서 반드시 구현)
    public abstract void ApplyEffect();
    // 각 능력의 효과를 제거하는 메서드 (하위 클래스에서 반드시 구현)

    // 선택적으로 각 능력 클래스에서 Update 로직을 가질 수 있습니다.
    // 하지만, Player 또는 PlayerController에서 이 메서드를 호출해주어야 합니다.
    // 또는 각 Abillity를 MonoBehaviour로 만들고 GameObject에 붙여서 자체 Update를 사용할 수도 있습니다.
    // 이 예시에서는 MonoBehaviour를 상속받으므로 자체 Update를 사용할 수 있습니다.
    public virtual void UpdateAbility() { }
}