using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateDivisors : MonoBehaviour
{
    Image imageToZoom;
    float currentScale = 1;

    const float minScale = 0.2f;
    const float maxScale = 5f;

    private void Start()
    {
        imageToZoom = GetComponent<Image>();
    }
    void Update()
    {
        Zoom(Input.GetAxis("Mouse ScrollWheel"));
    }

    void Zoom(float increment)
    {
        currentScale += increment;
        if (currentScale >= maxScale)
        {
            currentScale = maxScale;
        }
        else if (currentScale <= minScale)
        {
            currentScale = minScale;
        }
        imageToZoom.rectTransform.localScale = new Vector2(currentScale, 1);
    }
}
