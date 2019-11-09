using UnityEngine;

public class AppearEffect : CloseInEffect
{
    //public float initiatedNoteSample;
    //public float initiatedNoteOffset;
    //public float offsetStart;

    public KeyCode assignedKeyBind;

    //public SpriteRenderer sprite;
    public SpriteRenderer[] overlaySprites;
    public SpriteRenderer childSprite;

    Color originalAppearance;
    Color originalOverlayAppearance;

    void Awake()
    {
        

        if (Key_Layout.Instance.layoutMethod == Key_Layout.LayoutMethod.Region_Scatter)
        {
            Instance = this;

            //Get sprite renderers
            sprite = GetComponent<SpriteRenderer>();
            overlaySprites = GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer overlaySprite in overlaySprites)
            {
                if (overlaySprite != sprite)
                    childSprite = overlaySprite;
            }


            //We want these completely transparent from start
            childSprite.color = new Color(childSprite.color.r, childSprite.color.g, childSprite.color.b, 0f);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);

            //Assign this to orignal variable
            originalAppearance = sprite.color;
            originalOverlayAppearance = childSprite.color;
        }
    }

    private void OnEnable()
    {
        initiatedNoteSample = noteSample;
        initiatedNoteOffset = noteOffset;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!EditorToolClass.Instance.record)
            AppearOn();
    }

    void AppearOn()
    {
        float appearanceRate = GetPercentage();

        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, appearanceRate);
        childSprite.color = new Color(childSprite.color.r, childSprite.color.g, childSprite.color.b, appearanceRate);

        if (accuracyString != "" && Input.GetKeyDown(assignedKeyBind))
        {
            dispose = true;
        }
    }

    protected override float GetPercentage()
    {
        percentage = ((EditorToolClass.musicSource.timeSamples) - offsetStart - 3000) / (initiatedNoteSample - offsetStart);
        return percentage;
    }

    private void OnDisable()
    {
        sprite.color = originalAppearance;
        childSprite.color = originalOverlayAppearance;
        percentage = 0;
        assignedKeyBind = KeyCode.None;
        initiatedNoteSample = 0;
        initiatedNoteOffset = 0;
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }
}
