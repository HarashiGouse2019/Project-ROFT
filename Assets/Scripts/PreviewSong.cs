using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSong : MonoBehaviour
{
    [SerializeField]
    private AudioSource currentSongPreviewing;

    [SerializeField]
    private float offsetStart = 0f;

    // Start is called before the first frame update
    void Start()
    {
        PlaySongPreview();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentSongPreviewing.clip != null &&
            currentSongPreviewing.timeSamples >= (currentSongPreviewing.clip.frequency * currentSongPreviewing.clip.length) &&
            !currentSongPreviewing.isPlaying)
        {
            currentSongPreviewing.timeSamples = 0;
            PlaySongPreview();
        }
    }

    internal void PlaySongPreview()
    {
        //Convert the offsetStart to samples, and add the samples to the song samples
        //Then play the song.
        if (currentSongPreviewing.clip != null)
        {
            currentSongPreviewing.volume = 0f;
            FadeIn();
            currentSongPreviewing.timeSamples = 0;
            currentSongPreviewing.timeSamples += (int)(currentSongPreviewing.clip.frequency * offsetStart);
            currentSongPreviewing.Play();
        }
    }

    internal void SetSongToPreview(AudioClip song)
    {
        currentSongPreviewing.clip = song;
    }

    void FadeIn()
    {
        StartCoroutine(FadeInCycle());
    }

    void FadeOut()
    {
        StartCoroutine(FadeOutCycle());
    }

    internal IEnumerator FadeInCycle()
    {
        for (float i = 0; i < 1; i += 0.01f)
        {
            currentSongPreviewing.volume = i;
            yield return null;
        }
    }

    internal IEnumerator FadeOutCycle()
    {
        for (float i = 1; i > 0; i -= 0.01f)
        {
            currentSongPreviewing.volume = i;
            yield return null;
        }
    }
}
