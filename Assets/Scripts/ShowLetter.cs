using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowLetter : MonoBehaviour
{
    public TextMeshPro TMP_LETTER;
    AppearEffect appearEffector;

    // Start is called before the first frame update
    void Start()
    {
        appearEffector = GetComponent<AppearEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        char letter = (char)appearEffector.assignedKeyBind;
        TMP_LETTER.rectTransform.position = new Vector3(transform.position.x, transform.position.y, 1f);
        TMP_LETTER.text = char.ToUpper(letter).ToString();
    }
}
