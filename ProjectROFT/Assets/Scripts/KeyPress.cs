using UnityEngine;
using TMPro;

public class KeyPress : MonoBehaviour
{
    //Taking the binded keys, and making them interactive
    public Key_Layout key_Layout;

    public Sprite keyActive, keyInActive;

    public TextMeshProUGUI debugText;
    
    // Start is called before the first frame update
    void Awake()
    {
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
                ActivateKey(keyNum, true);
            else
                ActivateKey(keyNum, false);
        }
    }

    void ActivateKey(int _keyNum, bool _on)
    {
        SpriteRenderer keySpriteRenderer = key_Layout.keyObjects[_keyNum].GetComponent<SpriteRenderer>();

        if (_on)
        {
            keySpriteRenderer.sprite = keyActive;
            debugText.text = "Key " + key_Layout.bindedKeys[_keyNum] + " pressed." + " Key Num: " + _keyNum;
        }
        else
            keySpriteRenderer.sprite = keyInActive;
    }
}
