using UnityEngine;
using TMPro;

using ROFTIOMANAGEMENT;

[RequireComponent(typeof(Key_Layout))]
public class KeyPress : MonoBehaviour
{
    public static KeyPress Instance;

    //Taking the binded keys, and making them interactive
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

    void RunInteractivity()
    {
        if (GameManager.Instance.IsInteractable())
        {
            for (int keyNum = 0; keyNum < key_Layout.primaryBindedKeys.Count; keyNum++)
            {
                if (Input.GetKey(key_Layout.primaryBindedKeys[keyNum]) || Input.GetKey(key_Layout.secondaryBindedKeys[keyNum]))
                {
                    #region Write to RFTM File
                    if (RoftPlayer.Instance.record && Input.GetKeyDown(key_Layout.secondaryBindedKeys[keyNum]))
                    {
                        string data =
                            keyNum.ToString() + ","
                             + RoftPlayer.musicSource.timeSamples.ToString() + ","
                            + 0.ToString();

                        RoftIO.WriteToRFTM(RoftCreator.filename, Application.streamingAssetsPath + "/", data);
                    }
                    #endregion

                    ActivateKey(keyNum, true);

                    if (Input.GetKey(key_Layout.primaryBindedKeys[keyNum]) || Input.GetKey(key_Layout.secondaryBindedKeys[keyNum]))
                        keyPressInput = activeInput;
                }
                else
                    ActivateKey(keyNum, false);
            }

        }
    }

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
