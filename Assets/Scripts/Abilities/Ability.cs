// Ability.cs
using UnityEngine;

public class Ability : MonoBehaviour
{
    [Header("Base Ability Info")]
    public string AbilityName;
    [TextArea(3, 5)]
    public string Description;
    public Sprite AbilityIcon;
    public GameObject AbilityPrefab;

    // 모든 어빌리티에서 공통으로 사용될 변수들
    public Player player; // 이제 public으로 선언하여 외부에서 직접 할당 가능
    protected GameObject target;
    public int CurrentLevel { get; protected set; }
    public int MaxLevel = 3;

    // Start()는 virtual로 두어 자식 클래스에서 재정의할 수 있게 합니다.
    protected virtual void Start()
    {
        // 더 이상 GetComponentInParent를 사용하지 않습니다.
    }

    // 외부에서 플레이어 참조를 설정하는 메서드
    public void SetPlayer(Player playerInstance)
    {
        this.player = playerInstance;
    }

    public void InitializeAbility(GameObject abilityPrefab)
    {
        this.AbilityPrefab = abilityPrefab;
        CurrentLevel = 1;
        Debug.Log($"InitializeAbility 호출됨: {this.AbilityName}, 현재 레벨: {CurrentLevel}");
    }

    public virtual void OnAcquire(Player player)
    {
        CurrentLevel++;
        Debug.Log($"OnAcquire 호출됨: {this.AbilityName}, 레벨업 후: {CurrentLevel}");
    }

    public virtual void ApplyEffect()
    {
        // 개별 어빌리티에 따라 다른 효과를 적용하는 로직
    }

    public virtual void OnRemove()
    {
        Debug.Log($"OnRemove 호출됨: {this.AbilityName}");
    }
}