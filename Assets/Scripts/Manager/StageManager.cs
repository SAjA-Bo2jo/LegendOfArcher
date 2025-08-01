using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoSingleton<StageManager>
{
    // 스테이지 레벨을 저장하고 스테이지를 클리어할 때 마다 레벨이 증가
    // 스테이지 레벨이 올라갈 수록 난이도를 어렵게 설정할 예정 -> 레벨에 따라 추가 체력, 추가 공격력, 추가 화살 등등?
    // 스테이지 레벨이 5의 배수일 때마다 보스 몬스터 스폰
    private int stageLevel = 1;
    private bool isClear = false; // 플레이어가 몬스터를 다 잡아서 스테이지를 클리어 했는지 판별

    private StageDatabase _stageDatabase;
    private StageData _currentStageData;
    public StageData CurrentStageData
    {
        get { return _currentStageData; }
    }
    
    [SerializeField] private DungeonBuilder dungeonBuilder;
    private DungeonObjects _dungeon;
    private GateController exitGate;

    public GateController ExitGate
    {
        get { return exitGate; }
        set { exitGate = value; }
    }
    
    // 던전에 스폰된 몬스터 정보를 담아둘 몬스터 리스트
    [SerializeField] private List<GameObject> monsterList = new List<GameObject>();
    [SerializeField] public GameObject _Player;

    private void Start()
    {
        _stageDatabase = StageDatabase.Instance;

        LoadCurrentStageData();
        
        if (dungeonBuilder == null)
        {
            Debug.LogError("DungeonBuilder가 연결되지 않았습니다!");
            return;
        }

        _dungeon = dungeonBuilder.Build();

        // 게이트 충돌 이벤트 연결 - 게이트 충돌 시 메서드와 스테이지 이동 메서드를 통일하는 코드
        if (_dungeon.exitGate == null)
            Debug.LogError("exitGate 생성에 문제가 생겼습니다!");
        
        _dungeon.exitGate.OnPlayerEnterExitGate = StageClear;
    }

    private void Update()
    {
        if (monsterList.Count <= 0)
        {
            StageClear();
        }
    }

    private void LoadCurrentStageData()
    {
        _currentStageData = _stageDatabase.GetStageData(stageLevel);
    }

    public void AddMonsterToList(GameObject monster)
    {
        monsterList.Add(monster);
    }

    public void RemoveMonsterFromList(GameObject monster)
    {
        monsterList.Remove(monster);
    }
    
    // 스테이지 안의 몬스터가 0 -> 스테이지 클리어, 다음 스테이지로 가는 게이트가 열림
    // 나중에 Update 안에서 사용할 예정
    public void StageClear()
    {
        Debug.Log("스테이지 클리어!");
        // isClear를 true로 바꿈
        isClear = true;
        // 던전의 게이트 오픈
        exitGate.OpenExitGate();
        // 스테이지 레벨 +1
        // 다음 스테이지의 정보를 _currentStageData로 옮겨야 함
    }

    public void PlayerDie()
    {
        // 현재 플레이어가 가지고 있는 정보들을 결과창에 옮긴다.
        // 결과창 호출
    }
}