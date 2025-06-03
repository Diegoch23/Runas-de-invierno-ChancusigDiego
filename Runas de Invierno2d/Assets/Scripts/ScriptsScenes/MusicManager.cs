using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip menuMusic;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = menuMusic;
        audioSource.loop = true;
        audioSource.Play();

        DontDestroyOnLoad(gameObject);
    }
}
