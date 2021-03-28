using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollableValue : MonoBehaviour
{
    public float sensitivity = 1f;
    public Slider sliderTarget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnMouseOver()
    {
        
        sliderTarget.value += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
    }
}
