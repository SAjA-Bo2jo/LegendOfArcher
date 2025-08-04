using UnityEngine;
using System.Collections.Generic;

// Unity �����Ϳ��� ������ ������ �� �ֵ��� �޴� ������ �߰�
[CreateAssetMenu(fileName = "NewAbilityRecipe", menuName = "Ability System/Ability Recipe")]
public class AbilityRecipe : ScriptableObject
{
    // �ռ� ���� �ʿ��� �ɷ��� ����
    [System.Serializable]
    public class RequiredAbility
    {
        // �ʿ��� �ɷ��� ������ (�� ������ ��ü�� �ش� �ɷ��� ���� �ĺ��� ������ ��)
        public GameObject AbilityPrefab;
        // �ʿ��� �ּ� ����
        public int RequiredLevel;
    }

    public string RecipeName = "���ο� �ռ� �ɷ�";
    [TextArea(3, 5)] // ���� �� �Է� �����ϵ��� ������ UI ����
    public string Description = "�� �ɷ��� �ռ��Ͽ� �� ������ �ɷ��� ����ϴ�.";

    [Header("�ռ��� �ʿ��� �ɷ�")]
    // �ռ� ��� �ɷ� ���
    public List<RequiredAbility> RequiredAbilities;

    [Header("�ռ� ��� �ɷ�")]
    // �ռ����� ������ �ɷ��� ������
    public GameObject CombinedAbilityPrefab;
}