using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] musicSound, sfxSound;
    public AudioSource musicSource, sFXSource;

    private void Awake()
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

    private void Start()
    {
        // Lần đầu chơi 
        if (PlayerPrefs.GetInt(DataKey.Open_Count) == 1)
        {
            PlayerPrefs.SetInt(DataKey.Use_Music, 1);
            PlayerPrefs.SetInt(DataKey.Use_Vibrate, 1);
        }
        
        if (DataKey.IsUseMusic())
        {
            MuteMusic(false);
            MuteSFX(false);
        }
        else
        {
            MuteMusic(true);
            MuteSFX(true);
        }
    }

    public void PlayMusic(string name, float volume = 1f)
    {
        Sound sound = Array.Find(musicSound, x => x.name == name);
        
        if (sound == null)
        {
            Debug.Log("Music Not Exist");
        }
        else
        {
            musicSource.clip = sound.clip;
            musicSource.Play();
        }
    }
    
    public void PlaySFX(string name, float volume = 1f)
    {
        Sound sound = Array.Find(sfxSound, x => x.name == name);

        if (sound == null)
        {
            Debug.Log("SFX Not Exist");
        }
        else
        {
            sFXSource.PlayOneShot(sound.clip, volume);
        }
    }
    
    public void StopMusic()
    {
        musicSource.Stop();
    }
    
    public void MuteMusic(bool canPlayMusic)
    {
        musicSource.mute = canPlayMusic;
    }

    public void MuteSFX(bool canPlaySFX)
    {
        sFXSource.mute = canPlaySFX;
    }
}

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}