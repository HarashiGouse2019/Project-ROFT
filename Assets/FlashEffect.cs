using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    float time;

    const int reset = 0;

    Color currentAlpha; 
    readonly Color zeroAlpha = new Color(0f, 0f, 0f);
    readonly Color maxAlpha = new Color(1f, 1f, 1f);


    // Start is called before the first frame update
    void Start()
    {
        currentAlpha = spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        //Flash(5f);
    }

    void Flash(float _duration, float _rate = 0.05f)
    {
        time += Time.deltaTime;
        spriteRenderer.color = currentAlpha;

        if (time > _duration)
        {
            time = reset;
        }
    }
}
