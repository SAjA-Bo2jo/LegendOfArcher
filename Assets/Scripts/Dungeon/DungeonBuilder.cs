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
    private float playerPosX = 0;
    private float playerPosY = 0;
    Vector3 playerPos = Vector3.zero;

    // 적
    [SerializeField] private int enemyCount;
    private float enemyPosX = 0;
    private float enemyPosY = 0;
    Vector2 enemyPos = Vector2.zero;

    public DungeonObjects Build()
    {
        DungeonObjects result = new DungeonObjects();
        
        // 던전 생성
        dungeonPos = new Vector3(dungeonPosX, dungeonPosY);
        result.dungeonRoot = Instantiate(dungeonRoot, dungeonPos, Quaternion.identity);
        
        // 게이트 생성 - 입구
        entryGatePos = new Vector3(entryGatePosX, entryGatePosY);
        GameObject entry = Instantiate(entryGate, entryGatePos, Quaternion.identity);
        GateController entryCtrl = entry.GetComponent<GateController>();
        entryCtrl.SetGateType(GateType.Entry);
        result.entryGate = entryCtrl;

        // 게이트 생성 - 출구
        exitGatePos = new Vector3(exitGatePosX, exitGatePosY);
        GameObject exit = Instantiate(exitGate, exitGatePos, Quaternion.identity);
        GateController exitCtrl = exit.GetComponent<GateController>();
        exitCtrl.SetGateType(GateType.Exit);
        result.exitGate = exitCtrl;
        StageManager.Instance.ExitGate = result.exitGate;

        // build obstacles
        result.obstacles = SpawnObstacles();

        // 적 생성
        enemyCount = StageManager.Instance.CurrentStageData.enemyCount;
        result.enemies = new List<GameObject>();
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemy = ObjectPoolManager.Instance.Get("enemy");            // 적 키값은 enum으로 관리되도록 수정
            enemy.transform.position = new Vector2(enemyPosX, enemyPosY);
            result.enemies.Add(enemy);
            StageManager.Instance.AddMonsterToList(enemy);
        }

        // 플레이어 생성
        playerPos = new Vector3(playerPosX, playerPosY);
        result.player = Instantiate(playerPrefab, playerPos, Quaternion.identity);
        StageManager.Instance._Player = result.player;
        
        return result;
    }
}
