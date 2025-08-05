using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameWinUIManager : MonoBehaviour
{
    [SerializeField] private Text killScoreText;
    [SerializeField] private Text finalFloor;
    
    private void Start()
    {
        killScoreText.text = GameManager.Instance.KillMonsterCount.ToString();
        finalFloor.text = GameManager.Instance.FinalFloor.ToString();
    }
    
    public void OnClickMainButton()
    {
        SoundManager.Instance.PlayButtonSound();
        
        GameManager.Instance.KillMonsterCount = 0;
        GameManager.Instance.FinalFloor = 1;
        
        GameManager.Instance.LoadTitleScene();
    }
}
