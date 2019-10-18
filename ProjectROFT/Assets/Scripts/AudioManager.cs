using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public new static AudioManager audio;

    [System.Serializable]
    public class Audio
    {
        public string name; // Name of the audio

        public AudioClip clip; //The Audio Clip Reference

        [Range(0f, 1f)]
        public float volume; //Adjust Volume

        [Range(.1f, 3f)]
        public float pitch; //Adject pitch

        public bool enableLoop; //If the audio can repeat

        [HideInInspector] public AudioSource source;
    }

    public Slider musicVolumeAdjust, soundVolumeAdjust; //Reference to our volume sliders

    public Audio[] getAudio;

    private void Awake()
    {
        if (audio == null)
        {
            audio = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }

        foreach (Audio a in getAudio)
        {
            a.source = gameObject.AddComponent<AudioSource>();

            a.source.clip = a.clip;

            a.source.volume = a.volume;
            a.source.pitch = a.pitch;
            a.source.loop = a.enableLoop;
        }
    }
    /// <summary>
    /// Play audio and adjust its volume.
    /// </summary>
    /// 
    /// <param name="_name"></param>
    /// The audio clip by name.
    /// 
    /// <param name="_volume"></param>
    /// Support values between 0 and 100.
    ///

    public void Play(string _name, float _volume = 100)
    {
        Audio a = Array.Find(getAudio, sound => sound.name == _name);
        if (a == null)
        {
            Debug.LogWarning("Sound name " + _name + " was not found.");
            return;
        } else
        {
            a.source.Play();
            a.source.volume = _volume / 100;
        }
    }
    public void Stop(string _name)
    {
        Audio a = Array.Find(getAudio, sound => sound.name == _name);
        if (a == null)
        {
            Debug.LogWarning("Sound name " + _name + " was not found.");
            return;
        }
        else
        {
            a.source.Stop();
        }
    }

    public AudioClip GetAudio(string _name, float _volume = 100)
    {
        Audio a = Array.Find(getAudio, sound => sound.name == _name);
        if (a == null)
        {
            Debug.LogWarning("Sound name " + _name + " was not found.");
            return null;
        }
        else
        {
            a.source.Play();
            a.source.volume = _volume / 100;
            return a.source.clip;
        }
    }
}
