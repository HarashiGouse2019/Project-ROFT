using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongSelectionNavigator : MonoBehaviour
{
    /*SongSelectionNavigator will be responsible for navigating through the Song Selection Menu.
     I will initialize a variable in which handles the position in the song. That information will be stored perhaps
     in the registery. Whether or not that's a good way to cache what song we are on... I can't say... but that's what we need.

     Of course, we need the contorls as well. These controls we'll be static, because they do not need to be remapped.
     They will be "these" sets of controls... forever...

     Also, we should state when we are looking at a song, and when the player had selected that song. I believe a value between
     0 and 2 will sarfice; 0 being just selecting the song (you should be able to see sutff like the Song Unified Score, number of difficulties,
     all the good stuff), 1 being difficulty selection (information like if they've been FCed or not, the total score for each one, the combos that's been
     made, as well as other states...), and then 2 (which is simply confirmation).

     Since Song Entites in the MusicManager is structured as "nested data", one value will be Song Entity Position, while the other
     will be Difficulty Position. All of this information will probably be saved as PlayerPrefs (unless I want to get fancy with it, and
     create another file type that keeps track of the position in the song selection menu). 

     Song Entity Position and Difficulty Position can be whatever value at this point, because just for simplicity, Difficulty Position will only have 3
     since there are only 3 difficulties. Song Entity Position can be as big as it wants so long you have like....
     a lot of downloaded songs into the game. This will most likely be an array that keeps track of positioning overall...
     Yeah, I'll have my individiual variables, but I'll assign the array with those values just to handle the overall position...
     Kind of like X, Y values... but for songs.

     As for the buttons, they are obviously going to be the arrow keys. Also, the mouse can also be used, so keep that 
     as a note. 

    That's probably it for the main functionality. We'll just set up our variables; take it one step at a time... see how that goes.
     */

    public enum NavigationState { 
        SONG_SELECTION,
        DIFFICULTY_SELECTION,
        CONFIRMATION
    }

    public enum Shift
    {
        INCREMENT,
        DECREMENT
    }

    [SerializeField] private uint SongEntityPosition;

    [SerializeField] private uint DifficultyPosition;

    private static uint[,] Position { get; set; }

    private static KeyCode[] NavigationControls =
    {
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.F5
    };

    private static NavigationState navigationState { get; set; }

    void Update()
    {
        if (Input.GetKeyDown(NavigationControls[0]))
            ChangeSongEntityPosition(Shift.DECREMENT, 1);

        if (Input.GetKeyDown(NavigationControls[1]))
            ChangeSongEntityPosition(Shift.INCREMENT, 1);

        if (Input.GetKeyDown(NavigationControls[2]))
            ChangeDifficultyPosition(Shift.DECREMENT, 1);

        if (Input.GetKeyDown(NavigationControls[3]))
            ChangeDifficultyPosition(Shift.INCREMENT, 1);
    }

    uint[,] GetLastPosition() 
    {
        /*Get last position will basically update our functions here,
         Update the SongEntityPosition, the Difficulty Difficulty, in which
         updates the overallPositioning*/
        uint[,] lastPos = new uint[3, 3];

        //This does nothing... for now...

        return lastPos;
    }

    void ChangeSongEntityPosition(Shift _shift, uint _value)
    {
        //Check value shift: shift up or down; + or -; increment or decrement.
        switch (_shift)
        {
            case Shift.INCREMENT: SongEntityPosition+=_value; return;
            case Shift.DECREMENT: SongEntityPosition-=_value; return;
            default: return;
        }

    }

    void ChangeDifficultyPosition(Shift _shift, uint _value)
    {
        //Check value shift
        switch (_shift)
        {
            case Shift.INCREMENT: DifficultyPosition+=_value; return;
            case Shift.DECREMENT: DifficultyPosition-=_value; return;
            default: return;
        }
    }
}
