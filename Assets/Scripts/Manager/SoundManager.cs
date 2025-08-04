using UnityEngine;
using UnityEngine.SceneManagement;

// MonoSingleton<T>를 상속받아 싱글톤으로 동작하는 사운드 매니저 클래스
public class SoundManager : MonoSingleton<SoundManager>
{
    public SoundSource soundSourcePrefab; // 효과음 재생용 프리팹
    private AudioSource musicAudioSource; // 배경 음악 재생용 AudioSource

    [Range(0f, 1f)]
    public float musicVolume = 1f; // 배경음악 볼륨 (Inspector에서 0~1 사이 값 조정 가능)

    // 각 씬에 맞는 배경음악 AudioClip
    public AudioClip TitleMusic;
    public AudioClip MainGameMusic;
    public AudioClip DeadMusic;
    public AudioClip WinMusic;

    // MonoSingleton의 Awake를 오버라이드해 초기화 처리
    protected override void Awake()
    {
        base.Awake(); // 부모 Awake 호출로 싱글톤 및 DontDestroyOnLoad 처리

        // 현재 오브젝트에 AudioSource 컴포넌트가 있으면 가져오고, 없으면 추가
        musicAudioSource = GetComponent<AudioSource>();
        if (musicAudioSource == null)
            musicAudioSource = gameObject.AddComponent<AudioSource>();

        musicAudioSource.loop = true; // 배경음악 무한 반복 재생 설정
        musicAudioSource.volume = musicVolume;  // 초기 볼륨 설정
    }

    // 씬 로드 이벤트 등록 (상속받은 MonoSingleton의 가상 메서드 구현)
    protected override void RegisterSceneLoadedEvent()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬이 로드될 때 씬 이름에 맞는 음악 재생
    }

    // 씬 로드 이벤트 등록 해제
    protected override void UnregisterSceneLoadedEvent()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드되면 호출되는 메서드, 씬에 따라 적절한 음악 재생
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip clipToPlay = null;

        // 씬 이름에 따라 재생할 음악 선택
        switch (scene.name)
        {
            case "TitleScene":
                clipToPlay = TitleMusic;
                break;
            case "MainGameScene":
                clipToPlay = MainGameMusic;
                break;
            case "EndGameScene(Dead)":
                clipToPlay = DeadMusic;
                break;
            case "EndGameScene(Win)":
                clipToPlay = WinMusic;
                break;
        }

        // 만약 다른 음악이 재생 중일 경우에만 음악 교체 재생
        if (clipToPlay != null && musicAudioSource.clip != clipToPlay)
        {
            PlayBackgroundMusic(clipToPlay);
        }
    }

    // 배경음악 재생 메서드
    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (clip == null) return;

        // 중복 재생 방지: 이미 재생 중인 클립과 같으면 아무 행동 안 함
        if (musicAudioSource.clip == clip) return;

        musicAudioSource.Stop();      // 기존 음악 정지
        musicAudioSource.clip = clip; // 새 음악 할당
        musicAudioSource.Play();      // 음악 재생 시작
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

    // 효과음 재생 메서드
    // soundSourcePrefab을 인스턴스화하여 주어진 위치에서 재생
    // volume (볼륨), pitchVariance (음 높낮이 랜덤 변화) 기본값 지정되어 있음
    public void PlaySoundEffect(AudioClip clip, Vector3 position, float volume = 1f, float pitchVariance = 0f)
    {
        if (soundSourcePrefab == null || clip == null) return;

        // 효과음 재생용 프리팹 생성
        SoundSource obj = Instantiate(soundSourcePrefab, position, Quaternion.identity);

        // 생성된 SoundSource에서 효과음 재생 시작
        obj.Play(clip, volume, pitchVariance);
    }
}
