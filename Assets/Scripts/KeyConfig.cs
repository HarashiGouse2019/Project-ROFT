using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class KeyConfig
{
    public static KeyConfig Instance;

    //This class will be responsible of saving all configurations of how the keys should actually be layed out when
    //playing a song in the game.
    //This is only for entering a game. This gives the player to be able to adjust the scale, as well as the positioning of the
    //keyLayout. This will be saved as a JSON class, just for the sake of learning how to use it.
    //Of course we need a reference to the Key_Layout class.

    //We want to get all the values that keyLayout also has, because that will be important... I think...
    //No! Actually, what we want to do is take all of those values out from Key_Layout, and move it to this class.
    //Now that I think about it, having a separate class to handle the configuration of the layout of the keys
    //sounds a lot more to handle, and I really do approve that message! Yay!!!!!!
    [SerializeField]
    readonly private float[] defaultKeyScale = new float[5]
    {
        2.25f,
        1.75f,
        1.4f,
        1.4f,
        1f
    };

    [SerializeField]
    readonly private float[] keyHorizontalSpread = new float[5] {
        3.5f,
        2.5f,
        2f,
        2.25f,
        1.5f
    };

    [SerializeField]
    readonly private float[] keyVerticalSpread = new float[5]
    {
        3.5f,
        2.5f,
        2f,
        1.75f,
        1.5f
    };

    //String info
    private static string json;

    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SaveConfig()
    {
        /*SaveConfig method will be responsible of saving our scale, the position of different
         key totals, and the scale and distribution of it. That will then be save and send into the playerprefs,
         saving those preferences onto the user's computer.*/
        CreateJSON();


        
    }

    public static void LoadConfig()
    {
        /*LoadConfig will load all values from player prefs, and will use the referenced Key_Layout
         and assign those respective values to it before starting the song.*/
    }

    private static void CreateJSON()
    {
        //Simply make this into a JSON
        json = JsonUtility.ToJson(Instance);
    }

    private static KeyConfig LoadJSON() => JsonUtility.FromJson<KeyConfig>(json);
}
