using UnityEngine;

public class SongProgression : MonoBehaviour
{
    //This will be use to get different precentiles
    float firstNoteInSamples, lastNoteInSamples;

    //Check if the first note's been delivered
    public static bool isPassedFirstNote;

    //See if the last note was it
    public static bool isFinished;

    // Update is called once per frame
    void Update()
    {
        if (!RoftPlayer.Instance.record && isFinished == false)
            ShowProgression();
    }

    void ShowProgression()
    {

        //In order to know how much time we have until the first note,
        //we'll have to reference to the first note in our MapReader
        firstNoteInSamples = MapReader.Instance.noteObjs[0].instSample;

        //Now we calculate the percentage with the music sample and the firstNoteInSamples
        float firstNotePercentile = RoftPlayer.musicSource.timeSamples / firstNoteInSamples;

        //Now we want to get the percentage between
        //Our current sample, and the last key sample in our song!!!
        int lastKey = MapReader.Instance.noteObjs.Count - 1;

        lastNoteInSamples = MapReader.Instance.noteObjs[lastKey].instSample;

        //We get out percentage
        float lastNotePercentile = 
            (RoftPlayer.musicSource.timeSamples - firstNoteInSamples) /
            (lastNoteInSamples);

        if (!isPassedFirstNote)
        {
            GameManager.Instance.IMG_PROGRESSION_FILL.fillAmount = firstNotePercentile;

            if (firstNotePercentile > 0.99f)
                isPassedFirstNote = true;
        }
        else
            GameManager.Instance.IMG_PROGRESSION_FILL.fillAmount = lastNotePercentile;

        //Now, if the fillAmount is full (or once we hit the last note), have RoftPlayer fade out
        if (RoftPlayer.musicSource.timeSamples > lastNoteInSamples)
            isFinished = true;
    }
}
