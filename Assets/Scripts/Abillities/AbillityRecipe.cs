using UnityEngine;
using System.Collections.Generic;

// Unity 에디터에서 에셋을 생성할 수 있도록 메뉴 아이템 추가
[CreateAssetMenu(fileName = "NewAbilityRecipe", menuName = "Ability System/Ability Recipe")]
public class AbilityRecipe : ScriptableObject
{
    // 합성 재료로 필요한 능력의 정보
    [System.Serializable]
    public class RequiredAbility
    {
        // 필요한 능력의 프리팹 (이 프리팹 자체가 해당 능력의 고유 식별자 역할을 함)
        public GameObject AbilityPrefab;
        // 필요한 최소 레벨
        public int RequiredLevel;
    }

    public string RecipeName = "새로운 합성 능력";
    [TextArea(3, 5)] // 여러 줄 입력 가능하도록 에디터 UI 개선
    public string Description = "두 능력을 합성하여 더 강력한 능력을 만듭니다.";

    [Header("합성에 필요한 능력")]
    // 합성 재료 능력 목록
    public List<RequiredAbility> RequiredAbilities;

    [Header("합성 결과 능력")]
    // 합성으로 생성될 능력의 프리팹
    public GameObject CombinedAbilityPrefab;
}