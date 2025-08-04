using UnityEngine;
using UnityEngine.SceneManagement;

// MonoSingleton<T>를 상속받아 게임 전체에 단 하나만 존재하는 일시정지 관리 클래스
public class PauseManager : MonoSingleton<PauseManager>
{
    private bool isPaused = false; // 현재 게임이 일시정지 상태인지 저장하는 변수

    // 외부에서 일시정지 상태를 변경할 때 호출하는 메서드
    // pause: true면 게임 일시정지, false면 게임 재개
    public void SetPause(bool pause)
    {
        if (isPaused == pause) return; // 이미 요청한 상태면 중복 처리 방지

        isPaused = pause; // 상태 업데이트

        if (pause)
        {
            Time.timeScale = 0f;                    // 게임 시간 멈춤(모든 업데이트 및 물리 연산 정지)
            SoundManager.Instance.PauseMusic();     // 배경음악 일시정지 처리
            // TODO: 일시정지 UI 활성화 코드 추가 가능 (예: Pause창 보이기)
        }
        else
        {
            Time.timeScale = 1f;                    // 게임 시간 재개
            SoundManager.Instance.ResumeMusic();    // 배경음악 재생 재개
            // TODO: 일시정지 UI 비활성화 코드 추가 가능 (예: Pause창 숨기기)
        }
    }

    // Pause 창이 열릴 때 호출하는 메서드 (주로 UI 이벤트에서 호출)
    public void OnPauseWindowOpened()
    {
        SetPause(true); // 게임을 일시정지 상태로 변경
    }

    // Pause 창이 닫힐 때 호출하는 메서드 (주로 UI 이벤트에서 호출)
    public void OnPauseWindowClosed()
    {
        SetPause(false); // 게임을 재개 상태로 변경
    }
}
