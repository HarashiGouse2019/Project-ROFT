using UnityEngine;

public class LobbyOptions : MonoBehaviour
{
    public void OnSinglePlay()
    {
        RoftTransition.TransitionTo(1);
        GameManager.ChangeSongSelectionMode(0);
    }

    public void OnMultiPlay()
    {
        RoftTransition.TransitionTo(1);
        GameManager.ChangeSongSelectionMode(0);
    }

    public void OnMarathon()
    {

    }

    public void OnSongEditor()
    {
        RoftTransition.TransitionTo(1);
        GameManager.ChangeSongSelectionMode(1);
    }

    public void OnMusicPlayer()
    {
        
    }

    public void OnRecords()
    {

    }

    public void OnSettings()
    {

    }

    public void OnExit()
    {
        //For now, just quit the application
        Application.Quit();
    }
}
