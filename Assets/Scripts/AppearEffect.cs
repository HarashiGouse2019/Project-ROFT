using UnityEngine;

public class AppearEffect : CloseInEffect
{
    public KeyCode assignedKeyBind;

    //public SpriteRenderer sprite;
    public SpriteRenderer[] overlaySprites;
    public SpriteRenderer childSprite;

    public GameObject assignedCircle;

    Color originalAppearance;
    Color originalOverlayAppearance;

    bool fullOpacity = false;

    void Awake()
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


        if (Key_Layout.Instance.layoutMethod == Key_Layout.LayoutMethod.Region_Scatter)
        {
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
        initiatedNoteSample = noteSampleForKey;
        initiatedNoteOffset = noteOffset;
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!EditorToolClass.Instance.record)
            AppearOn();
    }

    private void FixedUpdate()
    {
        if (dispose)
        {
            gameObject.SetActive(false);
        }
    }

    void AppearOn()
    {
        float appearanceRate = GetPercentage() - 0.6f;

        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, appearanceRate);
        childSprite.color = new Color(childSprite.color.r, childSprite.color.g, childSprite.color.b, appearanceRate);

        if (assignedCircle.GetComponent<CloseInEffect>().accuracyString == "perfect")
        {
            gameObject.SetActive(false);
        }
    }

    protected override float GetPercentage()
    {
        percentage = ((EditorToolClass.musicSource.timeSamples) - offsetStart) / (initiatedNoteSample - offsetStart);
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
