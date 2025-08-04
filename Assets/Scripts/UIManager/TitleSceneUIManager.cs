using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneUIManager : MonoBehaviour
{
    [SerializeField] private GameObject characterUI;  // 캐릭터 UI 패널
    [SerializeField] private GameObject optionPanel;  // 옵션 UI 패널

    private void Start()
    {
        if (characterUI != null)
            characterUI.SetActive(false);

        if (optionPanel != null)
            optionPanel.SetActive(false);
    }

    public void OnGameStartButton()
    {
        SoundManager.Instance.PlayButtonSound();
        SceneManager.LoadScene("JYJ_DungeonTestScene");
    }

    public void OnCharacterButton()
    {
        SoundManager.Instance.PlayButtonSound();
        
        if (characterUI != null)
            characterUI.SetActive(true);
    }

    public void OnCharacterBackButton()
    {
        SoundManager.Instance.PlayButtonSound();
        
        if (characterUI != null)
            characterUI.SetActive(false);
    }

    public void OnOptionButton()
    {
        SoundManager.Instance.PlayButtonSound();
        
        if (optionPanel != null)
            optionPanel.SetActive(true);
    }

    public void OnExitButton()
    {
        SoundManager.Instance.PlayButtonSound();
        
        Debug.Log("게임 종료 시도");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
