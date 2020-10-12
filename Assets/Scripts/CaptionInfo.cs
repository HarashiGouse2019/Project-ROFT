using TMPro;
using UnityEngine;

public class CaptionInfo : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI TMP_Caption;

    public void DisplayCaption(string info)
    {
        TMP_Caption.text = info;
    }
}
