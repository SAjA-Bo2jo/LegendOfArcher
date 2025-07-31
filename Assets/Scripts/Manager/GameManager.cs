using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

// GameManager는 게임의 흐름을 감지하고 그 흐름에 맞게 씬을 변경해주는 역할
public class GameManager : MonoSingleton<GameManager>
{
    public void LoadTitleScene()
    {
        // SceneManager.LoadScene("TitleScene");
    }

    public void LoadMainGameScene()
    {
        // SceneManager.LoadScene("MainGameScene");
    }
}
