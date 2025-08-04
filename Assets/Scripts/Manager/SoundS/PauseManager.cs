using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoSingleton<PauseManager>
{
    private bool isPaused = false;

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainGameScene" && Input.GetKeyDown(KeyCode.Escape))
        {
            SetPause(!isPaused);  // ESC�� ��� ó��
        }
    }

    // �Ͻ����� ���� ���� (�ܺο����� ȣ�� ����)
    public void SetPause(bool pause)
    {
        if (isPaused == pause) return;

        isPaused = pause;

        if (pause)
        {
            Time.timeScale = 0f;                      // ���� ����
            SoundManager.Instance.PauseMusic();       // ������� �Ͻ�����
            // TODO : �Ͻ����� UI Ȱ��ȭ ó��
        }
        else
        {
            Time.timeScale = 1f;                      // ���� �簳
            SoundManager.Instance.ResumeMusic();      // ������� ��� �簳
            // TODO : �Ͻ����� UI ��Ȱ��ȭ ó��
        }
    }
}
