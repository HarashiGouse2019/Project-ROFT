using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectMouse : MonoBehaviour
{
    void OnMouseOver()
    {
        Debug.Log("This is detecting the Timeline Control");
        EditorTimelineControl.Instance.EnableScroll();
    }
}
