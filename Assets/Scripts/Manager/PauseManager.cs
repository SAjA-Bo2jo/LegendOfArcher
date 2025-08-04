using UnityEngine;
using UnityEngine.SceneManagement;

// MonoSingleton<T>�� ��ӹ޾� ���� ��ü�� �� �ϳ��� �����ϴ� �Ͻ����� ���� Ŭ����
public class PauseManager : MonoSingleton<PauseManager>
{
    private bool isPaused = false; // ���� ������ �Ͻ����� �������� �����ϴ� ����

    // �ܺο��� �Ͻ����� ���¸� ������ �� ȣ���ϴ� �޼���
    // pause: true�� ���� �Ͻ�����, false�� ���� �簳
    public void SetPause(bool pause)
    {
        if (isPaused == pause) return; // �̹� ��û�� ���¸� �ߺ� ó�� ����

        isPaused = pause; // ���� ������Ʈ

        if (pause)
        {
            Time.timeScale = 0f;                    // ���� �ð� ����(��� ������Ʈ �� ���� ���� ����)
            SoundManager.Instance.PauseMusic();     // ������� �Ͻ����� ó��
            // TODO: �Ͻ����� UI Ȱ��ȭ �ڵ� �߰� ���� (��: Pauseâ ���̱�)
        }
        else
        {
            Time.timeScale = 1f;                    // ���� �ð� �簳
            SoundManager.Instance.ResumeMusic();    // ������� ��� �簳
            // TODO: �Ͻ����� UI ��Ȱ��ȭ �ڵ� �߰� ���� (��: Pauseâ �����)
        }
    }

    // Pause â�� ���� �� ȣ���ϴ� �޼��� (�ַ� UI �̺�Ʈ���� ȣ��)
    public void OnPauseWindowOpened()
    {
        SetPause(true); // ������ �Ͻ����� ���·� ����
    }

    // Pause â�� ���� �� ȣ���ϴ� �޼��� (�ַ� UI �̺�Ʈ���� ȣ��)
    public void OnPauseWindowClosed()
    {
        SetPause(false); // ������ �簳 ���·� ����
    }
}
