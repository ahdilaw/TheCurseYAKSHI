using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] musicSound, sfxSound;
    public AudioSource musicSource, sfxSource,BackgroundSfx;

    public static AudioManager instance;
    public string currentMusic, currentSFX;

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        
    }
    private void Start()
    {
        PlayMusic("MainTheme");
    }
   public void PlayMusic(string name)
    {
        Sound s = System.Array.Find(musicSound, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        musicSource.clip = s.clip;
        musicSource.loop = s.loop;
        musicSource.volume = s.volume;
        musicSource.Play();
    }

    public void PlaySFX(string name)
    {
        currentSFX = name;
        Sound s = System.Array.Find(sfxSound, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        sfxSource.clip = s.clip;
        sfxSource.volume = s.volume;
        sfxSource.loop = s.loop;
        sfxSource.Play();

    }
    public bool IsPlayingSFX(string name)
    {
        return sfxSource.isPlaying && currentSFX == name;
    }
    public void StopSFX()
    {
        sfxSource.Stop();
    }
    public void PlayBackgroundSFX(string name)
    {
        Sound s = System.Array.Find(sfxSound, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        BackgroundSfx.clip = s.clip;
        BackgroundSfx.loop = s.loop;
        BackgroundSfx.volume = s.volume;
        BackgroundSfx.Play();
    }
    public bool IsPlayingBackgroundSFX(string name)
    {
        return BackgroundSfx.isPlaying && currentSFX == name;
    }
    public void StopBackgroundSFX()
    {
        BackgroundSfx.Stop();
    }
}
