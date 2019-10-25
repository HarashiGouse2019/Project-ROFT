using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectMouse : MonoBehaviour
{
    void OnMouseOver()
    {
        EditorTimelineControl.Instance.EnableScroll();
    }
}
