using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스테이지 데이터는 추후에 편집할 예정
[System.Serializable]
public struct StageData
{
    public int stageLevel;
    public int baseGoblinCount;
    public int archerGoblincount;
    public bool isBossStage;
    public int bossGoblinCount;
    public int obstacleCount;

    public StageData(int stageLevel, int baseGoblinCount, int archerGoblinCount, bool isBossStage, int bossGoblinCount, int obstacleCount)
    {
        this.stageLevel = stageLevel;
        this.baseGoblinCount = baseGoblinCount;
        this.archerGoblincount = archerGoblinCount;
        this.isBossStage = isBossStage;
        this.bossGoblinCount = bossGoblinCount;
        this.obstacleCount = obstacleCount;
    }
}

public class StageDatabase : MonoBehaviour
{
    public static StageDatabase Instance { get; private set; }

    private List<StageData> _stageList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        InitDatabase();
    }

    // stageDatabase 초기화 메소드. 스테이지를 늘리고 싶으면 여기에 추가하면 될듯
    public void InitDatabase()
    {
        _stageList = new List<StageData>
        {
            new StageData(1, 1,0, false, 0, 0),
            new StageData(2, 1, 1, false, 0, 1),
            new StageData(3, 2, 1, false, 0, 2),
            new StageData(4, 2, 2, false, 0, 3),
            new StageData(5, 0, 0, true, 1, 0),
            
            new StageData(6, 0, 1, false, 0, 1),
            new StageData(7, 2, 1, false, 0, 2),
            new StageData(8, 3, 1, false, 0, 3),
            new StageData(9, 3, 2, false, 0, 1),
            new StageData(10, 0, 0, true, 2, 0),
        };
        
        Debug.Log("stageList에 " + _stageList.Count + "개의 스테이지 데이터가 초기화되었습니다.");
    }

    // stageManager의 현재 레벨과 비교하여 현재 레벨의 스테이지 정보를 가져오는 메소드
    public StageData GetStageData(int stageLevel)
    {
        return _stageList[stageLevel - 1];
    }
}
