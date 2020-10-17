using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ColorChanger : MonoBehaviour
{
    [SerializeField]
    private Button logButton;

    private ColorBlock logColorBlock;
    private Color highLightedColor;

    private void OnEnable()
    {
        StartCoroutine(ColorChangeCycle());
    }

    IEnumerator ColorChangeCycle()
    {
        while (true)
        {
            if(highLightedColor != GameManager.LogNormalColor)
            {
                logColorBlock = logButton.colors;
                highLightedColor = GameManager.LogNormalColor;
                logColorBlock.highlightedColor = highLightedColor;
                logButton.colors = logColorBlock;
            }

            yield return null;
        }
    }
}
