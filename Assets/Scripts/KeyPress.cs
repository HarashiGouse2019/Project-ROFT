using UnityEngine;
using TMPro;

using ROFTIOMANAGEMENT;

[RequireComponent(typeof(Key_Layout))]
public class KeyPress : MonoBehaviour
{
    public static KeyPress Instance;

    //Taking the binded keys, and making them interactive
    [Cakewalk.IoC.Dependency]
    public Key_Layout key_Layout;

    public Sprite keyActive, keyInActive;

    public TextMeshProUGUI debugText;

    //Input Value for KeyPress
    public int keyPressInput = 0;
    const int activeInput = 1;
    const int inactiveInput = 0;


    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        if (key_Layout == null)
            key_Layout = GetComponent<Key_Layout>();
    }

    // Update is called once per frame
    void Update()
    {
        RunInteractivity();
    }

    /*This is how key presses are being registered in game.
    This is also how beatmaps can be recorded manually by the user.*/
    void RunInteractivity()
    {
        //Check if we can interact with keys
        if (GameManager.Instance.IsInteractable())
        {
            for (int keyNum = 0; keyNum < key_Layout.primaryBindedKeys.Count; keyNum++)
            {
                bool bindKeys = Input.GetKey(key_Layout.primaryBindedKeys[keyNum]) || Input.GetKey(key_Layout.secondaryBindedKeys[keyNum]);
                bool bindKeysPressed = Input.GetKeyDown(key_Layout.primaryBindedKeys[keyNum]) || Input.GetKeyDown(key_Layout.secondaryBindedKeys[keyNum]);

                if (bindKeys)
                {
                    /*If we happen to be recording, and we hit the second set of binded keys, the
                    data will be written to a file.*/
                    #region Write to RFTM File
                    if (RoftPlayer.Instance.record && bindKeysPressed)
                    {
                        string data =
                            keyNum.ToString() + ","
                             + RoftPlayer.musicSource.timeSamples.ToString() + ","
                            + 0.ToString();

                        RoftIO.WriteNewObjectToRFTM(RoftCreator.filename, RoftCreator.newSongDirectoryPath + "/", data);
                    }
                    #endregion

                    ActivateKey(keyNum, true);

                    if (bindKeys)
                        keyPressInput = activeInput;
                }
                else
                    ActivateKey(keyNum, false);
            }

        }
    }

    //This will turn the keys on, signifying that the key is being pressed
    public bool ActivateKey(int _keyNum, bool _on)
    {
        SpriteRenderer keySpriteRenderer = Key_Layout.keyObjects[_keyNum].GetComponent<SpriteRenderer>();

        if (_on)
        {
            keySpriteRenderer.sprite = keyActive;
            debugText.text = "Key " + key_Layout.primaryBindedKeys[_keyNum] + " pressed." + " Key Num: " + _keyNum;
        }
        else
            keySpriteRenderer.sprite = keyInActive;

        return _on;
    }

    public int GetKeyPressInputValue() => keyPressInput;
}
