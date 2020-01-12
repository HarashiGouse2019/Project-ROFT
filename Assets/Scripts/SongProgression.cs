using UnityEngine;

public class SongProgression : MonoBehaviour
{
    //This will be use to get different precentiles
    float firstNoteInSamples, lastNoteInSamples;

    //Check if the first note's been delivered
    public static bool isPassedFirstNote;

    // Update is called once per frame
    void Update()
    {
        if (!RoftPlayer.Instance.record)
            ShowProgression();
    }

    void ShowProgression()
    {

        //In order to know how much time we have until the first note,
        //we'll have to reference to the first note in our MapReader
        firstNoteInSamples = MapReader.Instance.keys[0].keySample;

        //Now we calculate the percentage with the music sample and the firstNoteInSamples
        float firstNotePercentile = RoftPlayer.musicSource.timeSamples / firstNoteInSamples;

        //Now we want to get the percentage between
        //Our current sample, and the last key sample in our song!!!
        int lastKey = MapReader.Instance.keys.Count - 1;

        lastNoteInSamples = MapReader.Instance.keys[lastKey].keySample;

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
    }
}
