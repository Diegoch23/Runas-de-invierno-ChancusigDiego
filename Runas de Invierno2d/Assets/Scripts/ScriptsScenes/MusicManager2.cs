using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager2 : MonoBehaviour
{
    public AudioClip gameMusic;
    public AudioClip gameOverMusic;

    private AudioSource audioSource;

    private static MusicManager2 instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoaded;

        PlayMusic();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "FinalScene")
        {
            // Detener la música del juego cuando carga la escena FinalScene
            StopMusic();

            // Reproducir música de Game Over si tienes asignada
            if(gameOverMusic != null)
            {
                audioSource.clip = gameOverMusic;
                audioSource.loop = false; // o true si quieres que se repita
                audioSource.Play();
            }
        }
        else
        {
            // En cualquier otra escena, aseguramos que se reproduzca la música del juego
            PlayMusic();
        }
    }

    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = gameMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
