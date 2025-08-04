using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoSingleton<SoundManager>
{
    public SoundSource soundSourcePrefab;
    private AudioSource musicAudioSource;

    [Range(0f, 1f)]
    public float musicVolume = 1f; // Inspector에서 슬라이더로 조정가능 (0~1)

    public AudioClip TitleMusic;
    public AudioClip MainGameMusic;
    public AudioClip DeadMusic;
    public AudioClip WinMusic;

    protected override void Awake()
    {
        base.Awake();

        musicAudioSource = GetComponent<AudioSource>();
        if (musicAudioSource == null)
            musicAudioSource = gameObject.AddComponent<AudioSource>();

        musicAudioSource.loop = true;
        musicAudioSource.volume = musicVolume;  // 초기 볼륨 적용
    }

    protected override void RegisterSceneLoadedEvent()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void UnregisterSceneLoadedEvent()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip clipToPlay = null;

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

        if (clipToPlay != null && musicAudioSource.clip != clipToPlay)
        {
            PlayBackgroundMusic(clipToPlay);
        }
    }

    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicAudioSource.clip == clip) return; // 중복 재생 방지

        musicAudioSource.Stop();
        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }

    public void PauseMusic()
    {
        if (musicAudioSource.isPlaying)
            musicAudioSource.Pause();
    }

    public void ResumeMusic()
    {
        if (!musicAudioSource.isPlaying && musicAudioSource.clip != null)
            musicAudioSource.UnPause();
    }

    // 효과음 재생, volume과 pitch variance는 기본값 설정 가능
    public void PlaySoundEffect(AudioClip clip, Vector3 position, float volume = 1f, float pitchVariance = 0f)
    {
        if (soundSourcePrefab == null || clip == null) return;

        SoundSource obj = Instantiate(soundSourcePrefab, position, Quaternion.identity);
        obj.Play(clip, volume, pitchVariance);
    }
}
