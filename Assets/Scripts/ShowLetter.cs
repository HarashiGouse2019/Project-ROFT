using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowLetter : MonoBehaviour
{
    public TextMeshProUGUI TMP_LETTER;
    CloseInEffect closeInEffector;
    KeyCode assignedKeyBind;

    // Start is called before the first frame update
    void Start()
    {
        closeInEffector = GetComponent<CloseInEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        char letter = (char)assignedKeyBind;
        TMP_LETTER.rectTransform.position = new Vector3(transform.position.x, transform.position.y, 1f);
        TMP_LETTER.text = char.ToUpper(letter).ToString();
    }

    public void SetAssignedKeyBind(char _letter) => assignedKeyBind = (KeyCode)Convert.ToInt32(_letter);
}
