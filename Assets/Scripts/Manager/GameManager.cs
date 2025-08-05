using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// GameManager는 게임의 흐름을 감지하고 그 흐름에 맞게 씬을 변경해주는 역할
public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private int killMonsterCount;
    public int KillMonsterCount
    {
        get { return killMonsterCount; }
        set { killMonsterCount = value; }
    }

    [SerializeField] private int finalFloor;
    public int FinalFloor
    {
        get { return finalFloor; }
        set { finalFloor = value; }
    }

    public Sprite monsterImage;
    
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        killMonsterCount = 0;
    }

    public void LoadTitleScene()
    {
        SceneManager.LoadScene("Scenes/UI/TitleUI");
    }

    public void LoadMainGameScene()
    {
        SceneManager.LoadScene("Scenes/JYJ_DungeonTestScene");
    }

    public void LoadResultWinScene()
    {
        SceneManager.LoadScene("EndGame(Win)UI");
    }
    
    public void LoadResultDeadScene()
    {
        SceneManager.LoadScene("Scenes/UI/EndGame(Dead)UI");
    }

    public void AddKillMonsterCount()
    {
        killMonsterCount++;
    }
}
