using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoSingleton<StageManager>
{
    // 스테이지 레벨을 저장하고 스테이지를 클리어할 때 마다 레벨이 증가
    // 스테이지 레벨이 올라갈 수록 난이도를 어렵게 설정할 예정 -> 레벨에 따라 추가 체력, 추가 공격력, 추가 화살 등등?
    // 스테이지 레벨이 5의 배수일 때마다 보스 몬스터 스폰
    // 

    private int stageLevel = 0;
    private bool isClear = false; // 플레이어가 몬스터를 다 잡아서 스테이지를 클리어 했는지 판별
}
