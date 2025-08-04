using UnityEngine;
// System.Collections.Generic�� �ʿ��ϸ� �߰�

public class LevelManager : MonoBehaviour
{
    [Header("����ġ/���� ���� ����")]
    [SerializeField] private int level = 1;
    public int Level => level;

    [SerializeField] private float experience = 0f;
    public float Experience => experience;

    // �������� �ʿ��� ����ġ �迭 (�ڵ忡�� �ڵ� ����)
    [SerializeField] private float[] expToNextLevel;

    [Tooltip("����ġ ���̺� ��꿡 ���� �⺻ ����ġ�� (����1->2).")]
    [SerializeField] private float baseExperienceForLevelUp = 100f;
    [Tooltip("����ġ ���̺� ��꿡 ���� ���� ���.")]
    [SerializeField] private float expCoefficient = 1.2f; // ����� ��û ��� 1.2
    [Tooltip("�ִ� ���� ������ ����.")]
    [SerializeField] private const int MAX_PLAYER_LEVEL = 40; // �ִ� ���� 40

    // LevelManager�� Player�� AbilitySelectionManager�� �����ϱ� ���� ����
    private Player player;
    private AbilitySelectionManager abilitySelectionManager;

    void Awake()
    {
        // ������ Player�� AbilitySelectionManager ������ ã���ϴ�.
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError("LevelManager: ������ Player ������Ʈ�� ã�� �� �����ϴ�!");
        }

        abilitySelectionManager = FindObjectOfType<AbilitySelectionManager>();
        if (abilitySelectionManager == null)
        {
            Debug.LogWarning("LevelManager: ������ AbilitySelectionManager ������Ʈ�� ã�� �� �����ϴ�.");
        }

        // ����ġ ���̺� ����
        GenerateExpTable();
    }

    /// <summary>
    /// �����Լ��� �����Ͽ� �������� �ʿ��� ����ġ ���̺��� �����մϴ�.
    /// </summary>
    private void GenerateExpTable()
    {
        // MAX_PLAYER_LEVEL������ �ʿ� ����ġ�� ������ �迭 (�ε��� 0���� ����, level-1 ���)
        // expToNextLevel[0]�� Level 1 -> Level 2 �ʿ� ����ġ
        // expToNextLevel[MAX_PLAYER_LEVEL-1]�� MAX_PLAYER_LEVEL-1 -> MAX_PLAYER_LEVEL �ʿ� ����ġ
        expToNextLevel = new float[MAX_PLAYER_LEVEL];

        for (int i = 0; i < MAX_PLAYER_LEVEL; i++)
        {
            // Level (i+1)���� Level (i+2)�� ���� ����ġ
            expToNextLevel[i] = baseExperienceForLevelUp * Mathf.Pow(expCoefficient, i);
            if (i < MAX_PLAYER_LEVEL - 1) // ������ ������ ������ �����ϰ� �α� ���
            {
                Debug.Log($"Lv.{i + 1} -> Lv.{i + 2} �ʿ� ����ġ: {expToNextLevel[i]:F2}");
            }
        }
        Debug.Log($"����ġ ���̺� ���� �Ϸ� (�� {expToNextLevel.Length} ���� ����).");
    }


    /// <summary>
    /// �÷��̾�� ����ġ�� �߰��մϴ�.
    /// </summary>
    /// <param name="expAmount">�߰��� ����ġ ��.</param>
    public void AddExperience(float expAmount)
    {
        if (level >= MAX_PLAYER_LEVEL) // �ִ� ������ �����ߴ��� ���� Ȯ��
        {
            Debug.Log("�ִ� ������ �����߽��ϴ�. �� �̻� ����ġ�� ȹ���� �� �����ϴ�.");
            experience = 0; // �ִ� ���������� ����ġ�� �� �̻� ���� ����
            return;
        }

        experience += expAmount;

        // ���� �������� �ʿ��� ����ġ��.
        // �迭 �ε����� �������� 1 �����Ƿ� (level - 1)
        float requiredExp = expToNextLevel[level - 1];
        Debug.Log($"����ġ ȹ��: {expAmount}. ���� ����ġ: {experience:F2} / {requiredExp:F2}");

        // ���� ������ �ִ� ���� �̸��̰�, ���� �������� �ʿ��� ����ġ�� �����ߴ��� Ȯ��
        // �������� �����ϴٸ� LevelUp �޼��带 ȣ���մϴ�.
        while (level < MAX_PLAYER_LEVEL && experience >= requiredExp)
        {
            LevelUp(); // ���� ���� �� LevelUp �޼��带 ȣ���մϴ�.

            // LevelUp���� ������ ���������Ƿ�, ���� ������ �ʿ� ����ġ�� �ٽ� ���
            if (level < MAX_PLAYER_LEVEL) // �ִ� ������ �����ߴ��� �ٽ� Ȯ��
            {
                requiredExp = expToNextLevel[level - 1];
            }
            else // �ִ� ������ �����ϸ� ���� ����
            {
                break;
            }
        }
    }

    /// <summary>
    /// �÷��̾ ������ ��Ű�� ���� ������ ������ŵ�ϴ�.
    /// �� �޼���� AddExperience������ ȣ��˴ϴ�.
    /// </summary>
    private void LevelUp()
    {
        // ���� �������� �ʿ��� ����ġ���� �����ɴϴ�.
        float requiredExpForCurrentLevel = expToNextLevel[level - 1];

        // �ʰ� ����ġ�� ����ϰ� ���� ������ �̿��մϴ�.
        experience -= requiredExpForCurrentLevel;

        level++; // ���� ����

        Debug.Log($"������! ���� ����: {level}. �̿��� ����ġ: {experience:F2}");

        // �÷��̾� ���� ������ ���� �����Ƽ ȹ��/���� �ÿ��� �Ͼ�ϴ�.
        // ������ ��ü�δ� ������ �ø��� �ʽ��ϴ�.
        // if (player != null)
        // {
        //     player.RecalculateStats(); // �� ���� �����մϴ�.
        // }

        // ������ �� �����Ƽ ����â ǥ�� (AbilitySelectionManager�� ȣ��)
        if (abilitySelectionManager != null)
        {
            abilitySelectionManager.ShowAbilitySelection();
        }
        else
        {
            Debug.LogWarning("LevelManager: AbilitySelectionManager�� ������ ã�� �� �����ϴ�. �����Ƽ ����â�� ǥ���� �� �����ϴ�.");
        }

        // �ִ� ������ �����ϸ� ����ġ�� 0���� �����մϴ�.
        if (level >= MAX_PLAYER_LEVEL)
        {
            experience = 0;
            Debug.Log("�ִ� ������ �����Ͽ� �� �̻� �������� �� �����ϴ�.");
        }
    }
}