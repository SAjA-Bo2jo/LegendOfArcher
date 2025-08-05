using System.Collections;
using UnityEngine;

// 단일 효과음 사운드를 개별적으로 재생하는 컴포넌트
// 효과음 재생이 끝나면 자동으로 해당 게임 오브젝트를 삭제하여 메모리 관리에 도움을 줌
public class SoundSource : MonoBehaviour
{
    private AudioSource _audioSource; // 효과음 재생용 AudioSource 컴포넌트

    // 효과음 재생 메서드
    // clip: 재생할 오디오 클립
    // soundEffectVolume: 효과음 볼륨 (0~1 범위 권장)
    // soundEffectPitchVariance: 피치 변조 폭, 기본 피치 1에서 ±범위 내 랜덤 변조를 적용
    public void Play(AudioClip clip, float soundEffectVolume, float soundEffectPitchVariance)
    {
        // AudioSource 컴포넌트가 없으면 현재 게임오브젝트에서 가져옴
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        // 중복 Disable 호출 예약이 있을 경우 중지하여 방지
        CancelInvoke();

        _audioSource.clip = clip;                      // 재생할 클립 설정
        _audioSource.volume = soundEffectVolume;       // 볼륨 설정

        // 피치 변조 적용: 기본 1 + 랜덤 값 (-pitchVariance ~ +pitchVariance)
        _audioSource.pitch = 1f + Random.Range(-soundEffectPitchVariance, soundEffectPitchVariance);

        _audioSource.Play();                            // 클립 재생 시작

        // 클립 길이 + 2초 후 Disable 메서드 자동 호출 예약
        // 재생을 충분히 보장하고 나서 오브젝트를 제거함
        Invoke("Disable", clip.length + 2f);
    }

    // 재생 종료 후 실행되는 메서드
    // 오디오 재생을 멈추고 이 컴포넌트가 붙은 게임오브젝트를 파괴하여 정리
    public void Disable()
    {
        _audioSource.Stop();            // 오디오 정지
        Destroy(gameObject);            // 게임오브젝트 제거 (메모리 해제)
    }
}
