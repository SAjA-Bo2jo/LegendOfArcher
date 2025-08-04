using System.Collections.Generic;
using UnityEngine;

// ���� MonoBehaviour�� ��ӹ޾� ������Ʈ�� �˴ϴ�.
// �� ��ũ��Ʈ�� ���� GameObject�� Player�� abilityRecipes�� �Ҵ��մϴ�.
public class AbilityRecipe : MonoBehaviour
{
    public GameObject CombinedAbilityPrefab;
    public List<AbilityRequirement> RequiredAbilities;

    public bool CanCombine(Player player)
    {
        if (player == null || RequiredAbilities == null) return false;

        foreach (var req in RequiredAbilities)
        {
            // �ʼ� �����Ƽ�� �����ϰ� �ִ��� Ȯ��
            if (!player.activeAbilities.ContainsKey(req.AbilityPrefab))
            {
                return false;
            }

            // �ʼ� �����Ƽ�� �ʿ��� ���� �̻����� Ȯ��
            if (player.activeAbilities[req.AbilityPrefab].CurrentLevel < req.RequiredLevel)
            {
                return false;
            }
        }
        return true;
    }
}

// AbilityRequirement�� ���� �ʿ��� ���� ������ �����մϴ�.
[System.Serializable]
public class AbilityRequirement
{
    public GameObject AbilityPrefab;
    public int RequiredLevel; // <-- �ʿ��� �����Ƽ�� �ּ� ����
}