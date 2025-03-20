using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
	private AudioSource source;
	[SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private AudioClip[] sfxClips;

    void Awake()
    {
		source = GetComponent<AudioSource>();
		if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);		
		}
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        PlayMusic(0); // Default background music
    }

    public void PlayMusic(int index)
    {
        if (musicSource.isPlaying)
            musicSource.Stop();

        musicSource.clip = musicClips[index];
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(int index)
    {
        sfxSource.PlayOneShot(sfxClips[index]); // Play sound effect without stopping the current one
	}

    public void PlaySFX(AudioClip audioClip)
    {
		source.PlayOneShot(audioClip);
	}

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
}