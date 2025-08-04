using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public partial class DungeonBuilder : MonoBehaviour
{
    // 던전 기반
    [SerializeField] private GameObject dungeonRoot;
    private float dungeonPosX = 0;
    private float dungeonPosY = 0;
    Vector3 dungeonPos = Vector3.zero;

    // 게이트 
    [SerializeField] private GameObject entryGate;  // 입구
    private float entryGatePosX = -6.5f;
    private float entryGatePosY = 4f;
    Vector3 entryGatePos = Vector3.zero;
    [SerializeField] private GameObject exitGate;   // 출구
    private float exitGatePosX = 6.5f;
    private float exitGatePosY = 4f;
    Vector3 exitGatePos = Vector3.zero;

    // 플레이어 - 추후 위치 수정
    [SerializeField] private GameObject playerPrefab;
    private float playerPosX = -6.5f;
    private float playerPosY = 3f;
    private Vector3 playerPos;
    
    // 적 
    [SerializeField] private int baseGoblinCount;
    [SerializeField] private int archerGoblinCount;
    [SerializeField] private int bossGoblinCount;
    private float enemyPosX = 0;
    private float enemyPosY = 0;
    Vector2 enemyPos = Vector2.zero; 

    public DungeonObjects result;

    public DungeonObjects Build()
    {
        result = new DungeonObjects();
        
        // 던전 생성
        MakeDungeonMaps();
        
        // 게이트 생성 - 입구, 게이트 생성 - 출구
        MakeEntryAndExitGates();

        // build obstacles
        result.obstacles = SpawnObstacles();
        
        // 적 생성
        SpawnMonsters();

        // 플레이어 생성
        playerPos = new Vector3(playerPosX, playerPosY, 0);
        result.player = Instantiate(playerPrefab, playerPos, Quaternion.identity);
        StageManager.Instance._Player = result.player;
        
        return result;
    }

    private void MakeDungeonMaps()
    {
        for (int i = 0; i < 5; i++)
        {
            dungeonPos = new Vector3(dungeonPosX + (i * 20), dungeonPosY);
            result.dungeonRoot = Instantiate(dungeonRoot, dungeonPos, Quaternion.identity);
            // 던전을 빈 오브젝트의 자식으로 만드는 메소드
            result.dungeonRoot.transform.SetParent(StageManager.Instance.dungeonParent.transform);
        }
    }

    private void MakeEntryAndExitGates()
    {
        for (int i = 0; i < 5; i++)
        {
            // 게이트 생성 - 입구
            entryGatePos = new Vector3(entryGatePosX + (i * 20), entryGatePosY);
            GameObject entry = Instantiate(entryGate, entryGatePos, Quaternion.identity);
            GateController entryCtrl = entry.GetComponent<GateController>();
            entryCtrl.SetGateType(GateType.Entry);
            // 게이트를 하나의 빈 오브젝트의 자식으로 만드는 메소드
            entryCtrl.transform.SetParent(StageManager.Instance.gateParent.transform);
            result.entryGate = entryCtrl;
            StageManager.Instance.EntryGateList.Add(result.entryGate);
            
            // 게이트 생성 - 출구
            exitGatePos = new Vector3(exitGatePosX + (i * 20), exitGatePosY);
            GameObject exit = Instantiate(exitGate, exitGatePos, Quaternion.identity);
            GateController exitCtrl = exit.GetComponent<GateController>();
            exitCtrl.SetGateType(GateType.Exit);
            // 게이트를 하나의 빈 오브젝트의 자식으로 만드는 메소드
            exitCtrl.transform.SetParent(StageManager.Instance.gateParent.transform);
            result.exitGate = exitCtrl;
            StageManager.Instance.ExitGateList.Add(result.exitGate);
        }
    }

    public void SpawnMonsters()
    {
        baseGoblinCount = StageManager.Instance.CurrentStageData.baseGoblinCount;
        archerGoblinCount = StageManager.Instance.CurrentStageData.archerGoblincount;
        bossGoblinCount = StageManager.Instance.CurrentStageData.bossGoblinCount;
        
        // result.enemies = SpawnEnemies();
        result.enemies = new List<GameObject>();
        
        // 기본 고블린 생성
        for (int i = 0; i < baseGoblinCount; i++)
        {
            GameObject enemy = ObjectPoolManager.Instance.Get("enemy");            // 적 키값은 enum으로 관리되도록 수정
            enemy.transform.SetParent(StageManager.Instance.enemyParent.transform);
            enemy.transform.position = 
                new Vector2(enemyPosX + 20 * ((StageManager.Instance.StageLevel - 1) % 5), enemyPosY);
            result.enemies.Add(enemy);
            StageManager.Instance.AddMonsterToList(enemy);
        }
        
        // 궁수 고블린 생성
        for (int i = 0; i < archerGoblinCount; i++)
        {
            GameObject enemy = ObjectPoolManager.Instance.Get("Archer");
            enemy.transform.SetParent(StageManager.Instance.enemyParent.transform);
            enemy.transform.position = 
                new Vector2(enemyPosX + 20 * ((StageManager.Instance.StageLevel - 1) % 5), enemyPosY);
            result.enemies.Add(enemy);
            StageManager.Instance.AddMonsterToList(enemy);
        }
        
        // 보스 고블린 생성
        for (int i = 0; i < bossGoblinCount; i++)
        {
            GameObject enemy = ObjectPoolManager.Instance.Get("Boss");
            enemy.transform.SetParent(StageManager.Instance.enemyParent.transform);
            enemy.transform.position = 
                new Vector2(enemyPosX + 20 * ((StageManager.Instance.StageLevel - 1) % 5), enemyPosY);
            result.enemies.Add(enemy);
            StageManager.Instance.AddMonsterToList(enemy);
        }
    }
}