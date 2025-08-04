using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoSingleton<PauseManager>
{
    private bool isPaused = false;

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainGameScene" && Input.GetKeyDown(KeyCode.Escape))
        {
            SetPause(!isPaused);  // ESC로 토글 처리
        }
    }

    // 일시정지 상태 설정 (외부에서도 호출 가능)
    public void SetPause(bool pause)
    {
        if (isPaused == pause) return;

        isPaused = pause;

        if (pause)
        {
            Time.timeScale = 0f;                      // 게임 정지
            SoundManager.Instance.PauseMusic();       // 배경음악 일시정지
            // TODO : 일시정지 UI 활성화 처리
        }
        else
        {
            Time.timeScale = 1f;                      // 게임 재개
            SoundManager.Instance.ResumeMusic();      // 배경음악 재생 재개
            // TODO : 일시정지 UI 비활성화 처리
        }
    }
}
