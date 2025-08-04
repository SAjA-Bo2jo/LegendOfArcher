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
    private float playerPosX = -6.5f;
    private float playerPosY = 3f;
    private Vector3 playerPos;
    
    // �� 
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
        
        // ���� ����
        MakeDungeonMaps();
        
        // ����Ʈ ���� - �Ա�, ����Ʈ ���� - �ⱸ
        MakeEntryAndExitGates();

        // build obstacles
        result.obstacles = SpawnObstacles();
        
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
        for (int i = 0; i < 5; i++)
        {
            dungeonPos = new Vector3(dungeonPosX + (i * 20), dungeonPosY);
            result.dungeonRoot = Instantiate(dungeonRoot, dungeonPos, Quaternion.identity);
            // ������ �� ������Ʈ�� �ڽ����� ����� �޼ҵ�
            result.dungeonRoot.transform.SetParent(StageManager.Instance.dungeonParent.transform);
        }
    }

    private void MakeEntryAndExitGates()
    {
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
        baseGoblinCount = StageManager.Instance.CurrentStageData.baseGoblinCount;
        archerGoblinCount = StageManager.Instance.CurrentStageData.archerGoblincount;
        bossGoblinCount = StageManager.Instance.CurrentStageData.bossGoblinCount;
        
        // result.enemies = SpawnEnemies();
        result.enemies = new List<GameObject>();
        
        // �⺻ ��� ����
        for (int i = 0; i < baseGoblinCount; i++)
        {
            GameObject enemy = ObjectPoolManager.Instance.Get("enemy");            // �� Ű���� enum���� �����ǵ��� ����
            enemy.transform.SetParent(StageManager.Instance.enemyParent.transform);
            enemy.transform.position = 
                new Vector2(enemyPosX + 20 * ((StageManager.Instance.StageLevel - 1) % 5), enemyPosY);
            result.enemies.Add(enemy);
            StageManager.Instance.AddMonsterToList(enemy);
        }
        
        // �ü� ��� ����
        for (int i = 0; i < archerGoblinCount; i++)
        {
            GameObject enemy = ObjectPoolManager.Instance.Get("Archer");
            enemy.transform.SetParent(StageManager.Instance.enemyParent.transform);
            enemy.transform.position = 
                new Vector2(enemyPosX + 20 * ((StageManager.Instance.StageLevel - 1) % 5), enemyPosY);
            result.enemies.Add(enemy);
            StageManager.Instance.AddMonsterToList(enemy);
        }
        
        // ���� ��� ����
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