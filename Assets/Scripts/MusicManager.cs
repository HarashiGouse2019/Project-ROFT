using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : Singleton<MusicManager>, IVolumeControl
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
    public AudioMixerGroup musicMixer;

    public static List<Song_Entity> Songs;

    public Slider musicVolumeAdjust; //Reference to our volume sliders

    public static string NowPlaying;

    void Start()
    {
        GameManager.Instance.ExecuteScouting();
    }

    public static List<Song_Entity> GetSongEntity() => Songs;
    public static MusicManager GetInstance() => Instance;

    /// <summary>
    /// On Volume Change
    /// </summary>
    public void OnVolumeChange()
    {
        musicMixer.audioMixer.SetFloat("BGMVolume", musicVolumeAdjust.value);
    }
}
