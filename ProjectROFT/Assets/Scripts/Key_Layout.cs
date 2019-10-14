using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is going to be an abstract class. This will be the base class of all other layouts in the game

public class Key_Layout : MonoBehaviour
{
    //So I want to be able to allow the player to freely keybind key layouts (even though there's hardly any reason besides 4x4, 8x8, and 12x12)
    //Other than that, I want to go through a process of getting all homerow, toprow, and bottomrow keys.
    //KeyLayout will be given an enumerator

    #region Public Members
    public enum KeyLayoutType
    {
        Layout_4x4,
        Layout_8x8,
        Layout_12x12,
        Layout_HomeRow,
        Layout_3Row
    }

    public KeyLayoutType keyLayout;

    public bool autoBindKeys = true;

    //After iterating through strings, we'll return a Input corresponding avaliable keys.
    readonly public List<KeyCode> bindedKeys = new List<KeyCode>();
    public List<GameObject> keyObjects = new List<GameObject>();

    #endregion

    #region Private Members
    readonly private string[] defaultLayout = new string[5]
    {
        "asdf",
        "qwerasdf",
        "qwerasdfzxcv",
        "asdfghjkl;",
        "qwertyuiopasdfghjkl;zxcvbnm,./"
    };

    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        if(autoBindKeys) InitiateAutoKeyBind();
    }

    void InitiateAutoKeyBind()
    {
        KeyCode key;
        
        //Find all objects in parent
        GameObject[] keysFound = GameObject.FindGameObjectsWithTag("key");

        for (int keyNum = 0; keyNum < defaultLayout[(int)keyLayout].Length; keyNum++)
        {
            key = (KeyCode)defaultLayout[(int)keyLayout][keyNum];
            bindedKeys.Add(key);
            keyObjects.Add(keysFound[keyNum]);
        }

        
    }
}
