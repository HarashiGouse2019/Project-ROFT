using System;
using UnityEngine.UI;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    /*Music Manager will essentially collect all music files
     * from all SongEntities detected by RoftScouter,
     * and it will preview it for the player.
     * 
     * Music Manager will later be changed to
     * SongEntityManager, because instead of just Music clips,
     * it'll be a class that holds not only the song (in which it'll go through and play),
     * but give us all kinds of different information.
     */

    public static MusicManager Instance;

    [System.Serializable]
    public class Music
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

    public string nowPlaying;

    public Music[] getMusic;

    // Start is called before the first frame update
    public float timeSamples;

    public float[] positionSeconds;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public AudioClip GetMusic(string _name)
    {
        Music a = Array.Find(getMusic, sound => sound.name == _name);
        if (a == null)
        {
            Debug.LogWarning("Sound name " + _name + " was not found.");
            return null;
        }
        else
        {
            return a.clip;
        }
    }
}
