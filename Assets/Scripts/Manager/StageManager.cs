using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoSingleton<StageManager>
{
    // 스테이지 레벨을 저장하고 스테이지를 클리어할 때 마다 레벨이 증가
    // 스테이지 레벨이 올라갈 수록 난이도를 어렵게 설정할 예정 -> 레벨에 따라 추가 체력, 추가 공격력, 추가 화살 등등?
    // 스테이지 레벨이 5의 배수일 때마다 보스 몬스터 스폰
    [SerializeField] private int stageLevel = 1;
    public int StageLevel
    {
        get { return stageLevel; }
    }
    
    private bool isClear = false; // 플레이어가 몬스터를 다 잡아서 스테이지를 클리어 했는지 판별
    private bool isClearProcessed = false; // 스테이지 클리어 후의 처리를 하였는지 판별

    private StageDatabase _stageDatabase;
    private StageData _currentStageData;
    public StageData CurrentStageData
    {
        get { return _currentStageData; }
    }
    
    [SerializeField] private DungeonBuilder dungeonBuilder;
    private DungeonObjects _dungeon;

    // 출구 게이트의 정보를 담아둘 리스트
    [SerializeField]private List<GateController> exitGateList = new List<GateController>();
    public List<GateController> ExitGateList
    {
        get { return exitGateList; }
    }
    
    // 입구 게이트의 정보를 담아둘 리스트
    [SerializeField] private List<GateController> entryGateList = new List<GateController>();
    public List<GateController> EntryGateList
    {
        get { return entryGateList; }
    }
    
    // 던전에 스폰된 몬스터 정보를 담아둘 몬스터 리스트
    [SerializeField] private List<GameObject> monsterList = new List<GameObject>();
    [SerializeField] public GameObject _Player;
    
    public GameObject dungeonParent;
    public GameObject gateParent;
    public GameObject enemyParent;

    [SerializeField] private MainGameUIManager mainGameUIManager;

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
        
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (monsterList.Count <= 0 && !isClearProcessed)
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
        GameManager.Instance.AddKillMonsterCount();
    }
    
    // 스테이지 안의 몬스터가 0 -> 스테이지 클리어, 다음 스테이지로 가는 게이트가 열림
    // 나중에 Update 안에서 사용할 예정
    private void StageClear()
    {
        Debug.Log("스테이지 클리어!");
        
        // isClear를 true로 바꿈
        isClear = true;
        isClearProcessed = true;
        
        // 던전의 게이트 오픈
        exitGateList[(stageLevel - 1) % 5].OpenExitGate();
    }

    public void ToNextStage()
    {
        // 스테이지 데이터가 적은 관계로 임시로 만든 스테이지 클리어
        if (stageLevel == 10)
        {
            ToWinEndGameScene();
            return;
        }
        
        Debug.Log("다음 스테이지로 이동합니다");
        _Player.transform.position = new Vector3((-6.5f + (stageLevel % 5) * 20f), _Player.transform.position.y,
            _Player.transform.position.z);
        Camera.main.transform.position = new Vector3(((stageLevel % 5) * 20f), Camera.main.transform.position.y,
            Camera.main.transform.position.z);
        
        // 플레이어가 다음 스테이지로 넘어가면 이전 스테이지의 출구 게이트를 닫는 애니메이션 출력
        exitGateList[(stageLevel - 1) % 5].CloseExitGate();
        
        // 다음 스테이지의 입구 게이트 닫는 애니메이션 출력
        entryGateList[(stageLevel % 5)]._animator.Play("IdleOpen");
        entryGateList[(stageLevel % 5)].CloseEntryGate();
        
        // 스테이지 레벨 +1
        stageLevel++;
        
        if (StageLevel % 5 == 0)
        {
            SoundManager.Instance.PlayBackgroundMusic(SoundManager.Instance.BossMusic);
        }
        else
        {
            SoundManager.Instance.PlayBackgroundMusic(SoundManager.Instance.MainGameMusic);
        }
        
        mainGameUIManager.CalculatePlayerIconPos(stageLevel);
        
        // 다음 스테이지의 정보를 _currentStageData로 옮겨야 함
        LoadCurrentStageData();
        
        // 다음 스테이지의 장애물들을 스폰
        dungeonBuilder.SpawnObstacles();
        
        // 다음 스테이지의 몬스터를 스폰
        dungeonBuilder.SpawnMonsters();
        
        isClear = false;
        isClearProcessed = false;
    }

    private void ToWinEndGameScene()
    {
        GameManager.Instance.FinalFloor = stageLevel;
        GameManager.Instance.LoadResultWinScene();
    }
    
    public void PlayerDie()
    {
        // 현재 플레이어가 가지고 있는 정보들을 결과창에 옮긴다.
        // 결과창 호출
        Invoke("LoadEndGameScene", 3f);
    }
    
    private void LoadEndGameScene()
    {
        GameManager.Instance.FinalFloor = stageLevel;
        GameManager.Instance.LoadResultDeadScene();
    }
}