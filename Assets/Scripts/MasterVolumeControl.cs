using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MasterVolumeControl : Singleton<MasterVolumeControl>, IVolumeControl
{
    [SerializeField]
    private AudioMixerGroup masterAudioMixer;

    [SerializeField]
    private Slider masterVolumeAdjust;

    /// <summary>
    /// On Volume Change
    /// </summary>
    public void OnVolumeChange()
    {
        masterAudioMixer.audioMixer.SetFloat("MasterVolume", masterVolumeAdjust.value);
    }
}
