using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayPulseEffect : MonoBehaviour
{
    public RectTransform overlayTransform;

    const float fullPulse = 1.1f;
    const int minPulse = 2;

    Vector3 size;

    float time;

    bool waiting;

    const int reset = 0;

    private void Update()
    {
        if (waiting && !GameManager.Instance.isGamePaused) Wait();
    }

    public void DoPulseReaction()
    {
        size = new Vector3(fullPulse, overlayTransform.localScale.y, 1f);
        overlayTransform.localScale = size;
        waiting = true;
    }

    void Wait(float _duration = 1f)
    {
        time += Time.deltaTime;
        size = new Vector3(overlayTransform.localScale.x + 0.05f, overlayTransform.localScale.y, 1f);
        overlayTransform.localScale = size;
        if (time >= _duration)
        {
            size = new Vector3(minPulse, overlayTransform.localScale.y, 1f);
            overlayTransform.localScale = size;
            waiting = false;
            time = reset;
        }
    }
}
