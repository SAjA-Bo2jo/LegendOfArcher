using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameDeadUIManager : MonoBehaviour
{
    [SerializeField] private Text killScoreText;
    [SerializeField] private Text finalFloor;
    [SerializeField] private Image monsterImage;

    private void Start()
    {
        killScoreText.text = GameManager.Instance.KillMonsterCount.ToString();
        finalFloor.text = GameManager.Instance.FinalFloor.ToString();
        monsterImage.sprite = GameManager.Instance.monsterImage;
    }

    public void OnClickMainButton()
    {
        SoundManager.Instance.PlayButtonSound();
        
        GameManager.Instance.KillMonsterCount = 0;
        GameManager.Instance.FinalFloor = 1;
        
        GameManager.Instance.LoadTitleScene();
    }

    public void OnClickRetryButton()
    {
        SoundManager.Instance.PlayButtonSound();
        
        GameManager.Instance.KillMonsterCount = 0;
        GameManager.Instance.FinalFloor = 1;
        
        GameManager.Instance.LoadMainGameScene();
    }
}
