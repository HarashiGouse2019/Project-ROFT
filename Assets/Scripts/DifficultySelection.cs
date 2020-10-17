using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySelection : MonoBehaviour
{
    [SerializeField, Header("Difficulty Select Button")]
    private Button difficultySelectButton;
    [SerializeField]
    private TextMeshProUGUI buttonText;

    [SerializeField]
    private TextMeshProUGUI difficultyDescription;

    [SerializeField, Header("Easy Mode Description"), TextArea]
    private string easyModeDescription;
    [SerializeField]
    private Color easyModeColor;

    [SerializeField, Header("Normal Mode Description"), TextArea]
    private string normalModeDescription;
    [SerializeField]
    private Color normalModeColor;

    [SerializeField, Header("Hard Mode Description"), TextArea]
    private string hardModeDescription;
    [SerializeField]
    private Color hardModeColor;

    [SerializeField, Header("Expert Mode Description"), TextArea]
    private string expertModeDescription;
    [SerializeField]
    private Color insaneModeColor;

    private string[] descriptions;
    private Color[] colors;
    private readonly string[] difficultyName =
    {
        "Easy",
        "Normal",
        "Hard",
        "Expert"
    };

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
            expertModeDescription
        };

        colors = new Color[]
        {
            easyModeColor,
            normalModeColor,
            hardModeColor,
            insaneModeColor
        };
    }

    /// <summary>
    /// Update Difficulty Description Info
    /// </summary>
    /// <param name="value"></param>
    public void UpdateDescriptionInfo(int value)
    {
        ColorBlock colorblock = difficultySelectButton.colors;

        difficultyDescription.text = descriptions[value];

        colorblock.normalColor = colors[value];
        difficultySelectButton.colors = colorblock;

        GameManager.UpdateLogColor(colors[value]);

        buttonText.text = difficultyName[value];
    }
}
