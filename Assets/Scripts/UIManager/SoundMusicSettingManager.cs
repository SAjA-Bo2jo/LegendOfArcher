using UnityEngine;
using UnityEngine.UI;

public class SoundMusicSettingsManager : MonoBehaviour
{
    [Header("Sound Settings")]
    public Slider soundSlider;                 // �Ҹ� ���� ���� �����̴�
    public Button soundMuteButton;             // �Ҹ� ���Ұ� ��� ��ư
    public Image soundMuteImage;               // �Ҹ� ���Ұ� ���� �̹��� (��ư �� Image)
    public Sprite soundIcon;                   // �Ҹ� �Ϲ� ������
    public Sprite soundMuteIcon;               // �Ҹ� ���Ұ� ������

    [Header("Music Settings")]
    public Slider musicSlider;                 // ���� ���� ���� �����̴�
    public Button musicMuteButton;             // ���� ���Ұ� ��� ��ư
    public Image musicMuteImage;               // ���� ���Ұ� ���� �̹���
    public Sprite musicIcon;                   // ���� �Ϲ� ������
    public Sprite musicMuteIcon;               // ���� ���Ұ� ������

    private float prevSoundVolume = 100f;     // ���Ұ� ���� �� ������ ����
    private float prevMusicVolume = 100f;

    private bool isSoundMuted = false;
    private bool isMusicMuted = false;

    private void Start()
    {
        // �����̴� Min Max �� ���� (�����ص� UI���� �����ص� ����)
        soundSlider.minValue = 0;
        soundSlider.maxValue = 100;
        musicSlider.minValue = 0;
        musicSlider.maxValue = 100;

        // �ʱⰪ ���� (��: 100)
        soundSlider.value = prevSoundVolume;
        musicSlider.value = prevMusicVolume;

        // �����̴� �̺�Ʈ ����
        soundSlider.onValueChanged.AddListener(OnSoundSliderChanged);
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);

        // ��ư Ŭ�� �̺�Ʈ ����
        soundMuteButton.onClick.AddListener(OnSoundMuteToggle);
        musicMuteButton.onClick.AddListener(OnMusicMuteToggle);

        // �ʱ� �̹��� ���� ���߱�
        UpdateSoundMuteImage();
        UpdateMusicMuteImage();
    }

    // �����̴� �� ����� ó�� (���Ұ� �̹��� ��� ���� ����)
    private void OnSoundSliderChanged(float value)
    {
        if (value <= 0f)
        {
            isSoundMuted = true;
        }
        else
        {
            isSoundMuted = false;
            prevSoundVolume = value; // ���� ���� ����
        }
        UpdateSoundMuteImage();

        // ���⿡ ���� ����� �ͼ��� AudioSource ���� ���� �ڵ� �߰� ����
        // ��: AudioManager.Instance.SetSoundVolume(value/100f);
    }

    private void OnMusicSliderChanged(float value)
    {
        if (value <= 0f)
        {
            isMusicMuted = true;
        }
        else
        {
            isMusicMuted = false;
            prevMusicVolume = value;
        }
        UpdateMusicMuteImage();

        // ���⿡ ���� ���� ���� ���� �ڵ� �߰� ����
        // ��: AudioManager.Instance.SetMusicVolume(value/100f);
    }

    // Sound ���Ұ� ��� ��ư Ŭ����
    private void OnSoundMuteToggle()
    {
        if (isSoundMuted)
        {
            // ���Ұ� ���� �� ���� �������� �����̴� ����
            soundSlider.value = prevSoundVolume;
        }
        else
        {
            // ���Ұ� ON �� �����̴� 0���� ����
            soundSlider.value = 0;
        }
        // �����̴� �� ���� �̺�Ʈ�� �̹��� �� ���� ���ŵ�
    }

    // Music ���Ұ� ��� ��ư Ŭ����
    private void OnMusicMuteToggle()
    {
        if (isMusicMuted)
        {
            musicSlider.value = prevMusicVolume;
        }
        else
        {
            musicSlider.value = 0;
        }
    }

    private void UpdateSoundMuteImage()
    {
        soundMuteImage.sprite = isSoundMuted ? soundMuteIcon : soundIcon;
    }

    private void UpdateMusicMuteImage()
    {
        musicMuteImage.sprite = isMusicMuted ? musicMuteIcon : musicIcon;
    }
}
