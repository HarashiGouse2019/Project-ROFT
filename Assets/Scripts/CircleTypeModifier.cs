using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleTypeModifier : MonoBehaviour
{
    public static CircleTypeModifier Instance;

    //This is used when a different type is set
    //The circles structure will change
    public SpriteRenderer spriteRenderer;

    //Now we have an array of different spites that we can use
    public Sprite[] circleTypeSprites = new Sprite[5];

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeType(int spriteIndex)
    {
        spriteRenderer.sprite = circleTypeSprites[spriteIndex];
    }
}
