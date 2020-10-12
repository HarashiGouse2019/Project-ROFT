using TMPro;
using UnityEngine;

public class DifficultySelection : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI difficultyDescription;

    [SerializeField, Header("Easy Mode Description"), TextArea]
    private string easyModeDescription;

    [SerializeField, Header("Normal Mode Description"), TextArea]
    private string normalModeDescription;

    [SerializeField, Header("Hard Mode Description"), TextArea]
    private string hardModeDescription;

    [SerializeField, Header("Insane Mode Description"), TextArea]
    private string insaneModeDescription;

    private string[] descriptions;

    void OnEnable()
    {
        Init();
    }

    /// <summary>
    /// Initalize Object
    /// </summary>
    void Init()
    {
        descriptions = new string[]
        {
            easyModeDescription,
            normalModeDescription,
            hardModeDescription,
            insaneModeDescription
        };
    }

    /// <summary>
    /// Update Difficulty Description Info
    /// </summary>
    /// <param name="value"></param>
    public void UpdateDescriptionInfo(int value)
    {
        difficultyDescription.text = descriptions[value];
    }
}
