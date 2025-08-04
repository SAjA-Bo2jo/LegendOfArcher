using System.Collections;
using UnityEngine;

// ���� ȿ���� ���带 ���� ����ϴ� ������Ʈ
// ��� �� �ڵ����� ���� ������Ʈ�� �����Ͽ� �޸� ����
public class SoundSource : MonoBehaviour
{
    private AudioSource _audioSource;

    // ȿ���� ��� �޼���
    // ����, ��ġ �������� ���ڷ� �޾� ���
    public void Play(AudioClip clip, float soundEffectVolume, float soundEffectPitchVariance)
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        // �ߺ� ȣ�� ������ ���� Disable ���� ȣ�� ���
        CancelInvoke();

        _audioSource.clip = clip;
        _audioSource.volume = soundEffectVolume;

        // ��ġ ���� ���� (�⺻ 1 + �������� ���� ����)
        _audioSource.pitch = 1f + Random.Range(-soundEffectPitchVariance, soundEffectPitchVariance);
        _audioSource.Play();

        // Ŭ�� ���� + 2�� �� Disable �޼��� ȣ�� ����
        Invoke("Disable", clip.length + 2f);
    }

    // ��� ���� �� ����� ���� �� ���ӿ�����Ʈ ����
    public void Disable()
    {
        _audioSource.Stop();
        Destroy(gameObject);
    }
}
