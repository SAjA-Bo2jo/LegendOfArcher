using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public partial class DungeonBuilder : MonoBehaviour
{
    // ���� ���
    [SerializeField] private GameObject dungeonRoot;
    [SerializeField] private GameObject dungeonRootBoss;
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

    // �÷��̾�
    [SerializeField] private GameObject playerPrefab;
    private float playerPosX = -6.5f;
    private float playerPosY = 3f;
    private Vector3 playerPos;

    // �� 
    [SerializeField] private int baseGoblinCount;
    [SerializeField] private int archerGoblinCount;
    [SerializeField] private int bossGoblinCount;
    Vector2 enemyPos = Vector2.zero;

    public DungeonObjects result;

    public DungeonObjects Build()
    {
        Debug.Log("�� [DungeonBuilder] Build called");

        result = new DungeonObjects();

        allOccupiedPositions = new List<Vector3>();

        // ���� ����
        MakeDungeonMaps();

        // ����Ʈ ���� - �Ա�, ����Ʈ ���� - �ⱸ
        MakeEntryAndExitGates();

        // build obstacles
        SpawnObstacles();

        // �� ����
        SpawnMonsters();

        // �÷��̾� ����
        playerPos = new Vector3(playerPosX, playerPosY, 0);
        result.player = Instantiate(playerPrefab, playerPos, Quaternion.identity);
        StageManager.Instance._Player = result.player;

        return result;
    }

    private void MakeDungeonMaps()
    {
        Debug.Log("�� [DungeonBuilder] MakeDungeonMaps called");

        for (int i = 0; i < 5; i++)
        {
            dungeonPos = new Vector3(dungeonPosX + (i * 20), dungeonPosY);

            // spawn boss map every 5 stage
            if (i == 4)
            {
                result.dungeonRoot = Instantiate(dungeonRootBoss, dungeonPos, Quaternion.identity);
                result.obstacles = SpawnFixedObstacles(i);  // spawn fixed obstacles in boss stage
            }
            else
            {
                result.dungeonRoot = Instantiate(dungeonRoot, dungeonPos, Quaternion.identity);
            }

            //������ �� ������Ʈ�� �ڽ����� ����� �޼ҵ�
            result.dungeonRoot.transform.SetParent(StageManager.Instance.dungeonParent.transform);
        }
    }

    private void MakeEntryAndExitGates()
    {
        Debug.Log("�� [DungeonBuilder] MakeEntryAndExitGates called");

        for (int i = 0; i < 5; i++)
        {
            // ����Ʈ ���� - �Ա�
            entryGatePos = new Vector3(entryGatePosX + (i * 20), entryGatePosY);
            GameObject entry = Instantiate(entryGate, entryGatePos, Quaternion.identity);
            GateController entryCtrl = entry.GetComponent<GateController>();
            entryCtrl.SetGateType(GateType.Entry);

            // ����Ʈ�� �ϳ��� �� ������Ʈ�� �ڽ����� ����� �޼ҵ�
            entryCtrl.transform.SetParent(StageManager.Instance.gateParent.transform);
            result.entryGate = entryCtrl;
            StageManager.Instance.EntryGateList.Add(result.entryGate);

            // ����Ʈ ���� - �ⱸ
            exitGatePos = new Vector3(exitGatePosX + (i * 20), exitGatePosY);
            GameObject exit = Instantiate(exitGate, exitGatePos, Quaternion.identity);
            GateController exitCtrl = exit.GetComponent<GateController>();
            exitCtrl.SetGateType(GateType.Exit);
            
            // ����Ʈ�� �ϳ��� �� ������Ʈ�� �ڽ����� ����� �޼ҵ�
            exitCtrl.transform.SetParent(StageManager.Instance.gateParent.transform);
            result.exitGate = exitCtrl;
            StageManager.Instance.ExitGateList.Add(result.exitGate);
        }
    }

    public void SpawnMonsters()
    {
        Debug.Log("�� [DungeonBuilder] SpawnMonsters called");

        baseGoblinCount = StageManager.Instance.CurrentStageData.baseGoblinCount;
        archerGoblinCount = StageManager.Instance.CurrentStageData.archerGoblincount;
        bossGoblinCount = StageManager.Instance.CurrentStageData.bossGoblinCount;

        // result.enemies = SpawnEnemies();
        result.enemies = new List<GameObject>();

        // �⺻ ��� ����
        SpawnSingleTypeOfEnemies(baseGoblinCount, "enemy");

        // �ü� ��� ����
        SpawnSingleTypeOfEnemies(archerGoblinCount, "Archer");

        // ���� ��� ����
        SpawnSingleTypeOfEnemies(bossGoblinCount, "Boss");
    }

    private void SpawnSingleTypeOfEnemies(int count, string key)
    {
        Debug.Log("�� [DungeonBuilder] SpawnSingleTypeOfEnemies called");

        List<Vector3> positions = new();
        positions = SetObjectsPosition(count);

        for (int i = 0; i < count; i++)
        {
            GameObject enemy = ObjectPoolManager.Instance.Get(key);

            if (enemy == null)
                Debug.LogWarning($"There is no {key} enemy in objects pool.");
                        
            enemy.transform.SetParent(StageManager.Instance.enemyParent.transform);
            
            enemy.transform.position = positions[i];
            Debug.Log($"Enemy {key} spawn location{i}: {positions[i]}");

            result.enemies.Add(enemy);
            StageManager.Instance.AddMonsterToList(enemy);
            Debug.Log($" Enemy {key} added in stage monster list");
        }
    }

}