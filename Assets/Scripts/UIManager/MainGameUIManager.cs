using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject GameUI;
    [SerializeField] private GameObject PauseUI;
    [SerializeField] private GameObject InfoUI;
    [SerializeField] private Slider HpBarSlider;
    [SerializeField] private Slider ExpBarSlider;

    private bool isPause = false;
    
    private void Start()
    {
        if (GameUI == null)
        {
            Debug.LogError("GameUI가 비었습니다!");
        }
        
        GameUI.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPause)
            {
                PauseUI.SetActive(true);
                InfoUI.SetActive(true);
                isPause = true;
                Time.timeScale = 0f;
            }
            else
            {
                PauseUI.SetActive(false);
                InfoUI.SetActive(false);
                isPause = false;
                Time.timeScale = 1f;
            }
        }
    }
}
