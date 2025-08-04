using UnityEngine;
using UnityEngine.SceneManagement;

// ���� �Ͻ����� ���¸� �����ϴ� �̱��� Ŭ����
// ���� ���� ������ ESC Ű�� �Ͻ�����/�簳 ���� ����
public class PauseManager : MonoSingleton<PauseManager>
{
    private bool isPaused = false;  // ���� �Ͻ����� ���� �÷���

    void Update()
    {
        // 'MainGame' ������ ESC Ű �Է� üũ
        if (SceneManager.GetActiveScene().name == "MainGame" && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();   // �Ͻ����� ���̸� ���� �簳
            else
                PauseGame();    // �ƴ϶�� �Ͻ����� ����
        }
    }

    // ���� �Ͻ����� ó��: �ð� ����, ���� �Ͻ�����, �÷��� ����
    public void PauseGame()
    {
        Time.timeScale = 0f;
        SoundManager.Instance.PauseMusic();
        isPaused = true;

        // TODO: �Ͻ����� UI Ȱ��ȭ ó�� ����
    }

    // ���� �簳 ó��: �ð� ����, ���� ��� �簳, �÷��� �ʱ�ȭ
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        SoundManager.Instance.ResumeMusic();
        isPaused = false;

        // TODO: �Ͻ����� UI ��Ȱ��ȭ ó�� ����
    }
}
