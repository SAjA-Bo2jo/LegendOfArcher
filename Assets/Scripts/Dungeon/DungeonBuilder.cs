using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public partial class DungeonBuilder : MonoBehaviour
{
    
    // ���� ���
    [SerializeField] private GameObject dungeonRoot;
    private float dungeonPosX = 0;
    private float dungeonPosY = 0;
    Vector3 dungeonPos = Vector3.zero;

    // ����Ʈ 
    [SerializeField] private GameObject entryGate;  // �Ա�
    private float entryGatePosX = -6.5f;
    private float entryGatePosY = 4f;
    Vector3 entryGatePos = Vector3.zero;
    [SerializeField] private GameObject exitGate;   // �ⱸ
    private float exitGatePosX = 6.5f;
    private float exitGatePosY = 4f;
    Vector3 exitGatePos = Vector3.zero;

    // �÷��̾� - ���� ��ġ ����
    [SerializeField] private GameObject playerPrefab;
    private float playerPosX = 0;
    private float playerPosY = 0;
    Vector3 playerPos = Vector3.zero;

    // ��
    [SerializeField] private int enemyCount;
    private float enemyPosX = 0;
    private float enemyPosY = 0;
    Vector2 enemyPos = Vector2.zero;

    public DungeonObjects Build()
    {
        DungeonObjects result = new DungeonObjects();
        
        // ���� ����
        dungeonPos = new Vector3(dungeonPosX, dungeonPosY);
        result.dungeonRoot = Instantiate(dungeonRoot, dungeonPos, Quaternion.identity);
        
        // ����Ʈ ���� - �Ա�
        entryGatePos = new Vector3(entryGatePosX, entryGatePosY);
        GameObject entry = Instantiate(entryGate, entryGatePos, Quaternion.identity);
        GateController entryCtrl = entry.GetComponent<GateController>();
        entryCtrl.SetGateType(GateType.Entry);
        result.entryGate = entryCtrl;

        // ����Ʈ ���� - �ⱸ
        exitGatePos = new Vector3(exitGatePosX, exitGatePosY);
        GameObject exit = Instantiate(exitGate, exitGatePos, Quaternion.identity);
        GateController exitCtrl = exit.GetComponent<GateController>();
        exitCtrl.SetGateType(GateType.Exit);
        result.exitGate = exitCtrl;
        StageManager.Instance.ExitGate = result.exitGate;

        // build obstacles
        result.obstacles = SpawnObstacles();

        // �� ����
        enemyCount = StageManager.Instance.CurrentStageData.enemyCount;
        result.enemies = new List<GameObject>();
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemy = ObjectPoolManager.Instance.Get("enemy");            // �� Ű���� enum���� �����ǵ��� ����
            enemy.transform.position = new Vector2(enemyPosX, enemyPosY);
            result.enemies.Add(enemy);
            StageManager.Instance.AddMonsterToList(enemy);
        }

        // �÷��̾� ����
        playerPos = new Vector3(playerPosX, playerPosY);
        result.player = Instantiate(playerPrefab, playerPos, Quaternion.identity);
        StageManager.Instance._Player = result.player;
        
        return result;
    }
}
