using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct SoundParameters
{
    [Range(0, 1)]
    public float volume;
    [Range(0, 1)]
    public bool loop;
}
[Serializable]
public class Sound
{
    #region Variables

    [SerializeField]String name = String.Empty;
    public String Name { get { return name; } }

    [SerializeField] AudioClip  clip = null;
    public AudioClip Clip { get { return clip; } }

    [SerializeField] SoundParameters parameters;
    public SoundParameters Parameters { get { return parameters; } }

    [HideInInspector]
    public AudioSource Source;

    #endregion

    public void Play ()
    {
        Source.clip = Clip;
        Source.volume = parameters.volume;
        Source.loop = Parameters.loop;

        Source.Play();
    }
    public void Stop ()
    {
        Source.Stop();
    }
}
public class AudioManager : MonoBehaviour {

    #region Variables

    public static AudioManager Instance = null;

    [SerializeField] Sound[] sounds = null;
    [SerializeField] AudioSource sourcePrefab = null;

    [SerializeField] String startupTrack = String.Empty;


    #endregion
    
    
    void Awake()
    {
        if (Instance != null)
        { Destroy(gameObject); }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        InitializeSounds();
    }

    void Start()
    {
        if (string.IsNullOrEmpty(startupTrack) != true)
        {
            PlaySound(startupTrack);
        }
    }

    void InitializeSounds()
    {
        foreach (var sound in sounds)
        {
            AudioSource source = Instantiate(sourcePrefab, gameObject.transform);
            source.name = sound.Name;

            sound.Source = source;
        }
    }
    public void PlaySound(string name)
    {
        var sound = GetSound(name);
        if (sound != null)
        {
            sound.Play();
        }
        else
        {
            Debug.LogWarning("Sound by the name " + name + " is not found! Issues occured at AudioManager.PlaySound()");
        }
    }
    

    Sound GetSound(string name)
    {
        foreach (var sound in sounds)
        {
            if (sound.Name == name)
            {
                return sound;
            }
        }
        return null;
    }
}
