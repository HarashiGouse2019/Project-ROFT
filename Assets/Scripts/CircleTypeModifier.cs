using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleTypeModifier : MonoBehaviour
{
    public static CircleTypeModifier Instance;

    //This is used when a different type is set
    //The circles structure will change
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer innerSpriteRenderer;
    public NoteType[] circleTypes = new NoteType[5];

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeType(int spriteIndex)
    {
        spriteRenderer.sprite = circleTypes[spriteIndex].typeOuterTexture;
        innerSpriteRenderer.sprite = circleTypes[spriteIndex].typeInnerTexture;

        spriteRenderer.color = circleTypes[spriteIndex].typeColor;
    }
}
