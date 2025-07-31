using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DungeonBuilder : MonoBehaviour
{

    // ���� ������ �غ�
    [SerializeField] private GameObject dungeon;
    [SerializeField] private float dungeonPosX = 0;
    [SerializeField] private float dungeonPosY = 0;
    Vector3 dungeonPos = Vector3.zero;

    // ����Ʈ ������ �غ�
    [SerializeField] private GameObject entryGate;
    [SerializeField] private float entryGatePosX = 0;
    [SerializeField] private float entryGatePosY = 0;
    Vector3 entryGatePos = Vector3.zero;
    [SerializeField] private GameObject exitGate;
    [SerializeField] private float exitGatePosX = 0;
    [SerializeField] private float exitGatePosY = 0;
    Vector3 exitGatePos = Vector3.zero;

    public void Build()
    {
        // ���� ����
        dungeonPos = new Vector3(dungeonPosX, dungeonPosY);
        Instantiate(dungeon, dungeonPos, Quaternion.identity);
        
        // ����Ʈ ����
        entryGatePos = new Vector3(entryGatePosX, entryGatePosY);
        Instantiate(entryGate, entryGatePos, Quaternion.identity);
        exitGatePos = new Vector3(exitGatePosX, exitGatePosY);
        Instantiate(exitGate, exitGatePos, Quaternion.identity);
    }
}
