using UnityEngine;
using UnityEngine.UI;

public class SoundMusicSettingsManager : MonoBehaviour
{
    [Header("Sound Settings")]
    public Slider soundSlider;                 // 소리 음량 조절 슬라이더
    public Button soundMuteButton;             // 소리 음소거 토글 버튼
    public Image soundMuteImage;               // 소리 음소거 상태 이미지 (버튼 내 Image)
    public Sprite soundIcon;                   // 소리 일반 아이콘
    public Sprite soundMuteIcon;               // 소리 음소거 아이콘

    [Header("Music Settings")]
    public Slider musicSlider;                 // 음악 음량 조절 슬라이더
    public Button musicMuteButton;             // 음악 음소거 토글 버튼
    public Image musicMuteImage;               // 음악 음소거 상태 이미지
    public Sprite musicIcon;                   // 음악 일반 아이콘
    public Sprite musicMuteIcon;               // 음악 음소거 아이콘

    private float prevSoundVolume = 100f;     // 음소거 해제 시 복원용 음량
    private float prevMusicVolume = 100f;

    private bool isSoundMuted = false;
    private bool isMusicMuted = false;

    private void Start()
    {
        // 슬라이더 Min Max 값 설정 (설정해도 UI에서 지정해도 무방)
        soundSlider.minValue = 0;
        soundSlider.maxValue = 100;
        musicSlider.minValue = 0;
        musicSlider.maxValue = 100;

        // 초기값 세팅 (예: 100)
        soundSlider.value = prevSoundVolume;
        musicSlider.value = prevMusicVolume;

        // 슬라이더 이벤트 연결
        soundSlider.onValueChanged.AddListener(OnSoundSliderChanged);
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);

        // 버튼 클릭 이벤트 연결
        soundMuteButton.onClick.AddListener(OnSoundMuteToggle);
        musicMuteButton.onClick.AddListener(OnMusicMuteToggle);

        // 초기 이미지 상태 맞추기
        UpdateSoundMuteImage();
        UpdateMusicMuteImage();
    }

    // 슬라이더 값 변경시 처리 (음소거 이미지 토글 여부 포함)
    private void OnSoundSliderChanged(float value)
    {
        if (value <= 0f)
        {
            isSoundMuted = true;
        }
        else
        {
            isSoundMuted = false;
            prevSoundVolume = value; // 현재 음량 저장
        }
        UpdateSoundMuteImage();

        // 여기에 실제 오디오 믹서나 AudioSource 음량 조절 코드 추가 가능
        // 예: AudioManager.Instance.SetSoundVolume(value/100f);
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

        // 여기에 실제 음악 볼륨 조절 코드 추가 가능
        // 예: AudioManager.Instance.SetMusicVolume(value/100f);
    }

    // Sound 음소거 토글 버튼 클릭시
    private void OnSoundMuteToggle()
    {
        if (isSoundMuted)
        {
            // 음소거 해제 → 이전 음량으로 슬라이더 복원
            soundSlider.value = prevSoundVolume;
        }
        else
        {
            // 음소거 ON → 슬라이더 0으로 설정
            soundSlider.value = 0;
        }
        // 슬라이더 값 변경 이벤트로 이미지 및 상태 갱신됨
    }

    // Music 음소거 토글 버튼 클릭시
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
