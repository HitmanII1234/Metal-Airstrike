using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource menuSource;
    public AudioSource gameplaySource;

    [Range(0f,1f)] public float musicVolume = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Cargar el volumen guardado
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        PlayMenuMusic();
        UpdateVolume();
    }

    public void PlayMenuMusic()
    {
        if (gameplaySource != null && gameplaySource.isPlaying) gameplaySource.Stop();
        if (menuSource != null)
        {
            menuSource.loop = true;
            if (!menuSource.isPlaying) menuSource.Play();
        }
    }

    public void PlayGameplayMusic()
    {
        if (menuSource != null && menuSource.isPlaying) menuSource.Stop();
        if (gameplaySource != null)
        {
            gameplaySource.loop = true;
            if (!gameplaySource.isPlaying) gameplaySource.Play();
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateVolume();
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }

    void UpdateVolume()
    {
        if (menuSource != null) menuSource.volume = musicVolume;
        if (gameplaySource != null) gameplaySource.volume = musicVolume;
    }
}
