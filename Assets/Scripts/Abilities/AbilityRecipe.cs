using System.Collections.Generic;
using UnityEngine;

// 이제 MonoBehaviour를 상속받아 컴포넌트가 됩니다.
// 이 스크립트를 가진 GameObject를 Player의 abilityRecipes에 할당합니다.
public class AbilityRecipe : MonoBehaviour
{
    public GameObject CombinedAbilityPrefab;
    public List<AbilityRequirement> RequiredAbilities;

    public bool CanCombine(Player player)
    {
        if (player == null || RequiredAbilities == null) return false;

        foreach (var req in RequiredAbilities)
        {
            // 필수 어빌리티를 보유하고 있는지 확인
            if (!player.activeAbilities.ContainsKey(req.AbilityPrefab))
            {
                return false;
            }

            // 필수 어빌리티가 필요한 레벨 이상인지 확인
            if (player.activeAbilities[req.AbilityPrefab].CurrentLevel < req.RequiredLevel)
            {
                return false;
            }
        }
        return true;
    }
}

// AbilityRequirement는 이제 필요한 레벨 정보를 포함합니다.
[System.Serializable]
public class AbilityRequirement
{
    public GameObject AbilityPrefab;
    public int RequiredLevel; // <-- 필요한 어빌리티의 최소 레벨
}