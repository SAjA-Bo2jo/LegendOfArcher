using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DungeonBuilder : MonoBehaviour
{

    // 던전 프리팹 준비
    [SerializeField] private GameObject dungeon;
    [SerializeField] private float dungeonPosX = 0;
    [SerializeField] private float dungeonPosY = 0;
    Vector3 dungeonPos = Vector3.zero;

    // 게이트 프리팹 준비
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
        // 던전 생성
        dungeonPos = new Vector3(dungeonPosX, dungeonPosY);
        Instantiate(dungeon, dungeonPos, Quaternion.identity);
        
        // 게이트 생성
        entryGatePos = new Vector3(entryGatePosX, entryGatePosY);
        Instantiate(entryGate, entryGatePos, Quaternion.identity);
        exitGatePos = new Vector3(exitGatePosX, exitGatePosY);
        Instantiate(exitGate, exitGatePos, Quaternion.identity);
    }
}
