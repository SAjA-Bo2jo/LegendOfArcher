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
        SceneManager.LoadScene("MainGame");
    }

    public void OnCharacterButton()
    {
        if (characterUI != null)
            characterUI.SetActive(true);
    }

    public void OnCharacterBackButton()
    {
        if (characterUI != null)
            characterUI.SetActive(false);
    }

    public void OnOptionButton()
    {
        if (optionPanel != null)
            optionPanel.SetActive(true);
    }

    public void OnExitButton()
    {
        Debug.Log("게임 종료 시도");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
