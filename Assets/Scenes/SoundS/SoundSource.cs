using System.Collections;
using UnityEngine;

// 단일 효과음 사운드를 개별 재생하는 컴포넌트
// 재생 후 자동으로 게임 오브젝트를 삭제하여 메모리 관리
public class SoundSource : MonoBehaviour
{
    private AudioSource _audioSource;

    // 효과음 재생 메서드
    // 볼륨, 피치 변조폭을 인자로 받아 재생
    public void Play(AudioClip clip, float soundEffectVolume, float soundEffectPitchVariance)
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        // 중복 호출 방지용 이전 Disable 예약 호출 취소
        CancelInvoke();

        _audioSource.clip = clip;
        _audioSource.volume = soundEffectVolume;

        // 피치 변조 적용 (기본 1 + ±변조폭 범위 랜덤)
        _audioSource.pitch = 1f + Random.Range(-soundEffectPitchVariance, soundEffectPitchVariance);
        _audioSource.Play();

        // 클립 길이 + 2초 후 Disable 메서드 호출 예약
        Invoke("Disable", clip.length + 2f);
    }

    // 재생 종료 후 오디오 정지 및 게임오브젝트 삭제
    public void Disable()
    {
        _audioSource.Stop();
        Destroy(gameObject);
    }
}
