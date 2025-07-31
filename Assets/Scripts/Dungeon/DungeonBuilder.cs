using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DungeonBuilder : MonoBehaviour
{

    // ���� ������ �غ�
    [SerializeField] private GameObject dungeon;
    private float dungeonPosX = 0;
    private float dungeonPosY = 0;
    Vector3 dungeonPos = Vector3.zero;

    // ����Ʈ ������ �غ�
    [SerializeField] private GameObject entryGate;
    private float entryGatePosX = -6.5f;
    private float entryGatePosY = 4f;
    Vector3 entryGatePos = Vector3.zero;
    [SerializeField] private GameObject exitGate;
    private float exitGatePosX = 6.5f;
    private float exitGatePosY = 4f;
    Vector3 exitGatePos = Vector3.zero;

    public void Build()
    {
        // ���� ����
        dungeonPos = new Vector3(dungeonPosX, dungeonPosY);
        Instantiate(dungeon, dungeonPos, Quaternion.identity);
        
        // ����Ʈ ���� - �Ա�
        entryGatePos = new Vector3(entryGatePosX, entryGatePosY);
        GameObject entry = Instantiate(entryGate, entryGatePos, Quaternion.identity);
        entry.GetComponent<GateController>().SetGateType(GateType.Entry);

        // ����Ʈ ���� - �ⱸ
        exitGatePos = new Vector3(exitGatePosX, exitGatePosY);
        GameObject exit = Instantiate(exitGate, exitGatePos, Quaternion.identity);
        exit.GetComponent<GateController>().SetGateType(GateType.Exit); 
    }
}
