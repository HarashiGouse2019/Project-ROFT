using UnityEngine;
using TMPro;

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

    float time;
    float keyResponsiveDuration = 0.25f;
    const int reset = 0;

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


        for (int keyNum = 0; keyNum < key_Layout.bindedKeys.Count; keyNum++)
        {

            if (Input.GetKey(key_Layout.bindedKeys[keyNum]))
            {
                #region Write to RFTM File
                if (EditorToolClass.Instance.record && Input.GetKeyDown(key_Layout.bindedKeys[keyNum]))
                {
                    string data =
                        keyNum.ToString() + ","
                         + EditorToolClass.musicSource.timeSamples.ToString() + ","
                        + 0.ToString();

                    EditorToolClass.Instance.WriteToRFTM(EditorToolClass.musicSource.clip.name, Application.streamingAssetsPath + "/", data);
                }
                #endregion

                ActivateKey(keyNum, true);

                if (Input.GetKeyDown(key_Layout.bindedKeys[keyNum]))
                    keyPressInput = activeInput;
            }
            else
                ActivateKey(keyNum, false);
        }


    }

    public bool ActivateKey(int _keyNum, bool _on)
    {
        SpriteRenderer keySpriteRenderer = Key_Layout.keyObjects[_keyNum].GetComponent<SpriteRenderer>();

        if (_on)
        {
            keySpriteRenderer.sprite = keyActive;
            debugText.text = "Key " + key_Layout.bindedKeys[_keyNum] + " pressed." + " Key Num: " + _keyNum;
        }
        else
            keySpriteRenderer.sprite = keyInActive;

        return _on;
    }

    public int GetKeyPressInputValue()
    {
        return keyPressInput;
    }

    void StartKeyResponsiveDelay()
    {
        time += Time.deltaTime;
        if (time >= keyResponsiveDuration)
        {
            keyPressInput = inactiveInput;
            time = reset;
        }
    }

    public void ResetResponsiveDelay()
    {
        time = reset;
    }
}
