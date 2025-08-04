using UnityEngine;
using UnityEngine.SceneManagement;

// 씬 전환 시 씬별 배경음악을 자동 재생하고,
// 효과음 재생, 배경음악 일시정지 및 재개 기능을 수행하는 싱글톤 클래스
public class SoundManager : MonoSingleton<SoundManager>
{
    public SoundSource soundSourcePrefab;     // 효과음 재생용 프리팹

    private AudioSource musicAudioSource;     // 배경음악 재생용 AudioSource
    public float musicVolume = 1f;             // 배경음악 볼륨

    // 씬별 배경음악 클립 (인스펙터에서 할당)
    public AudioClip mainMenuMusic;
    public AudioClip gameSceneMusic;
    public AudioClip scene3Music;
    public AudioClip scene4Music;

    // Awake에서 컴포넌트 생성 및 초기화
    protected override void Awake()
    {
        base.Awake();

        musicAudioSource = GetComponent<AudioSource>();
        if (musicAudioSource == null)
            musicAudioSource = gameObject.AddComponent<AudioSource>();

        musicAudioSource.loop = true;           // 반복 재생 설정
        musicAudioSource.volume = musicVolume;  // 볼륨 설정
    }

    // 씬 로드 이벤트 구독 등록
    protected override void RegisterSceneLoadedEvent()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 씬 로드 이벤트 구독 해제
    protected override void UnregisterSceneLoadedEvent()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 새로 로드되면 파악하여 씬 이름에 따른 배경음악 재생
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip clipToPlay = null;

        switch (scene.name)
        {
            case "TitleScene":                 // 타이틀 씬
                clipToPlay = mainMenuMusic;   // 타이틀 음악 재생 (Inspector에서 할당)
                break;
            case "MainGameScene":             // 메인 게임 씬
                clipToPlay = gameSceneMusic;  // 메인 게임 음악 재생
                break;
            case "EndGameScene(Dead)":        // 게임 오버(사망) 씬
                clipToPlay = scene3Music;     // 예: 사망씬 음악
                break;
            case "EndGameScene(Win)":         // 게임 승리 씬
                clipToPlay = scene4Music;     // 예: 승리씬 음악
                break;
        }

        if (clipToPlay != null)
        {
            PlayBackgroundMusic(clipToPlay);
        }
    }

    // 배경음악 재생 메서드
    // 다른 곡일 경우에만 음악 교체 및 재생
    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (musicAudioSource.clip == clip) return;

        musicAudioSource.Stop();
        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }

    // 배경음악 일시정지
    public void PauseMusic()
    {
        if (musicAudioSource.isPlaying)
            musicAudioSource.Pause();
    }

    // 배경음악 재개
    public void ResumeMusic()
    {
        if (!musicAudioSource.isPlaying && musicAudioSource.clip != null)
            musicAudioSource.UnPause();
    }

    // 효과음 재생: 위치와 볼륨, 피치 변조 인자 사용 가능
    public void PlaySoundEffect(AudioClip clip, Vector3 position, float volume = 1f, float pitchVariance = 0f)
    {
        if (soundSourcePrefab == null || clip == null) return;

        SoundSource obj = Instantiate(soundSourcePrefab, position, Quaternion.identity);
        obj.Play(clip, volume, pitchVariance);
    }
}
