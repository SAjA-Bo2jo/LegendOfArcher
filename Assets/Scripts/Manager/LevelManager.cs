using UnityEngine;
// System.Collections.Generic은 필요하면 추가

public class LevelManager : MonoBehaviour
{
    [Header("경험치/레벨 관련 스탯")]
    [SerializeField] private int level = 1;
    public int Level => level;

    [SerializeField] private float experience = 0f;
    public float Experience => experience;

    // 레벨업에 필요한 경험치 배열 (코드에서 자동 생성)
    [SerializeField] private float[] expToNextLevel;

    [Tooltip("경험치 테이블 계산에 사용될 기본 경험치량 (레벨1->2).")]
    [SerializeField] private float baseExperienceForLevelUp = 100f;
    [Tooltip("경험치 테이블 계산에 사용될 지수 계수.")]
    [SerializeField] private float expCoefficient = 1.2f; // 사용자 요청 계수 1.2
    [Tooltip("최대 도달 가능한 레벨.")]
    [SerializeField] private const int MAX_PLAYER_LEVEL = 40; // 최대 레벨 40

    // LevelManager가 Player와 AbilitySelectionManager에 접근하기 위한 참조
    private Player player;
    private AbilitySelectionManager abilitySelectionManager;

    void Awake()
    {
        // 씬에서 Player와 AbilitySelectionManager 참조를 찾습니다.
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError("LevelManager: 씬에서 Player 오브젝트를 찾을 수 없습니다!");
        }

        abilitySelectionManager = FindObjectOfType<AbilitySelectionManager>();
        if (abilitySelectionManager == null)
        {
            Debug.LogWarning("LevelManager: 씬에서 AbilitySelectionManager 오브젝트를 찾을 수 없습니다.");
        }

        // 경험치 테이블 생성
        GenerateExpTable();
    }

    /// <summary>
    /// 지수함수를 적용하여 레벨업에 필요한 경험치 테이블을 생성합니다.
    /// </summary>
    private void GenerateExpTable()
    {
        // MAX_PLAYER_LEVEL까지의 필요 경험치를 저장할 배열 (인덱스 0부터 시작, level-1 사용)
        // expToNextLevel[0]은 Level 1 -> Level 2 필요 경험치
        // expToNextLevel[MAX_PLAYER_LEVEL-1]은 MAX_PLAYER_LEVEL-1 -> MAX_PLAYER_LEVEL 필요 경험치
        expToNextLevel = new float[MAX_PLAYER_LEVEL];

        for (int i = 0; i < MAX_PLAYER_LEVEL; i++)
        {
            // Level (i+1)에서 Level (i+2)로 가는 경험치
            expToNextLevel[i] = baseExperienceForLevelUp * Mathf.Pow(expCoefficient, i);
            if (i < MAX_PLAYER_LEVEL - 1) // 마지막 레벨업 구간만 제외하고 로그 출력
            {
                Debug.Log($"Lv.{i + 1} -> Lv.{i + 2} 필요 경험치: {expToNextLevel[i]:F2}");
            }
        }
        Debug.Log($"경험치 테이블 생성 완료 (총 {expToNextLevel.Length} 레벨 구간).");
    }


    /// <summary>
    /// 플레이어에게 경험치를 추가합니다.
    /// </summary>
    /// <param name="expAmount">추가할 경험치 양.</param>
    public void AddExperience(float expAmount)
    {
        if (level >= MAX_PLAYER_LEVEL) // 최대 레벨에 도달했는지 먼저 확인
        {
            Debug.Log("최대 레벨에 도달했습니다. 더 이상 경험치를 획득할 수 없습니다.");
            experience = 0; // 최대 레벨에서는 경험치를 더 이상 쌓지 않음
            return;
        }

        experience += expAmount;

        // 현재 레벨업에 필요한 경험치량.
        // 배열 인덱스는 레벨보다 1 작으므로 (level - 1)
        float requiredExp = expToNextLevel[level - 1];
        Debug.Log($"경험치 획득: {expAmount}. 현재 경험치: {experience:F2} / {requiredExp:F2}");

        // 현재 레벨이 최대 레벨 미만이고, 다음 레벨업에 필요한 경험치를 충족했는지 확인
        // 레벨업이 가능하다면 LevelUp 메서드를 호출합니다.
        while (level < MAX_PLAYER_LEVEL && experience >= requiredExp)
        {
            LevelUp(); // 조건 충족 시 LevelUp 메서드를 호출합니다.

            // LevelUp에서 레벨이 증가했으므로, 다음 레벨업 필요 경험치를 다시 계산
            if (level < MAX_PLAYER_LEVEL) // 최대 레벨에 도달했는지 다시 확인
            {
                requiredExp = expToNextLevel[level - 1];
            }
            else // 최대 레벨에 도달하면 루프 종료
            {
                break;
            }
        }
    }

    /// <summary>
    /// 플레이어를 레벨업 시키고 관련 스탯을 증가시킵니다.
    /// 이 메서드는 AddExperience에서만 호출됩니다.
    /// </summary>
    private void LevelUp()
    {
        // 현재 레벨업에 필요한 경험치량을 가져옵니다.
        float requiredExpForCurrentLevel = expToNextLevel[level - 1];

        // 초과 경험치를 계산하고 다음 레벨로 이월합니다.
        experience -= requiredExpForCurrentLevel;

        level++; // 레벨 증가

        Debug.Log($"레벨업! 현재 레벨: {level}. 이월된 경험치: {experience:F2}");

        // 플레이어 스탯 재계산은 이제 어빌리티 획득/제거 시에만 일어납니다.
        // 레벨업 자체로는 스탯을 올리지 않습니다.
        // if (player != null)
        // {
        //     player.RecalculateStats(); // 이 줄은 제거합니다.
        // }

        // 레벨업 시 어빌리티 선택창 표시 (AbilitySelectionManager를 호출)
        if (abilitySelectionManager != null)
        {
            abilitySelectionManager.ShowAbilitySelection();
        }
        else
        {
            Debug.LogWarning("LevelManager: AbilitySelectionManager를 씬에서 찾을 수 없습니다. 어빌리티 선택창을 표시할 수 없습니다.");
        }

        // 최대 레벨에 도달하면 경험치를 0으로 설정합니다.
        if (level >= MAX_PLAYER_LEVEL)
        {
            experience = 0;
            Debug.Log("최대 레벨에 도달하여 더 이상 레벨업할 수 없습니다.");
        }
    }
}