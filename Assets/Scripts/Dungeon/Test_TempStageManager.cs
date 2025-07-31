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
            Debug.LogError("DungeonBuilder가 연결되지 않았습니다!");
            return;
        }

        dungeon = dungeonBuilder.Build();

        // 게이트 충돌 이벤트 연결 - 게이트 충돌 시 메서드와 스테이지 이동 메서드를 통일하는 코드
        if (dungeon.exitGate == null)
            Debug.LogError("exitGate 생성에 문제가 생겼습니다!");
        
        dungeon.exitGate.OnPlayerEnterExitGate = StageClear;


        // 각 오브젝트 스크립트 연결방법 - 플레이어 예시
        //  PlayerController playerCtrl = dungeon.player.GetComponent<PlayerController>();
        //  playerCtrl.Init();
    }

    // 예시용 메서드 - 임시 스테이지 클리어 처리 메서드
    void StageClear()
    {
        Debug.Log("다음 스테이지로 이동 처리");
    }
}
