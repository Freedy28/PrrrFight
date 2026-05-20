using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Fuentes de Audio")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            CargarAjustesGuardados(); // Carga el volumen antes de que suene algo
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void CargarAjustesGuardados()
    {
        // Si ya hay un ajuste guardado, lo carga. Si es la primera vez que abre el juego, por defecto usa 1f (100%).
        musicSource.volume = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        sfxSource.volume = PlayerPrefs.GetFloat("VolumenSFX", 1f);
    }

    // Métodos para reproducir audio (los que ya tenías)
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip newMusic)
    {
        if (musicSource.clip == newMusic) return;
        musicSource.clip = newMusic;
        musicSource.Play();
    }

    // NUEVOS MÉTODOS: Controlan el volumen en tiempo real y guardan el dato
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("VolumenMusica", volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("VolumenSFX", volume);
    }
}