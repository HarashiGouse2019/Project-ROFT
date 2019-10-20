using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseInEffect : NoteEffect
{
    public float initiatedNoteSample;
    public float initiatedNoteOffset;
    public float offsetStart;

    public SpriteRenderer sprite;

    public int score;

    public int[] accuracyScore = new int[4]
    {
        1000,
        500,
        250,
        125
    };

    //Start
    private void Awake()
    {
        initiatedNoteSample = noteSample;
        initiatedNoteOffset = noteOffset;
        mapReader = MapReader.Instance;
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!EditorToolClass.Instance.record) CloseIn();
    }
    
    void CloseIn()
    {
        transform.localScale = new Vector3(1 / GetPercentage(), 1 / GetPercentage(), 1f);

        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, GetPercentage() - 0.2f);

        if (EditorToolClass.musicSource.timeSamples >= initiatedNoteSample) {
            AudioSource sound = gameObject.AddComponent<AudioSource>();
            sound.PlayOneShot(AudioManager.audio.GetAudio("Normal", 100f));
            gameObject.SetActive(false);
        }
    }

    protected override float GetPercentage()
    {
        percentage = (EditorToolClass.musicSource.timeSamples - offsetStart) / (initiatedNoteSample - offsetStart);
        return percentage;
    }

    void InHitRange()
    {

    }
}
