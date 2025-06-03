using UnityEngine;

public class MusicManager3 : MonoBehaviour
{
    public AudioClip gameOverMusic;
        private AudioSource audioSource;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();

            audioSource.clip = gameOverMusic;
            audioSource.loop = true;
            audioSource.Play();

            DontDestroyOnLoad(gameObject);
        }
}
