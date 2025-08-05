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
    [SerializeField] private GameObject OptionUI;
    [SerializeField] private Slider HpBarSlider;
    [SerializeField] private Slider ExpBarSlider;
    [SerializeField] private GameObject currentPlayerStage;
    [SerializeField] private Text floorText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private Slider musicSlider;

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
        
        CalculateHpBar();
    }

    public void OnClickReturnButton()
    {
        SoundManager.Instance.PlayButtonSound();
        
        if (OptionUI.activeSelf)
        {
            OptionUI.SetActive(false);
        }
    }
    
    public void OnClickOptionButton()
    {
        SoundManager.Instance.PlayButtonSound();
        
        if (!OptionUI.activeSelf)
        {
            OptionUI.SetActive(true);
        }
    }
    
    public void OnClickExitButton()
    {
        SoundManager.Instance.PlayButtonSound();

        GameManager.Instance.LoadTitleScene();
    }
    
    public void OnClickResumeButton()
    {
        SoundManager.Instance.PlayButtonSound();
        
        if (isPause)
        {
            PauseUI.SetActive(false);
            InfoUI.SetActive(false);
            isPause = false;
            Time.timeScale = 1f;
        }
    }
    
    public void OnClickPauseButton()
    {
        SoundManager.Instance.PlayButtonSound();
        
        if (!isPause)
        {
            PauseUI.SetActive(true);
            InfoUI.SetActive(true);
            isPause = true;
            Time.timeScale = 0f;
        }
    }
    
    public void CalculatePlayerIconPos(int stageLevel)
    {
        // stageLevel -> pos, 1 -> -83, 2 -> -41.5, 3 -> 0, 4 -> 41.5, 5 -> 83
        switch (stageLevel % 5)
        {
            case 1:
                currentPlayerStage.GetComponent<RectTransform>().anchoredPosition = new Vector2(-83f, 0.5f);
                break;
            case 2:
                currentPlayerStage.GetComponent<RectTransform>().anchoredPosition = new Vector2(-41.5f, 0.5f);
                break;
            case 3:
                currentPlayerStage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0.5f);
                break;
            case 4:
                currentPlayerStage.GetComponent<RectTransform>().anchoredPosition = new Vector2(41.5f, 0.5f);
                break;
            case 0:
                currentPlayerStage.GetComponent<RectTransform>().anchoredPosition = new Vector2(83f, 0.5f);
                break;
        }
        
        CalculateFloorText(stageLevel);
    }

    private void CalculateFloorText(int stageLevel)
    {
        floorText.text = (((stageLevel - 1) / 5) + 1).ToString();
    }
    
    private void CalculateHpBar()
    {
        if (HpBarSlider == null)
            return;
        
        HpBarSlider.value = StageManager.Instance._Player.GetComponent<Player>().Health /
                            StageManager.Instance._Player.GetComponent<Player>().MaxHealth;
    }
    
    public void OnSoundSliderChanged(float value)
    {
        SoundManager.Instance.musicVolume = musicSlider.value;
    }
}
