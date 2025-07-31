using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_TempStageManager : MonoBehaviour
{
    [SerializeField] private DungeonBuilder dungeonBuilder;

    private DungeonObjects dungeon;


    private void Start()
    {
        if (dungeonBuilder == null)
        {
            Debug.LogError("DungeonBuilder�� ������� �ʾҽ��ϴ�!");
            return;
        }

        dungeon = dungeonBuilder.Build();

        // ����Ʈ �浹 �̺�Ʈ ���� - ����Ʈ �浹 �� �޼���� �������� �̵� �޼��带 �����ϴ� �ڵ�
        if (dungeon.exitGate == null)
            Debug.LogError("exitGate ������ ������ ������ϴ�!");
        
        dungeon.exitGate.OnPlayerEnterExitGate = StageClear;


        // �� ������Ʈ ��ũ��Ʈ ������ - �÷��̾� ����
        //  PlayerController playerCtrl = dungeon.player.GetComponent<PlayerController>();
        //  playerCtrl.Init();
    }

    // ���ÿ� �޼��� - �ӽ� �������� Ŭ���� ó�� �޼���
    void StageClear()
    {
        Debug.Log("���� ���������� �̵� ó��");
    }
}
