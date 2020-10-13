using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleTypeModifier : MonoBehaviour
{
    public static CircleTypeModifier Instance;

    //This is used when a different type is set
    //The circles structure will change
    public Image spriteRenderer;
    public Image innerSpriteRenderer;
    public NoteType[] circleTypes = new NoteType[5];

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeType(int spriteIndex)
    {
        if(spriteRenderer != null) spriteRenderer.sprite = circleTypes[spriteIndex].typeOuterTexture;
        if(innerSpriteRenderer != null) innerSpriteRenderer.sprite = circleTypes[spriteIndex].typeInnerTexture;

        spriteRenderer.color = circleTypes[spriteIndex].typeColor;
    }
}
