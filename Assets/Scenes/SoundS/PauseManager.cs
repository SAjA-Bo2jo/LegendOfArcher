using UnityEngine;
using UnityEngine.SceneManagement;

// 게임 일시정지 상태를 관리하는 싱글톤 클래스
// 메인 게임 씬에서 ESC 키로 일시정지/재개 동작 수행
public class PauseManager : MonoSingleton<PauseManager>
{
    private bool isPaused = false;  // 현재 일시정지 상태 플래그

    void Update()
    {
        // 'MainGame' 씬에서 ESC 키 입력 체크
        if (SceneManager.GetActiveScene().name == "MainGame" && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();   // 일시정지 중이면 게임 재개
            else
                PauseGame();    // 아니라면 일시정지 실행
        }
    }

    // 게임 일시정지 처리: 시간 정지, 음악 일시정지, 플래그 설정
    public void PauseGame()
    {
        Time.timeScale = 0f;
        SoundManager.Instance.PauseMusic();
        isPaused = true;

        // TODO: 일시정지 UI 활성화 처리 가능
    }

    // 게임 재개 처리: 시간 복원, 음악 재생 재개, 플래그 초기화
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        SoundManager.Instance.ResumeMusic();
        isPaused = false;

        // TODO: 일시정지 UI 비활성화 처리 가능
    }
}
