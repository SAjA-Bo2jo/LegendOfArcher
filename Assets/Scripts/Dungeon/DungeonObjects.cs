using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ���� ������ ������ ������Ʈ ������ ��� �����̳� Ŭ����
public class DungeonObjects
{
    public GameObject dungeonRoot;
    public GameObject player;
    public GateController entryGate;
    public GateController exitGate;
    public List<GameObject> enemies;
    public List<GameObject> obstacles;

    public DungeonObjects()
    {
        enemies = new List<GameObject>();
        obstacles = new List<GameObject>();
    }
}
