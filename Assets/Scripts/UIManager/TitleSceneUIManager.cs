using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneUIManager : MonoBehaviour
{
    [SerializeField] private GameObject characterUI;  // ĳ���� UI �г�
    [SerializeField] private GameObject optionPanel;  // �ɼ� UI �г�

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
        Debug.Log("���� ���� �õ�");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
