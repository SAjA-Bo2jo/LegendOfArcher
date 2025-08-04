using System.Collections;
using UnityEngine;

// ���� ȿ���� ���带 ���������� ����ϴ� ������Ʈ
// ȿ���� ����� ������ �ڵ����� �ش� ���� ������Ʈ�� �����Ͽ� �޸� ������ ������ ��
public class SoundSource : MonoBehaviour
{
    private AudioSource _audioSource; // ȿ���� ����� AudioSource ������Ʈ

    // ȿ���� ��� �޼���
    // clip: ����� ����� Ŭ��
    // soundEffectVolume: ȿ���� ���� (0~1 ���� ����)
    // soundEffectPitchVariance: ��ġ ���� ��, �⺻ ��ġ 1���� ������ �� ���� ������ ����
    public void Play(AudioClip clip, float soundEffectVolume, float soundEffectPitchVariance)
    {
        // AudioSource ������Ʈ�� ������ ���� ���ӿ�����Ʈ���� ������
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        // �ߺ� Disable ȣ�� ������ ���� ��� �����Ͽ� ����
        CancelInvoke();

        _audioSource.clip = clip;                      // ����� Ŭ�� ����
        _audioSource.volume = soundEffectVolume;       // ���� ����

        // ��ġ ���� ����: �⺻ 1 + ���� �� (-pitchVariance ~ +pitchVariance)
        _audioSource.pitch = 1f + Random.Range(-soundEffectPitchVariance, soundEffectPitchVariance);

        _audioSource.Play();                            // Ŭ�� ��� ����

        // Ŭ�� ���� + 2�� �� Disable �޼��� �ڵ� ȣ�� ����
        // ����� ����� �����ϰ� ���� ������Ʈ�� ������
        Invoke("Disable", clip.length + 2f);
    }

    // ��� ���� �� ����Ǵ� �޼���
    // ����� ����� ���߰� �� ������Ʈ�� ���� ���ӿ�����Ʈ�� �ı��Ͽ� ����
    public void Disable()
    {
        _audioSource.Stop();            // ����� ����
        Destroy(gameObject);            // ���ӿ�����Ʈ ���� (�޸� ����)
    }
}
